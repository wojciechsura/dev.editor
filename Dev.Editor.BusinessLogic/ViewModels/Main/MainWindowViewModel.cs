﻿using Dev.Editor.BusinessLogic.Models.Dialogs;
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
using Dev.Editor.BusinessLogic.Services.Commands;
using System.Windows.Media;
using System.Reflection;
using System.Windows.Media.Imaging;
using Dev.Editor.BusinessLogic.Services.ImageResources;
using Dev.Editor.BusinessLogic.Models.Navigation;
using Dev.Editor.BusinessLogic.Services.FileIcons;
using Dev.Editor.BusinessLogic.Types.UI;
using Dev.Editor.BusinessLogic.ViewModels.Tools.Explorer;
using Dev.Editor.BusinessLogic.ViewModels.Tools.Base;
using Dev.Editor.BusinessLogic.Models.UI;

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
        private readonly ICommandRepositoryService commandRepositoryService;
        private readonly IImageResources imageResources;
        private readonly IFileIconProvider fileIconProvider;

        private readonly ObservableCollection<DocumentViewModel> documents;
        private DocumentViewModel activeDocument;

        private readonly List<HighlightingInfo> highlightings;

        private bool wordWrap;
        private bool lineNumbers;

        private string navigationText;
        private readonly ObservableCollection<BaseNavigationModel> navigationItems;
        private BaseNavigationModel selectedNavigationItem;

        private SidePanelPlacement sidePanelPlacement;
        private List<SidePanelPlacementModel> sidePanelPlacements;
        private double sidePanelSize;

        // Tools

        private readonly ExplorerToolViewModel explorerToolViewModel;

        // Private methods ----------------------------------------------------

        private void HandleActiveDocumentChanged()
        {
            documentExistsCondition.Value = activeDocument != null;
        }

        private void HandleSidePanelSizeChanged()
        {
            configurationService.Configuration.UI.SidePanelSize.Value = sidePanelSize;
        }

        private void HandleSidePanelPlacementChanged()
        {
            configurationService.Configuration.UI.SidePanelPlacement.Value = sidePanelPlacement;
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

                        document.SetFilename(storedFile.Filename.Value, fileIconProvider.GetImageForFile(storedFile.Filename.Value));
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

        private bool CanCloseDocument(DocumentViewModel document)
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

        private IEnumerable<BaseToolViewModel> GetToolViewModels()
        {
            yield return explorerToolViewModel;
        }

        // IDocumentHandler implementation ------------------------------------

        void IDocumentHandler.RequestClose(DocumentViewModel documentViewModel)
        {
            if (CanCloseDocument(documentViewModel))
                RemoveDocument(documentViewModel);
        }

        // Public methods -----------------------------------------------------

        public MainWindowViewModel(IMainWindowAccess access, 
            IDialogService dialogService, 
            IMessagingService messagingService,
            IConfigurationService configurationService,
            IPathService pathService,
            IStartupInfoService startupInfoService,
            IHighlightingProvider highlightingProvider,
            ICommandRepositoryService commandRepositoryService,
            IImageResources imageResources,
            IFileIconProvider fileIconProvider)
        {
            this.access = access;
            this.dialogService = dialogService;
            this.messagingService = messagingService;
            this.configurationService = configurationService;
            this.pathService = pathService;
            this.startupInfoService = startupInfoService;
            this.highlightingProvider = highlightingProvider;
            this.commandRepositoryService = commandRepositoryService;
            this.imageResources = imageResources;
            this.fileIconProvider = fileIconProvider;

            wordWrap = configurationService.Configuration.Editor.WordWrap.Value;
            lineNumbers = configurationService.Configuration.Editor.LineNumbers.Value;
            sidePanelPlacement = configurationService.Configuration.UI.SidePanelPlacement.Value;

            sidePanelPlacements = new List<SidePanelPlacementModel>
            {
                new SidePanelPlacementModel("#Left", SidePanelPlacement.Left),
                new SidePanelPlacementModel("#Right", SidePanelPlacement.Right),
                new SidePanelPlacementModel("#Hidden", SidePanelPlacement.Hidden)
            };

            sidePanelSize = configurationService.Configuration.UI.SidePanelSize.Value;

            documents = new ObservableCollection<DocumentViewModel>();

            highlightings = new List<HighlightingInfo>(highlightingProvider.HighlightingDefinitions);
            highlightings.Sort((h1, h2) => h1.Name.CompareTo(h2.Name));

            documentExistsCondition = new Condition(activeDocument != null);

            // Initializing conditions

            canUndoCondition = new MutableSourcePropertyWatchCondition<MainWindowViewModel, DocumentViewModel>(this, vm => vm.ActiveDocument, doc => doc.CanUndo, false);
            canRedoCondition = new MutableSourcePropertyWatchCondition<MainWindowViewModel, DocumentViewModel>(this, vm => vm.ActiveDocument, doc => doc.CanRedo, false);
            selectionAvailableCondition = new MutableSourcePropertyWatchCondition<MainWindowViewModel, DocumentViewModel>(this, vm => ActiveDocument, doc => doc.SelectionAvailable, false);
            regularSelectionAvailableCondition = new MutableSourcePropertyWatchCondition<MainWindowViewModel, DocumentViewModel>(this, vm => ActiveDocument, doc => doc.RegularSelectionAvailable, false);
            searchPerformedCondition = new MutableSourcePropertyNotNullWatchCondition<MainWindowViewModel, DocumentViewModel>(this, vm => ActiveDocument, doc => doc.LastSearch);

            // Initializing tools

            explorerToolViewModel = new ExplorerToolViewModel(fileIconProvider, imageResources);

            // Initializing commands

            // This command should not be registered in command repository service
            NavigateCommand = new AppCommand(obj => DoNavigate());

            NewCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_File_New, "New16.png", obj => DoNew());
            OpenCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_File_Open, "Open16.png", obj => DoOpen());
            SaveCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_File_Save, "Save16.png", obj => DoSave(), documentExistsCondition);
            SaveAsCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_File_SaveAs, "Save16.png", obj => DoSaveAs(), documentExistsCondition);

            UndoCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Edit_Undo, "Undo16.png", obj => DoUndo(), canUndoCondition);
            RedoCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Edit_Redo, "Redo16.png", obj => DoRedo(), canRedoCondition);
            CopyCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Edit_Copy, "Copy16.png", obj => DoCopy(), selectionAvailableCondition);
            CutCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Edit_Cut, "Cut16.png", obj => DoCut(), selectionAvailableCondition);
            PasteCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Edit_Paste, "Paste16.png", obj => DoPaste(), documentExistsCondition);

            SearchCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Search_Search, "Search16.png", obj => DoSearch(), documentExistsCondition);
            ReplaceCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Search_Replace, "Replace16.png", obj => DoReplace(), documentExistsCondition);
            FindNextCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Search_FindNext, "FindNext16.png", obj => DoFindNext(), documentExistsCondition & searchPerformedCondition);

            SortLinesAscendingCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Ordering_SortAscending, "SortAscending16.png", obj => DoSortAscending(), documentExistsCondition);
            SortLinesDescendingCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Ordering_SortDescending, "SortDescending16.png", obj => DoSortDescending(), documentExistsCondition);
            RemoveEmptyLinesCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Cleanup_RemoveEmptyLines, null, obj => DoRemoveEmptyLines(), documentExistsCondition);
            RemoveWhitespaceLinesCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Cleanup_RemoveWhitespaceLines, null, obj => DoRemoveWhitespaceLines(), documentExistsCondition);

            // Navigation

            navigationText = String.Empty;
            navigationItems = new ObservableCollection<BaseNavigationModel>();

            // Applying current close behavior

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

        public void FocusActiveDocument()
        {
            activeDocument?.FocusDocument();
        }

        public void SelectPreviousNavigationItem()
        {
            if (selectedNavigationItem != null)
            {
                int index = navigationItems.IndexOf(selectedNavigationItem);
                if (index > 0)
                    SelectedNavigationItem = navigationItems[index - 1];
                access.EnsureSelectedNavigationItemVisible();
            }
        }

        public void SelectNextNavigationItem()
        {
            if (selectedNavigationItem != null)
            {
                int index = navigationItems.IndexOf(selectedNavigationItem);
                if (index < navigationItems.Count - 1)
                    SelectedNavigationItem = navigationItems[index + 1];
                access.EnsureSelectedNavigationItemVisible();
            }
        }

        // Public properties --------------------------------------------------

        public ObservableCollection<DocumentViewModel> Documents => documents;

        public DocumentViewModel ActiveDocument
        {
            get => activeDocument;
            set => Set(ref activeDocument, () => ActiveDocument, value, HandleActiveDocumentChanged);
        }

        public IReadOnlyList<HighlightingInfo> Highlightings => highlightings;

        public string NavigationText
        {
            get => navigationText;
            set
            {
                Set(ref navigationText, () => NavigationText, value);
            }
        }

        public ObservableCollection<BaseNavigationModel> NavigationItems => navigationItems;

        public BaseNavigationModel SelectedNavigationItem
        {
            get => selectedNavigationItem;
            set
            {
                Set(ref selectedNavigationItem, () => SelectedNavigationItem, value);
            }
        }

        public SidePanelPlacement SidePanelPlacement
        {
            get => sidePanelPlacement;
            set
            {
                Set(ref sidePanelPlacement, () => SidePanelPlacement, value, HandleSidePanelPlacementChanged);
            }
        }

        public IEnumerable<SidePanelPlacementModel> SidePanelPlacements => sidePanelPlacements;

        public double SidePanelSize
        {
            get => sidePanelSize;
            set
            {
                Set(ref sidePanelSize, () => SidePanelSize, value, HandleSidePanelSizeChanged);
            }
        }

        public IEnumerable<BaseToolViewModel> Tools => GetToolViewModels();
    }
}
