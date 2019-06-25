using Dev.Editor.BusinessLogic.Models.Dialogs;
using Dev.Editor.Resources;
using Dev.Editor.BusinessLogic.Services.Dialogs;
using Dev.Editor.BusinessLogic.Services.Messaging;
using Dev.Editor.BusinessLogic.ViewModels.Base;
using Dev.Editor.BusinessLogic.ViewModels.Document;
using Dev.Editor.BusinessLogic.ViewModels.Search;
using Dev.Editor.Common.Commands;
using Dev.Editor.Common.Conditions;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Dev.Editor.BusinessLogic.Services.Config;
using Dev.Editor.BusinessLogic.Types.Behavior;
using Dev.Editor.BusinessLogic.Models.Configuration.Internal;
using Dev.Editor.BusinessLogic.Services.Paths;
using Dev.Editor.BusinessLogic.Services.StartupInfo;
using Dev.Editor.BusinessLogic.Services.Highlighting;
using Dev.Editor.BusinessLogic.Models.Highlighting;

namespace Dev.Editor.BusinessLogic.ViewModels.Main
{
    public partial class MainWindowViewModel : BaseViewModel, IDocumentHandler
    {
        // Private fields -----------------------------------------------------

        private readonly IMainWindowAccess access;
        private readonly IDialogService dialogService;
        private readonly IMessagingService messagingService;
        private readonly IConfigurationService configurationService;
        private readonly IPathService pathService;
        private readonly IStartupInfoService startupInfoService;
        private readonly IHighlightingProvider highlightingProvider;

        private readonly ObservableCollection<DocumentViewModel> documents;
        private DocumentViewModel activeDocument;
        private readonly List<HighlightingInfo> highlightings;

        private readonly Condition documentExistsCondition;
        private readonly MutableSourcePropertyWatchCondition<MainWindowViewModel, DocumentViewModel> canUndoCondition;
        private readonly MutableSourcePropertyWatchCondition<MainWindowViewModel, DocumentViewModel> canRedoCondition;
        private readonly MutableSourcePropertyWatchCondition<MainWindowViewModel, DocumentViewModel> selectionAvailableCondition;
        private readonly MutableSourcePropertyWatchCondition<MainWindowViewModel, DocumentViewModel> regularSelectionAvailableCondition;
        private readonly MutableSourcePropertyNotNullWatchCondition<MainWindowViewModel, DocumentViewModel> searchPerformedCondition;

        private bool wordWrap;
        private bool lineNumbers;

        // Private methods ----------------------------------------------------

        private void HandleActiveDocumentChanged()
        {
            documentExistsCondition.Value = activeDocument != null;
        }

        private void RemoveDocument(DocumentViewModel document)
        {
            int index = documents.IndexOf(document);

            documents.Remove(document);
            if (ActiveDocument == document)
            {
                if (index >= documents.Count)
                    index = documents.Count - 1;

                if (index > 0 && index < documents.Count)
                    ActiveDocument = documents[index];
                else
                    ActiveDocument = null;                
            }
        }

        private bool StoreFiles()
        {
            configurationService.Configuration.Internal.StoredFiles.Clear();
            var storedFilesPath = pathService.StoredFilesPath;

            for (int i = 0; i < documents.Count; i++)
            {
                var document = documents[i];
                var storedFilename = Path.Combine(storedFilesPath, $"{i}.txt");
                
                try
                {
                    InternalWriteDocument(document, storedFilename);
                }
                catch (Exception e)
                {
                    messagingService.ShowError(String.Format(Resources.Strings.Message_CannotSaveFile, e.Message));
                    return false;
                }

                var storedFile = new StoredFile();
                storedFile.Filename.Value = document.FileName;
                storedFile.FilenameIsVirtual.Value = document.FilenameVirtual;
                storedFile.IsDirty.Value = document.Changed;
                storedFile.StoredFilename.Value = storedFilename;
                storedFile.HighlightingName.Value = document.Highlighting?.Name;

                configurationService.Configuration.Internal.StoredFiles.Add(storedFile);
            }

            return true;
        }

        private void RestoreFiles()
        {
            for (int i = 0; i < configurationService.Configuration.Internal.StoredFiles.Count; i++)
            {
                var storedFile = configurationService.Configuration.Internal.StoredFiles[i];

                try
                {
                    InternalAddDocument(document =>
                    {
                        InternalReadDocument(document, storedFile.StoredFilename.Value);

                        document.FileName = storedFile.Filename.Value;
                        document.FilenameVirtual = storedFile.FilenameIsVirtual.Value;

                        if (!storedFile.IsDirty.Value)
                        {
                            document.Document.UndoStack.MarkAsOriginalFile();
                        }
                        else
                        {
                            document.Document.UndoStack.DiscardOriginalFileMarker();
                        }
                        document.Highlighting = highlightingProvider.GetDefinitionByName(storedFile.HighlightingName.Value);
                    });
                }
                catch (Exception e)
                {
                    messagingService.ShowError(string.Format(Resources.Strings.Message_CannotRestoreFile, storedFile.Filename, e.Message));
                }
            }
        }

        private bool OpenParameters()
        {
            bool anyDocumentLoaded = false;

            for (int i = 0; i < startupInfoService.Parameters.Length; i++)
            {
                string param = startupInfoService.Parameters[i];

                if (File.Exists(param))
                {
                    try
                    {
                        LoadDocument(param);
                        anyDocumentLoaded = true;
                    }
                    catch (Exception e)
                    {
                        messagingService.ShowError(string.Format(Strings.Message_CannotOpenFile, param, e.Message));
                    }
                }
            }

            return anyDocumentLoaded;
        }

