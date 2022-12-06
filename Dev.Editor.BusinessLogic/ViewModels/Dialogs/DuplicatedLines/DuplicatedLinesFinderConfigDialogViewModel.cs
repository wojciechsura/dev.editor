using Dev.Editor.BusinessLogic.Models.DuplicatedLines;
using Dev.Editor.BusinessLogic.Services.Dialogs;
using Dev.Editor.BusinessLogic.Services.Messaging;
using Dev.Editor.BusinessLogic.Types.DuplicatedLines;
using Dev.Editor.BusinessLogic.ViewModels.Base;
using Dev.Editor.Resources;
using Spooksoft.VisualStateManager.Commands;
using Spooksoft.VisualStateManager.Conditions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dev.Editor.BusinessLogic.ViewModels.Dialogs.DuplicatedLines
{
    public class DuplicatedLinesFinderConfigDialogViewModel : BaseViewModel
    {
        private readonly IDialogService dialogService;
        private readonly IMessagingService messagingService;
        private string entry;
        private SourceEntry selectedEntry;
        private int minimumLines;
        private int minimumFiles;
        private bool trimLines;
        private string excludedFileMasks;
        private bool recursive;
        private readonly BaseCondition entryNotNullCondition;
        private readonly BaseCondition entrySelectedCondition;
        private DuplicatedLinesResultSortKind resultSortKind;
        private string lineExclusionRegex;
        private bool mergeCommonResults;
        private int allowedDifferentLines;

        private void DoReset()
        {
            Sources.Clear();

            MinimumFiles = 2;
            MinimumLines = 5;
            TrimLines = true;
            LineExclusionRegex = String.Empty;
            Recursive = false;
            ExcludedFileMasks = String.Empty;
            ResultSortKind = DuplicatedLinesResultSortKind.FirstByLinesThenByFiles;
            MergeCommonResults = false;
            AllowedDifferentLines = 2;
        }

        private void DoAddEntry()
        {
            var newEntry = new SourceEntry(entry);
            Sources.Add(newEntry);
            SelectedEntry = newEntry;

            Entry = String.Empty;
        }

        private void DoRemoveEntry()
        {
            int index = Sources.IndexOf(selectedEntry);
            Sources.Remove(selectedEntry);
            if (Sources.Count > 0)
                SelectedEntry = Sources[Math.Min(0, Math.Max(Sources.Count - 1, index))];
            else
                SelectedEntry = null;
        }

        public void NotifyEntryDoubleClicked()
        {
            if (selectedEntry != null)
            {
                Entry = selectedEntry.Path;
                Sources.Remove(selectedEntry);
                SelectedEntry = null;
            }
        }

        private void DoAddFolder()
        {
            (bool result, string folder) = dialogService.ShowChooseFolderDialog(DefaultFolder);
            if (result)
            {
                var newEntry = new SourceEntry(Path.Combine(folder, "*.*"));
                Sources.Add(newEntry);
                SelectedEntry = newEntry;

                DefaultFolder = folder;
            }
        }

        private void DoAddFiles()
        {
            (bool result, List<string> files) = dialogService.ShowOpenFilesDialog();
            if (result)
            {
                SourceEntry newEntry = null;
                foreach (var file in files)
                {
                    newEntry = new SourceEntry(file);
                    Sources.Add(newEntry);
                }

                if (newEntry != null)
                    SelectedEntry = newEntry;
            }
        }

        public DuplicatedLinesFinderConfigDialogViewModel(IDialogService dialogService, 
            IMessagingService messagingService)
        {
            this.dialogService = dialogService;
            this.messagingService = messagingService;

            Sources = new ObservableCollection<SourceEntry>();

            DoReset();

            entryNotNullCondition = new ChainedLambdaCondition<DuplicatedLinesFinderConfigDialogViewModel>(this, vm => !String.IsNullOrEmpty(vm.Entry), false);
            entrySelectedCondition = new LambdaCondition<DuplicatedLinesFinderConfigDialogViewModel>(this, vm => vm.SelectedEntry != null, false);

            AddEntryCommand = new AppCommand(obj => DoAddEntry(), entryNotNullCondition);
            RemoveEntryCommand = new AppCommand(obj => DoRemoveEntry(), entrySelectedCondition);
            AddFolderCommand = new AppCommand(obj => DoAddFolder());
            AddFilesCommand = new AppCommand(obj => DoAddFiles());
            OkCommand = new AppCommand(obj => DoOk());
            CancelCommand = new AppCommand(obj => DoCancel());
            ResetCommand = new AppCommand(obj => DoReset());
        }

        private void DoCancel()
        {
            if (Access == null)
                throw new InvalidOperationException("ViewModel cannot work properly without access reference!");

            Access.CloseDialog(null, false);
        }

        private void DoOk()
        {
            if (Access == null)
                throw new InvalidOperationException("ViewModel cannot work properly without access!");

            (bool result, List<string> paths) = GenerateFileList();
            if (!result)
                return;

            Regex lineRegex = null;
            try
            {
                if (!string.IsNullOrEmpty(lineExclusionRegex))
                    lineRegex = new Regex(lineExclusionRegex);
            }
            catch
            {
                messagingService.ShowError(Strings.Message_InvalidLineRegularExpression);
                return;

            }

            DuplicatedLinesFinderConfig model = new DuplicatedLinesFinderConfig
            {
                SourcePaths = paths,
                ExcludeMasks = ExcludedFileMasks,
                MinFiles = MinimumFiles,
                MinLines = MinimumLines,
                Trim = TrimLines,
                ResultSortKind = resultSortKind,
                LineExclusionRegex = lineRegex,
                MergeCommonResults = mergeCommonResults,
                AllowedDifferentLines = allowedDifferentLines
            };

            Access.CloseDialog(model, true);
        }

        private (bool result, List<string> paths) GenerateFileList()
        {
            List<string> result = new List<string>();

            foreach (var path in Sources.Select(s => s.Path))
            {
                var pathPart = Path.GetDirectoryName(path);
                var filePart = Path.GetFileName(path);

                if (String.IsNullOrEmpty(pathPart))
                {
                    messagingService.ShowError(String.Format(Strings.Message_DirectoryDoesNotExistsForPath, path));
                    return (false, null);
                }

                if (pathPart.Contains("*") || pathPart.Contains("?"))
                {
                    messagingService.ShowError(String.Format(Strings.Message_MaskNotAllowedInDirectoryForPath, path));
                    return (false, null);
                }

                if (!Directory.Exists(pathPart))
                {
                    messagingService.ShowError(String.Format(Strings.Message_DirectoryDoesNotExistsForPath, path));
                    return (false, null);
                }

                try
                {
                    IEnumerable<string> files;
                    if (recursive)
                        files = Directory.EnumerateFiles(pathPart, filePart, SearchOption.AllDirectories);
                    else
                        files = Directory.EnumerateFiles(pathPart, filePart);

                    foreach (var file in files)
                        result.Add(file);
                }
                catch (Exception e)
                {
                    messagingService.ShowError(String.Format(Strings.Message_FailedToFindFiles, path, e.Message));
                    return (false, null);
                }
            }

            return (true, result);
        }

        public IDuplicatedLinesFinderConfigDialogAccess Access { get; set; }

        public string DefaultFolder { get; set; }

        public string Entry
        {
            get => entry;
            set => Set(ref entry, () => Entry, value);
        }

        public ICommand AddEntryCommand { get; }
        public ICommand RemoveEntryCommand { get; }
        public ICommand AddFolderCommand { get; }
        public ICommand AddFilesCommand { get; }
        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand ResetCommand { get; }

        public ObservableCollection<SourceEntry> Sources { get; }

        public SourceEntry SelectedEntry
        {
            get => selectedEntry;
            set => Set(ref selectedEntry, () => SelectedEntry, value);
        }

        public int MinimumLines
        {
            get => minimumLines;
            set => Set(ref minimumLines, () => MinimumLines, value);
        }

        public int MinimumFiles
        {
            get => minimumFiles;
            set => Set(ref minimumFiles, () => MinimumFiles, value);
        }

        public bool TrimLines
        {
            get => trimLines;
            set => Set(ref trimLines, () => TrimLines, value);
        }

        public string LineExclusionRegex
        {
            get => lineExclusionRegex;
            set => Set(ref lineExclusionRegex, () => LineExclusionRegex, value);
        }

        public bool Recursive
        {
            get => recursive;
            set => Set(ref recursive, () => Recursive, value);
        }

        public string ExcludedFileMasks
        {
            get => excludedFileMasks;
            set => Set(ref excludedFileMasks, () => ExcludedFileMasks, value);
        }

        public DuplicatedLinesResultSortKind ResultSortKind
        {
            get => resultSortKind; 
            set => Set(ref resultSortKind, () => ResultSortKind, value);
        }

        public bool MergeCommonResults
        {
            get => mergeCommonResults;
            set => Set(ref mergeCommonResults, () => MergeCommonResults, value);
        }

        public int AllowedDifferentLines
        {
            get => allowedDifferentLines;
            set => Set(ref allowedDifferentLines, () => AllowedDifferentLines, value);
        }
    }
}
