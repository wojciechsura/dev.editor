using Dev.Editor.BusinessLogic.Models.SubstitutionCipher;
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

        private class CipherWorkerInput
        {
            public CipherWorkerInput(Dictionary<char, char> key, string data, bool forward)
            {
                Key = key;
                Data = data;
                Forward = forward;
            }

            public Dictionary<char, char> Key { get; }
            public string Data { get; }
            public bool Forward { get; }
        }

        private class CipherWorkerOutput
        {
            public CipherWorkerOutput(string result)
            {
                Result = result;
            }

            public string Result { get; }
        }

        private class CrackWorkerInput
        {
            public CrackWorkerInput(Dictionary<char, char> key, string data, LanguageInfoModel languageInfo)
            {
                Key = key;
                Data = data;
                LanguageInfo = languageInfo;
            }

            public Dictionary<char, char> Key { get; }
            public string Data { get; }
            public LanguageInfoModel LanguageInfo { get; }
        }

        private class CrackWorkerOutput
        {
            public CrackWorkerOutput(Dictionary<char, char> bestKey)
            {
                BestKey = bestKey;
            }

            public Dictionary<char, char> BestKey { get; }
        }

        private class CipherWorker : BackgroundWorker
        {
            private readonly ISubstitutionCipherService substitutionCipherService;

            public CipherWorker(ISubstitutionCipherService substitutionCipherService)
            {
                WorkerSupportsCancellation = true;
                this.substitutionCipherService = substitutionCipherService;
            }

            protected override void OnDoWork(DoWorkEventArgs e)
            {
                var input = e.Argument as CipherWorkerInput;
                var result = substitutionCipherService.Process(input.Key, input.Data, input.Forward, () => CancellationPending);
                e.Result = new CipherWorkerOutput(result);
            }
        }

        private class CrackWorker : BackgroundWorker
        {
            private readonly ISubstitutionCipherService substitutionCipherService;

            public CrackWorker(ISubstitutionCipherService substitutionCipherService)
            {
                WorkerSupportsCancellation = true;
                WorkerReportsProgress = true;
                this.substitutionCipherService = substitutionCipherService;
            }

            protected override void OnDoWork(DoWorkEventArgs e)
            {
                var input = e.Argument as CrackWorkerInput;
                var result = substitutionCipherService.TrySolve(input.Data, input.Key, input.LanguageInfo, () => CancellationPending, progress => ReportProgress(progress));
                e.Result = new CrackWorkerOutput(result);
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
                var info = substitutionCipherService.BuildLanguageInfoModel(data, () => CancellationPending, progress => ReportProgress(progress));
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
        private readonly BaseCondition modeIsUncipher;

        // Private methods ----------------------------------------------------

        private void SetPlaintext(object sender, RunWorkerCompletedEventArgs e)
        {
            var result = e.Result as CipherWorkerOutput;
            plaintextDoc.Text = result.Result;
        }

        private void SetCipherText(object sender, RunWorkerCompletedEventArgs e)
        {
            var result = e.Result as CipherWorkerOutput;
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
            var result = dialogService.ShowSaveDialog("#Language data files (*.langdata)|*.langdata");
            if (result.Result)
            {
                try
                {
                    substitutionCipherService.SaveLanguageInfoModel(result.FileName, languageData);
                }
                catch 
                {
                    messagingService.ShowError("#Failed to save language data!");
                }
            }
        }

        private void DoOpenLanguageData()
        {
            var result = dialogService.ShowOpenDialog("#Language data files (*.langdata)|*.langdata");
            if (result.Result)
            {
                try
                {
                    LanguageData = substitutionCipherService.LoadLanguageInfoModel(result.FileName);
                }
                catch
                {
                    messagingService.ShowError("#Failed to load language data!");
                }
            }
        }

        private void DoGenerateLanguageData()
        {
            (bool result, List<string> files) = dialogService.ShowOpenFilesDialog("#Text files (*.txt)|*.txt");
            if (result)
            {
                string[] lines;
                try
                {
                    lines = files.SelectMany(file => File.ReadAllLines(file))
                        .ToArray();
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

        private int? FindAlphabetPlaintextEntry(char c)
        {
            for (int i = 0; i < alphabet.Count; i++)
                if (alphabet[i].Plaintext[0] == c)
                    return i;

            return null;
        }

        private void DoSolveFromLetterFreq()
        {
            var plain = alphabet.Select(e => e.Plaintext[0]);
            var cipher = alphabet.Where(e => !string.IsNullOrEmpty(e.Cipher)).Select(e => e.Cipher[0]);

            // If user did not enter any cipher values, assume the same alphabet as plain
            if (!cipher.Any())
                cipher = new List<char>(plain);

            var letterStats = cipherDoc.Text
                .ToLowerInvariant()
                .GroupBy(c => c)
                .Where(g => cipher.Contains(g.Key))
                .Select(g => new { Character = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToList();

            var letterFreqs = languageData.LetterFrequencies
                .OrderByDescending(lf => lf.Value)
                .ToList();

            int letterFreqIndex = 0;
            int cipherIndex = 0;

            while (letterFreqIndex < letterFreqs.Count() && cipherIndex < letterStats.Count())
            {
                // Try to find plain letter from letter freqs
                int? alphabetIndex = FindAlphabetPlaintextEntry(letterFreqs[letterFreqIndex].Key);
                if (alphabetIndex != null)
                {
                    alphabet[alphabetIndex.Value].Cipher = letterStats[cipherIndex].Character.ToString();
                    cipherIndex++;
                }

                letterFreqIndex++;
            }

            StartCipherOperation();
        }

        private void DoCrack()
        {
            Dictionary<char, char> key;
            if (alphabet != null)
            {
                key = alphabet
                    .Where(e => !String.IsNullOrEmpty(e.Cipher) && !e.IsDoubled)
                    .ToDictionary(e => e.Plaintext.ToLowerInvariant()[0], e => e.Cipher.ToLowerInvariant()[0]);

                // If user entered no ciphers in alphabet, copy plaintexts instead
                if (!key.Any())
                {
                    key = alphabet
                        .ToDictionary(e => e.Plaintext.ToLowerInvariant()[0], e => e.Plaintext.ToLowerInvariant()[0]);
                }
            }
            else
                key = new Dictionary<char, char>();

            var input = new CrackWorkerInput(key, cipherDoc.Text, languageData);

            var worker = new CrackWorker(substitutionCipherService);
            worker.RunWorkerCompleted += HandleCrackWorkerFinished;
            
            dialogService.ShowProgressDialog("#Cracking cipher...", worker, input);
        }

        private void HandleCrackWorkerFinished(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                var output = e.Result as CrackWorkerOutput;

                foreach (var kvp in output.BestKey)
                {
                    var index = FindAlphabetPlaintextEntry(kvp.Key);
                    if (index != null)
                        alphabet[index.Value].SetCipherSilently(kvp.Value.ToString());
                }

                StartCipherOperation();
            }
        }

        // IAlphabetEntryHandler implementation -------------------------------

        void IAlphabetEntryHandler.NotifyChanged(AlphabetEntryViewModel alphabetEntryViewModel, string previousCipher)
        {
            if (!string.IsNullOrEmpty(alphabetEntryViewModel.Cipher))
            {
                // If there is entry with the same cipher, replace it automatically with previous cipher.
                var entry = alphabet.FirstOrDefault(ae => ae != alphabetEntryViewModel && ae.Cipher == alphabetEntryViewModel.Cipher);
                if (entry != null)
                    entry.SetCipherSilently(previousCipher);
            }

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
            modeIsUncipher = new LambdaCondition<SubstitutionCipherWindowViewModel>(this, vm => vm.Mode == SubstitutionCipherMode.Uncipher);

            EnterAlphabetCommand = new AppCommand(obj => DoEnterAlphabet());
            SwitchModeToCipherCommand = new AppCommand(obj => DoSwitchModeToCipher());
            SwitchModeToUncipherCommand = new AppCommand(obj => DoSwitchModeToUncipher());

            GenerateLanguageDataCommand = new AppCommand(obj => DoGenerateLanguageData());
            OpenLanguageDataCommand = new AppCommand(obj => DoOpenLanguageData());
            SaveLanguageDataCommand = new AppCommand(obj => DoSaveLanguageData(), languageDataAvailableCondition);
            SolveFromLetterFreqCommand = new AppCommand(obj => DoSolveFromLetterFreq(), languageDataAvailableCondition & modeIsUncipher);
            CrackCommand = new AppCommand(obj => DoCrack(), languageDataAvailableCondition & modeIsUncipher);
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
                    .ToDictionary(e => e.Plaintext.ToLowerInvariant()[0], e => e.Cipher.ToLowerInvariant()[0]);
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

            var input = new CipherWorkerInput(key, data, forward);
            currentWorker = new CipherWorker(substitutionCipherService);
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
        public ICommand SolveFromLetterFreqCommand { get; }

        public ICommand CrackCommand { get; }
    }
}
