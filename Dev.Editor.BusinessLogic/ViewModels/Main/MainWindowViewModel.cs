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
    public partial class MainWindowViewModel : BaseViewModel, IDocumentHandler, IExplorerHandler
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

        private readonly ObservableCollection<BaseDocumentViewModel> documents;
        private BaseDocumentViewModel activeDocument;

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

        private readonly List<BaseToolViewModel> tools;
        private readonly ExplorerToolViewModel explorerToolViewModel;
        private BaseToolViewModel selectedTool;

        // Private methods ----------------------------------------------------

        private void HandleActiveDocumentChanged()
        {
            documentExistsCondition.Value = activeDocument != null;
            documentIsTextCondition.Value = activeDocument is TextDocumentViewModel;
        }

        private void HandleSidePanelSizeChanged()
        {
            configurationService.Configuration.UI.SidePanelSize.Value = sidePanelSize;
        }

        private void HandleSidePanelPlacementChanged()
        {
            configurationService.Configuration.UI.SidePanelPlacement.Value = sidePanelPlacement;
        }

        private void HandleSelcetedToolChanged()
        {
            configurationService.Configuration.UI.SidePanelActiveTab.Value = selectedTool.Uid;
        }

        private void RemoveDocument(BaseDocumentViewModel document)
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

                switch (document)
                {
                    case TextDocumentViewModel textDocument:
                        {
                            var storedFile = new TextStoredFile();
                            storedFile.Filename.Value = textDocument.FileName;
                            storedFile.FilenameIsVirtual.Value = textDocument.FilenameVirtual;
                            storedFile.IsDirty.Value = textDocument.Changed;
                            storedFile.StoredFilename.Value = storedFilename;
                            storedFile.HighlightingName.Value = textDocument.Highlighting?.Name;

                            configurationService.Configuration.Internal.StoredFiles.Add(storedFile);
                            break;
                        }
                    case HexDocumentViewModel hexDocument:
                        {
                            var storedFile = new HexStoredFile();
                            storedFile.Filename.Value = hexDocument.FileName;
                            storedFile.FilenameIsVirtual.Value = hexDocument.FilenameVirtual;
                            storedFile.IsDirty.Value = hexDocument.Changed;
                            storedFile.StoredFilename.Value = storedFilename;

                            configurationService.Configuration.Internal.StoredFiles.Add(storedFile);
                            break;
                        }
                    default:
                        throw new InvalidOperationException("Unsupported document type!");
                }                
            }

            return true;
        }

        private void RestoreFiles()
        {
            for (int i = 0; i < configurationService.Configuration.Internal.StoredFiles.Count; i++)
            {
                var file = configurationService.Configuration.Internal.StoredFiles[i];

                switch (file)
                {
                    case TextStoredFile textStoredFile:
                        {
                            try
                            {
                                InternalAddTextDocument(document =>
                                {
                                    InternalReadTextDocument(document, textStoredFile.StoredFilename.Value);

                                    document.SetFilename(textStoredFile.Filename.Value, fileIconProvider.GetImageForFile(textStoredFile.Filename.Value));
                                    document.FilenameVirtual = textStoredFile.FilenameIsVirtual.Value;

                                    if (!textStoredFile.IsDirty.Value)
                                    {
                                        document.Document.UndoStack.MarkAsOriginalFile();
                                    }
                                    else
                                    {
                                        document.Document.UndoStack.DiscardOriginalFileMarker();
                                    }
                                    document.Highlighting = highlightingProvider.GetDefinitionByName(textStoredFile.HighlightingName.Value);
                                });
                            }
                            catch (Exception e)
                            {
                                messagingService.ShowError(string.Format(Resources.Strings.Message_CannotRestoreFile, textStoredFile.Filename, e.Message));
                            }

                            break;
                        }
                    case HexStoredFile hexStoredFile:
                        {
                            try
                            {
                                InternalAddHexDocument(document =>
                                {
                                    InternalReadHexDocument(document, hexStoredFile.StoredFilename.Value);

                                    document.SetFilename(hexStoredFile.Filename.Value, fileIconProvider.GetImageForFile(hexStoredFile.Filename.Value));
                                    document.FilenameVirtual = hexStoredFile.FilenameIsVirtual.Value;
                                    document.Changed = hexStoredFile.IsDirty.Value;
                                });
                            }
                            catch (Exception e)
                            {
                                messagingService.ShowError(string.Format(Resources.Strings.Message_CannotRestoreFile, hexStoredFile.Filename, e.Message));
                            }

                            break;
                        }
                    default:
                        throw new InvalidOperationException("Unsupported document type!");
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
                        LoadTextDocument(param);
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

        private bool CanCloseDocument(BaseDocumentViewModel document)
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

        // IDocumentHandler implementation ------------------------------------

        void IDocumentHandler.RequestClose(BaseDocumentViewModel documentViewModel)
        {
            if (CanCloseDocument(documentViewModel))
                RemoveDocument(documentViewModel);
        }

        // IExplorerHandler implementation ------------------------------------

        void IExplorerHandler.OpenTextFile(string path)
        {
            LoadTextDocument(path);
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

            documents = new ObservableCollection<BaseDocumentViewModel>();

            highlightings = new List<HighlightingInfo>(highlightingProvider.HighlightingDefinitions);
            highlightings.Sort((h1, h2) => h1.Name.CompareTo(h2.Name));

            documentExistsCondition = new Condition(activeDocument != null);
            documentIsTextCondition = new Condition(activeDocument is TextDocumentViewModel);

            // Initializing conditions

            canUndoCondition = new MutableSourcePropertyWatchCondition<MainWindowViewModel, BaseDocumentViewModel>(this, vm => vm.ActiveDocument, doc => doc.CanUndo, false);
            canRedoCondition = new MutableSourcePropertyWatchCondition<MainWindowViewModel, BaseDocumentViewModel>(this, vm => vm.ActiveDocument, doc => doc.CanRedo, false);
            selectionAvailableCondition = new MutableSourcePropertyWatchCondition<MainWindowViewModel, BaseDocumentViewModel>(this, vm => ActiveDocument, doc => doc.SelectionAvailable, false);
            regularSelectionAvailableCondition = new MutableSourcePropertyWatchCondition<MainWindowViewModel, BaseDocumentViewModel>(this, vm => ActiveDocument, doc => doc.RegularSelectionAvailable, false);
            searchPerformedCondition = new MutableSourcePropertyNotNullWatchCondition<MainWindowViewModel, BaseDocumentViewModel>(this, vm => ActiveDocument, doc => doc.LastSearch);

            // Initializing tools

            tools = new List<BaseToolViewModel>();

            explorerToolViewModel = new ExplorerToolViewModel(fileIconProvider, imageResources, configurationService, this);
            tools.Add(explorerToolViewModel);

            selectedTool = tools.FirstOrDefault(t => t.Uid.Equals(configurationService.Configuration.UI.SidePanelActiveTab.Value));

            // Initializing commands

            // This command should not be registered in command repository service
            NavigateCommand = new AppCommand(obj => DoNavigate());

            NewTextCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_File_New, "New16.png", obj => DoNewTextDocument());
            NewHexCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_File_NewHex, "New16.png", obj => DoNewHexDocument());
            OpenTextCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_File_Open, "Open16.png", obj => DoOpenTextDocument());
            OpenHexCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_File_OpenHex, "Open16.png", obj => DoOpenHexDocument());
            SaveCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_File_Save, "Save16.png", obj => DoSaveDocument(), documentExistsCondition);
            SaveAsCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_File_SaveAs, "Save16.png", obj => DoSaveDocumentAs(), documentExistsCondition);

            UndoCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Edit_Undo, "Undo16.png", obj => DoUndo(), canUndoCondition);
            RedoCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Edit_Redo, "Redo16.png", obj => DoRedo(), canRedoCondition);
            CopyCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Edit_Copy, "Copy16.png", obj => DoCopy(), selectionAvailableCondition);
            CutCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Edit_Cut, "Cut16.png", obj => DoCut(), selectionAvailableCondition);
            PasteCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Edit_Paste, "Paste16.png", obj => DoPaste(), documentExistsCondition);

            SearchCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Search_Search, "Search16.png", obj => DoSearch(), documentIsTextCondition);
            ReplaceCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Search_Replace, "Replace16.png", obj => DoReplace(), documentIsTextCondition);
            FindNextCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Search_FindNext, "FindNext16.png", obj => DoFindNext(), documentIsTextCondition & searchPerformedCondition);

            SortLinesAscendingCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Ordering_SortAscending, "SortAscending16.png", obj => DoSortAscending(), documentIsTextCondition);
            SortLinesDescendingCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Ordering_SortDescending, "SortDescending16.png", obj => DoSortDescending(), documentIsTextCondition);
            RemoveEmptyLinesCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Cleanup_RemoveEmptyLines, null, obj => DoRemoveEmptyLines(), documentIsTextCondition);
            RemoveWhitespaceLinesCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Cleanup_RemoveWhitespaceLines, null, obj => DoRemoveWhitespaceLines(), documentIsTextCondition);

            // Navigation

            navigationText = String.Empty;
            navigationItems = new ObservableCollection<BaseNavigationModel>();

            // Applying current close behavior

            if (configurationService.Configuration.Behavior.CloseBehavior.Value == CloseBehavior.Fluent)
            {
                RestoreFiles();

                OpenParameters();

                if (documents.Count == 0)
                    DoNewTextDocument();
            }
            else if (configurationService.Configuration.Behavior.CloseBehavior.Value == CloseBehavior.Standard)
            {
                if (!OpenParameters())
                    DoNewTextDocument();
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

        public ObservableCollection<BaseDocumentViewModel> Documents => documents;

        public BaseDocumentViewModel ActiveDocument
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

        public IEnumerable<BaseToolViewModel> Tools => tools;

        public BaseToolViewModel SelectedTool
        {
            get => selectedTool;
            set
            {
                Set(ref selectedTool, () => SelectedTool, value, HandleSelcetedToolChanged);
            }
        }
    }
}