        // Public methods -----------------------------------------------------

        public MainWindowViewModel(IMainWindowAccess access, 
            IDialogService dialogService, 
            IMessagingService messagingService,
            IConfigurationService configurationService,
            IPathService pathService,
            IStartupInfoService startupInfoService,
            IHighlightingProvider highlightingProvider)
        {
            this.access = access;
            this.dialogService = dialogService;
            this.messagingService = messagingService;
            this.configurationService = configurationService;
            this.pathService = pathService;
            this.startupInfoService = startupInfoService;
            this.highlightingProvider = highlightingProvider;

            wordWrap = configurationService.Configuration.Editor.WordWrap.Value;
            lineNumbers = configurationService.Configuration.Editor.LineNumbers.Value;

            documents = new ObservableCollection<DocumentViewModel>();

            highlightings = new List<HighlightingInfo>(highlightingProvider.HighlightingDefinitions);
            highlightings.Sort((h1, h2) => h1.Name.CompareTo(h2.Name));

            documentExistsCondition = new Condition(activeDocument != null);

            canUndoCondition = new MutableSourcePropertyWatchCondition<MainWindowViewModel, DocumentViewModel>(this, vm => vm.ActiveDocument, doc => doc.CanUndo, false);
            canRedoCondition = new MutableSourcePropertyWatchCondition<MainWindowViewModel, DocumentViewModel>(this, vm => vm.ActiveDocument, doc => doc.CanRedo, false);
            selectionAvailableCondition = new MutableSourcePropertyWatchCondition<MainWindowViewModel, DocumentViewModel>(this, vm => ActiveDocument, doc => doc.SelectionAvailable, false);
            regularSelectionAvailableCondition = new MutableSourcePropertyWatchCondition<MainWindowViewModel, DocumentViewModel>(this, vm => ActiveDocument, doc => doc.RegularSelectionAvailable, false);
            searchPerformedCondition = new MutableSourcePropertyNotNullWatchCondition<MainWindowViewModel, DocumentViewModel>(this, vm => ActiveDocument, doc => doc.LastSearch);

            NewCommand = new AppCommand(obj => DoNew());
            OpenCommand = new AppCommand(obj => DoOpen());
            SaveCommand = new AppCommand(obj => DoSave(), documentExistsCondition);
            SaveAsCommand = new AppCommand(obj => DoSaveAs(), documentExistsCondition);

            UndoCommand = new AppCommand(obj => DoUndo(), canUndoCondition);
            RedoCommand = new AppCommand(obj => DoRedo(), canRedoCondition);
            CopyCommand = new AppCommand(obj => DoCopy(), selectionAvailableCondition);
            CutCommand = new AppCommand(obj => DoCut(), selectionAvailableCondition);
            PasteCommand = new AppCommand(obj => DoPaste(), documentExistsCondition);

            SearchCommand = new AppCommand(obj => DoSearch(), documentExistsCondition);
            ReplaceCommand = new AppCommand(obj => DoReplace(), documentExistsCondition);
            FindNextCommand = new AppCommand(obj => DoFindNext(), documentExistsCondition & searchPerformedCondition);

            if (configurationService.Configuration.Behavior.CloseBehavior.Value == CloseBehavior.Fluent)
            {
                RestoreFiles();

                OpenParameters();

                if (documents.Count == 0)
                    DoNew();
            }
            else if (configurationService.Configuration.Behavior.CloseBehavior.Value == CloseBehavior.Standard)
            {
                if (!OpenParameters())
                    DoNew();
            }
            else
                throw new InvalidOperationException("Invalid close behavior!");
        }

        public bool CanCloseDocument(DocumentViewModel document)
        {
            if (!document.Changed)
            {
                return true;
            }
            else
            {
                var decision = messagingService.AskYesNoCancel(String.Format(Strings.Message_FileNotSaved, document.FileName));

                if (decision == false)
                {
                    return true;
                }
                else if (decision == true)
                {
                    if (!document.FilenameVirtual)
                    {
                        if (SaveDocument(document))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (SaveDocumentAs(document))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public bool CanCloseApplication()
        {
            switch (configurationService.Configuration.Behavior.CloseBehavior.Value)
            {
                case CloseBehavior.Standard:
                    {
                        while (documents.Count > 0)
                        {
                            ActiveDocument = documents[0];
                            if (CanCloseDocument(documents[0]))
                            {
                                RemoveDocument(documents[0]);
                            }
                            else
                            {
                                return false;
                            }
                        }

                        break;
                    }
                case CloseBehavior.Fluent:
                    {
                        StoreFiles();

                        break;
                    }
                default:
                    throw new InvalidOperationException("Unsupported close behavior!");
            }

            var configSaved = configurationService.Save();

            if (!configSaved)
            {
                if (!messagingService.WarnYesNo(Resources.Strings.Message_CannotSaveConfiguration))
                    return false;
            }

            return true;
        }

        public void NotifyClosedDocument(DocumentViewModel document)
        {
            RemoveDocument(document);
        }

        // Public properties --------------------------------------------------

        public ObservableCollection<DocumentViewModel> Documents => documents;

        public DocumentViewModel ActiveDocument
        {
            get => activeDocument;
            set => Set(ref activeDocument, () => ActiveDocument, value, HandleActiveDocumentChanged);
        }

        public IReadOnlyList<HighlightingInfo> Highlightings => highlightings;
    }
}
