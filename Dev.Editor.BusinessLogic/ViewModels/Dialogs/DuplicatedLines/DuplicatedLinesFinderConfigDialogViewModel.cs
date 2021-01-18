using Dev.Editor.BusinessLogic.Models.DuplicatedLines;
using Dev.Editor.BusinessLogic.Services.Dialogs;
using Dev.Editor.BusinessLogic.Services.Messaging;
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
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dev.Editor.BusinessLogic.ViewModels.Dialogs.DuplicatedLines
{
    public class DuplicatedLinesFinderConfigDialogViewModel : BaseViewModel
    {
        private readonly IDuplicatedLinesFinderConfigDialogAccess access;
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
            (bool result, string folder) = dialogService.ShowChooseFolderDialog(null);
            if (result)
            {
                var newEntry = new SourceEntry(Path.Combine(folder, "*.*"));
                Sources.Add(newEntry);
                SelectedEntry = newEntry;
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

        public DuplicatedLinesFinderConfigDialogViewModel(IDuplicatedLinesFinderConfigDialogAccess access,
            IDialogService dialogService, 
            IMessagingService messagingService)
        {
            this.access = access;
            this.dialogService = dialogService;
            this.messagingService = messagingService;
            minimumFiles = 2;
            minimumLines = 5;
            recursive = false;
            trimLines = true;

            entryNotNullCondition = new LambdaCondition<DuplicatedLinesFinderConfigDialogViewModel>(this,
                vm => !String.IsNullOrEmpty(vm.Entry));
            entrySelectedCondition = new LambdaCondition<DuplicatedLinesFinderConfigDialogViewModel>(this,
                vm => vm.SelectedEntry != null);

            AddEntryCommand = new AppCommand(obj => DoAddEntry(), entryNotNullCondition);
            RemoveEntryCommand = new AppCommand(obj => DoRemoveEntry(), entrySelectedCondition);
            AddFolderCommand = new AppCommand(obj => DoAddFolder());
            AddFilesCommand = new AppCommand(obj => DoAddFiles());
            OkCommand = new AppCommand(obj => DoOk());
            CancelCommand = new AppCommand(obj => DoCancel());

            Sources = new ObservableCollection<SourceEntry>();
        }

        private void DoCancel()
        {
            access.CloseDialog(null, false);
        }

        private void DoOk()
        {
            (bool result, List<string> paths) = GenerateFileList();
            if (!result)
                return;

            DuplicatedLinesFinderConfig model = new DuplicatedLinesFinderConfig
            {
                SourcePaths = paths,
                ExcludeMasks = ExcludedFileMasks,
                MinFiles = MinimumFiles,
                MinLines = MinimumLines,
                Trim = TrimLines
            };

            access.CloseDialog(model, true);
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
    }
}
