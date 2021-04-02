﻿using Dev.Editor.BusinessLogic.Models.SubstitutionCipher;
using Dev.Editor.BusinessLogic.Services.Dialogs;
using Dev.Editor.BusinessLogic.Services.Messaging;
using Dev.Editor.BusinessLogic.Services.SubstitutionCipher;
using Dev.Editor.BusinessLogic.Types.SubstitutionCipher;
using Dev.Editor.BusinessLogic.ViewModels.Base;
using ICSharpCode.AvalonEdit.Document;
using Spooksoft.VisualStateManager.Commands;
using Spooksoft.VisualStateManager.Conditions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dev.Editor.BusinessLogic.ViewModels.SubstitutionCipher
{
    public class SubstitutionCipherWindowViewModel : BaseViewModel, IAlphabetEntryHandler
    {
        // Private types ------------------------------------------------------

        private class UniqueValueComparer : IEqualityComparer<KeyValuePair<char, char>>
        {
            public bool Equals(KeyValuePair<char, char> x, KeyValuePair<char, char> y)
            {
                return x.Value.Equals(y.Value);
            }

            public int GetHashCode(KeyValuePair<char, char> obj)
            {
                return obj.Value.GetHashCode();
            }
        }

        private class WorkerInput
        {
            public WorkerInput(Dictionary<char, char> key, string data, bool forward)
            {
                Key = key;
                Data = data;
                Forward = forward;
            }

            public Dictionary<char, char> Key { get; }
            public string Data { get; }
            public bool Forward { get; }
        }

        private class WorkerOutput
        {
            public WorkerOutput(string result)
            {
                Result = result;
            }

            public string Result { get; }
        }

        private class CipherWorker : BackgroundWorker
        {
            public CipherWorker()
            {
                WorkerSupportsCancellation = true;
            }

            protected override void OnDoWork(DoWorkEventArgs e)
            {
                var input = e.Argument as WorkerInput;

                if (input.Data == null)
                {
                    e.Result = null;
                    return;
                }

                if (input.Key == null)
                    throw new InvalidOperationException("Key cannot be null!");
                if (input.Key.Any(kvp => !char.IsLower(kvp.Key) || !char.IsLower(kvp.Value)))
                    throw new InvalidOperationException("Only lowercase letters are allowed in the key!");

                Dictionary<char, char> key;
                if (input.Forward)
                    key = input.Key;
                else
                {
                    key = input.Key
                        .Distinct(new UniqueValueComparer())
                        .ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
                }

                StringBuilder result = new StringBuilder();
                
                foreach (var dataChar in input.Data)
                {
                    bool isUpper = char.IsUpper(dataChar);

                    var dataCharKey = char.ToLowerInvariant(dataChar);
                    if (key.ContainsKey(dataCharKey))
                    {
                        if (isUpper)
                            result.Append(char.ToUpper(key[dataCharKey]));
                        else
                            result.Append(key[dataCharKey]);
                    }
                    else
                    {
                        result.Append(dataCharKey);
                    }

                    if (CancellationPending)
                    {
                        e.Result = null;
                        return;
                    }
                }

                e.Result = new WorkerOutput(result.ToString());
            }
        }

        private class LanguageDataBuilderWorker : BackgroundWorker
        {
            private readonly ISubstitutionCipherService substitutionCipherService;

            public LanguageDataBuilderWorker(ISubstitutionCipherService substitutionCipherService)
            {
                this.substitutionCipherService = substitutionCipherService;

                WorkerSupportsCancellation = true;
                WorkerReportsProgress = true;
            }

            protected override void OnDoWork(DoWorkEventArgs e)
            {
                var data = e.Argument as string[];

                var statsModel = substitutionCipherService.InitializeLanguageStatisticsModel();

                for (int i = 0; i < data.Length; i++)
                {
                    if (CancellationPending)
                        return;

                    ReportProgress(i * 100 / data.Length);

                    var line = data[i];
                    substitutionCipherService.AddLineToLanguageStatisticsModel(statsModel, line);
                }

                var info = substitutionCipherService.BuildLanguageInfoModel(statsModel);
                e.Result = info;
            }
        }

        // Private fields -----------------------------------------------------

        private readonly ISubstitutionCipherHost host;
        private readonly IDialogService dialogService;
        private readonly IMessagingService messagingService;
        private readonly ISubstitutionCipherService substitutionCipherService;
        private readonly ISubstitutionCipherWindowAccess access;

        private readonly TextDocument plaintextDoc;
        private readonly TextDocument cipherDoc;
        
        private SubstitutionCipherMode mode;

        private LanguageInfoModel languageData;

        private CipherWorker currentWorker;

        private readonly ObservableCollection<AlphabetEntryViewModel> alphabet = new ObservableCollection<AlphabetEntryViewModel>();

        private readonly BaseCondition languageDataAvailableCondition;

        // Private methods ----------------------------------------------------

        private void SetPlaintext(object sender, RunWorkerCompletedEventArgs e)
        {
            var result = e.Result as WorkerOutput;
            plaintextDoc.Text = result.Result;
        }

        private void SetCipherText(object sender, RunWorkerCompletedEventArgs e)
        {
            var result = e.Result as WorkerOutput;
            cipherDoc.Text = result.Result;
        }

        private void DoEnterAlphabet()
        {
            string previousAlphabet = null;
            if (alphabet != null)
                previousAlphabet = String.Join("", alphabet
                    .Select(e => e.Plaintext)
                    .OrderBy(x => x));

            (bool result, string newAlphabet) = dialogService.ShowAlphabetDialog(previousAlphabet);
            if (result)
            {
                // Move entries from old to new alphabet
                var previous = alphabet.ToList();
                alphabet.Clear();

                if (!string.IsNullOrEmpty(newAlphabet))
                {
                    foreach (char ch in newAlphabet.Distinct().OrderBy(x => x))
                    {
                        var entry = new AlphabetEntryViewModel(ch, this);
                        var oldEntry = previous.FirstOrDefault(e => e.Plaintext.Equals(entry.Plaintext));
                        if (oldEntry != null)
                            entry.Cipher = oldEntry.Cipher;

                        alphabet.Add(entry);
                    }
                }

                RestartActionTimer();
            }
        }

        private void RestartActionTimer()
        {
            access.RestartActionTimer();
        }

        private void HandleCipherTextChanged(object sender, DocumentChangeEventArgs e)
        {
            if (mode == SubstitutionCipherMode.Uncipher)
                RestartActionTimer();
        }

        private void HandlePlaintextChanged(object sender, DocumentChangeEventArgs e)
        {
            if (mode == SubstitutionCipherMode.Cipher)
                RestartActionTimer();
        }

        private void HandleModeChanged()
        {
            RestartActionTimer();
        }

        private void ValidateAlphabet()
        {
            if (alphabet == null)
                return;

            var groups = alphabet.GroupBy(e => e.Cipher?.ToLowerInvariant());

            foreach (var group in groups)
            {
                if (group.Count() == 1 || string.IsNullOrEmpty(group.Key))
                {
                    foreach (var item in group)
                        item.IsDoubled = false;
                }
                else
                {
                    foreach (var item in group)
                        item.IsDoubled = true;
                }
            }
        }

        private void DoSwitchModeToUncipher()
        {
            Mode = SubstitutionCipherMode.Uncipher;
        }

        private void DoSwitchModeToCipher()
        {
            Mode = SubstitutionCipherMode.Cipher;
        }

        private void DoSaveLanguageData()
        {
            throw new NotImplementedException();
        }

        private void DoOpenLanguageData()
        {
            throw new NotImplementedException();
        }

        private void DoGenerateLanguageData()
        {
            var result = dialogService.ShowOpenDialog("#Text files (*.txt)|*.txt");
            if (result.Result)
            {
                string[] lines;
                try
                {
                    lines = File.ReadAllLines(result.FileName);
                }
                catch (IOException)
                {
                    messagingService.ShowError("#Failed to open language sample file!");
                    return;
                }

                var worker = new LanguageDataBuilderWorker(substitutionCipherService);
                worker.RunWorkerCompleted += HandleLanguageDataBuilt;

                dialogService.ShowProgressDialog("#Building language data...", worker, lines);
            }
        }

        private void HandleLanguageDataBuilt(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                LanguageData = e.Result as LanguageInfoModel;
            }
        }

        // IAlphabetEntryHandler implementation -------------------------------

        void IAlphabetEntryHandler.NotifyChanged(AlphabetEntryViewModel alphabetEntryViewModel)
        {
            ValidateAlphabet();

            RestartActionTimer();
        }

        // Public methods -----------------------------------------------------

        public SubstitutionCipherWindowViewModel(IDialogService dialogService, 
            IMessagingService messagingService, 
            ISubstitutionCipherService substitutionCipherService,
            ISubstitutionCipherWindowAccess access, 
            ISubstitutionCipherHost host)
        {
            this.dialogService = dialogService;
            this.messagingService = messagingService;
            this.substitutionCipherService = substitutionCipherService;
            this.access = access;
            this.host = host;

            mode = SubstitutionCipherMode.Cipher;
            plaintextDoc = new TextDocument();
            plaintextDoc.Changed += HandlePlaintextChanged;

            cipherDoc = new TextDocument();
            cipherDoc.Changed += HandleCipherTextChanged;

            languageDataAvailableCondition = new LambdaCondition<SubstitutionCipherWindowViewModel>(this, vm => vm.LanguageData != null, false);

            EnterAlphabetCommand = new AppCommand(obj => DoEnterAlphabet());
            SwitchModeToCipherCommand = new AppCommand(obj => DoSwitchModeToCipher());
            SwitchModeToUncipherCommand = new AppCommand(obj => DoSwitchModeToUncipher());

            GenerateLanguageDataCommand = new AppCommand(obj => DoGenerateLanguageData());
            OpenLanguageDataCommand = new AppCommand(obj => DoOpenLanguageData());
            SaveLanguageDataCommand = new AppCommand(obj => DoSaveLanguageData(), languageDataAvailableCondition);
        }

        public void NotifyActionTimerElapsed()
        {
            StartCipherOperation();
        }

        private void StartCipherOperation()
        {
            if (currentWorker != null)
            {
                currentWorker.CancelAsync();
                currentWorker = null;
            }

            Dictionary<char, char> key;
            if (alphabet != null)
                key = alphabet
                    .Where(e => !String.IsNullOrEmpty(e.Cipher) && !e.IsDoubled)
                    .ToDictionary(e => e.Plaintext[0], e => e.Cipher[0]);
            else
                key = new Dictionary<char, char>();

            string data;
            bool forward;
            RunWorkerCompletedEventHandler completeHandler;

            switch (mode)
            {
                case SubstitutionCipherMode.Cipher:
                    {
                        data = plaintextDoc.Text;
                        forward = true;
                        completeHandler = SetCipherText;
                        break;
                    }
                case SubstitutionCipherMode.Uncipher:
                    {
                        data = cipherDoc.Text;
                        forward = false;
                        completeHandler = SetPlaintext;
                        break;
                    }
                default:
                    throw new InvalidEnumArgumentException("Unsupported cipher mode!");
            }

            var input = new WorkerInput(key, data, forward);
            currentWorker = new CipherWorker();
            currentWorker.RunWorkerCompleted += completeHandler;
            currentWorker.RunWorkerAsync(input);
        }

        // Public properties --------------------------------------------------

        public TextDocument PlaintextDoc => plaintextDoc;

        public TextDocument CipherDoc => cipherDoc;

        public SubstitutionCipherMode Mode
        {
            get => mode;
            set => Set(ref mode, () => Mode, value, HandleModeChanged);
        }

        public LanguageInfoModel LanguageData
        {
            get => languageData;
            set => Set(ref languageData, () => LanguageData, value);
        }

        public ObservableCollection<AlphabetEntryViewModel> Alphabet => alphabet;

        public ICommand EnterAlphabetCommand { get; }
        public ICommand SwitchModeToCipherCommand { get; }
        public ICommand SwitchModeToUncipherCommand { get; }
        public ICommand GenerateLanguageDataCommand { get; }
        public ICommand OpenLanguageDataCommand { get; }
        public ICommand SaveLanguageDataCommand { get; }
    }
}
