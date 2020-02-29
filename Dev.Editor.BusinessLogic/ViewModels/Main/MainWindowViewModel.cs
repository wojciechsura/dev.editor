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
using Dev.Editor.BusinessLogic.ViewModels.Tools.BinDefinitions;
using Dev.Editor.Common.Tools;
using System.Windows.Data;
using Dev.Editor.BusinessLogic.Models.Configuration.BinDefinitions;
using Dev.Editor.BusinessLogic.ViewModels.BottomTools.Base;
using Dev.Editor.BusinessLogic.ViewModels.BottomTools.Messages;
using Dev.Editor.BusinessLogic.Services.EventBus;
using Dev.Editor.BusinessLogic.Models.Events;
using Dev.Editor.BusinessLogic.Models.Configuration.Search;
using Dev.Editor.BusinessLogic.Models.Search;
using Dev.Editor.BusinessLogic.Services.SearchEncoder;
using Dev.Editor.BusinessLogic.Services.Platform;
using Dev.Editor.BusinessLogic.Types.Main;

namespace Dev.Editor.BusinessLogic.ViewModels.Main
{
    public partial class MainWindowViewModel : BaseViewModel, IDocumentHandler,
        IExplorerHandler, IBinDefinitionsHandler, IMessagesHandler, IEventListener<StoredSearchesChangedEvent>
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
        private readonly IEventBus eventBus;
        private readonly ISearchEncoderService searchEncoder;
        private readonly IPlatformService platformService;

        private BaseDocumentViewModel activeDocument;

        private bool showSecondaryDocumentTab;
        private readonly ObservableCollection<BaseDocumentViewModel> primaryDocuments;
        private readonly ObservableCollection<BaseDocumentViewModel> secondaryDocuments;
        private BaseDocumentViewModel selectedPrimaryDocument;
        private BaseDocumentViewModel selectedSecondaryDocument;
        private DocumentTabKind activeDocumentTab;

        private readonly ObservableCollection<StoredSearchReplaceViewModel> storedSearches;
        private readonly ObservableCollection<StoredSearchReplaceViewModel> storedReplaces;

        private readonly List<HighlightingInfo> highlightings;

        private bool wordWrap;
        private bool lineNumbers;

        private string navigationText;
        private readonly ObservableCollection<BaseNavigationModel> navigationItems;
        private BaseNavigationModel selectedNavigationItem;

        private SidePanelPlacement sidePanelPlacement;
        private List<SidePanelPlacementModel> sidePanelPlacements;
        private double sidePanelSize;

        private BottomPanelVisibility bottomPanelVisibility;
        private List<BottomPanelVisibilityModel> bottomPanelVisibilities;
        private double bottomPanelSize;

        // Tools

        private readonly ExplorerToolViewModel explorerToolViewModel;
        private readonly BinDefinitionsToolViewModel binDefinitionsToolViewModel;

        private readonly MessagesBottomToolViewModel messagesBottomToolViewModel;

        // Private methods ----------------------------------------------------

        private void UpdateActiveDocument()
        {
            ActiveDocument = activeDocumentTab == DocumentTabKind.Primary ? selectedPrimaryDocument : selectedSecondaryDocument;
        }

        private void HandleActiveDocumentChanged()
        {
            documentExistsCondition.Value = ActiveDocument != null;
            documentIsTextCondition.Value = ActiveDocument is TextDocumentViewModel;
        }

        private void HandleSidePanelSizeChanged()
        {
            configurationService.Configuration.UI.SidePanelSize.Value = sidePanelSize;
        }

        private void HandleSidePanelPlacementChanged()
        {
            configurationService.Configuration.UI.SidePanelPlacement.Value = sidePanelPlacement;
        }

        private void HandleBottomPanelSizeChanged()
        {
            configurationService.Configuration.UI.BottomPanelSize.Value = bottomPanelSize;
        }

        private void HandleBottomPanelVisibilityChanged()
        {
            configurationService.Configuration.UI.BottomPanelVisibility.Value = bottomPanelVisibility;
        }

        private void HandleActiveDocumentTabChanged()
        {
            UpdateActiveDocument();
        }

        private void HandleSelectedPrimaryDocumentChanged()
        {
            if (ActiveDocumentTab == DocumentTabKind.Primary)
            {
                ActiveDocument = selectedPrimaryDocument;
            }
        }

        private void HandleSelectedSecondaryDocumentChanged()
        {
            if (ActiveDocumentTab == DocumentTabKind.Secondary)
            {
                ActiveDocument = selectedSecondaryDocument;
            }
        }

        private void HandleShowSecondaryDocumentTabChanged()
        {
            if (!showSecondaryDocumentTab)
            {
                // Moving documents back to main tab
                while (secondaryDocuments.Count > 0)
                {
                    var doc = secondaryDocuments[secondaryDocuments.Count - 1];
                    secondaryDocuments.RemoveAt(secondaryDocuments.Count - 1);
                    primaryDocuments.Add(doc);
                }

                ActiveDocument = SelectedPrimaryDocument;
            }
        }

        private void RemoveDocument(BaseDocumentViewModel document)
        {
            if (primaryDocuments.Contains(document))
                RemovePrimaryDocument(document);
            else if (secondaryDocuments.Contains(document))
                RemoveSecondaryDocument(document);
            else
                throw new ArgumentException(nameof(document));                
        }

        private void RemovePrimaryDocument(BaseDocumentViewModel document)
        {
            int index = primaryDocuments.IndexOf(document);

            primaryDocuments.Remove(document);
            if (SelectedPrimaryDocument == document)
            {
                if (index >= primaryDocuments.Count)
                    index = primaryDocuments.Count - 1;

                if (index > 0 && index < primaryDocuments.Count)
                    SelectedPrimaryDocument = primaryDocuments[index];
                else
                    SelectedPrimaryDocument = null;
            }
        }

        private void RemoveSecondaryDocument(BaseDocumentViewModel document)
        {
            int index = secondaryDocuments.IndexOf(document);

            secondaryDocuments.Remove(document);
            if (SelectedSecondaryDocument == document)
            {
                if (index >= secondaryDocuments.Count)
                    index = secondaryDocuments.Count - 1;

                if (index > 0 && index < secondaryDocuments.Count)
                    SelectedSecondaryDocument = secondaryDocuments[index];
                else
                    SelectedSecondaryDocument = null;
            }
        }

        private void SetActiveDocument(BaseDocumentViewModel value)
        {
            if (activeDocument != null)
            {
                activeDocument.IsActive = false;
            }

            activeDocument = value;

            if (activeDocument != null)
            {
                if (primaryDocuments.Contains(activeDocument))
                {
                    // Activate primary document tab
                    SelectedPrimaryDocument = activeDocument;
                    ActiveDocumentTab = DocumentTabKind.Primary;
                }
                else if (secondaryDocuments.Contains(activeDocument))
                {
                    SelectedSecondaryDocument = activeDocument;
                    ActiveDocumentTab = DocumentTabKind.Secondary;
                }
                else
                    throw new ArgumentException(nameof(value));

                activeDocument.IsActive = true;
            }

            HandleActiveDocumentChanged();
            OnPropertyChanged(() => ActiveDocument);
        }

        private bool StoreFiles()
        {
            int storedFileIndex = 0;

            bool StoreDocuments(IList<BaseDocumentViewModel> documents, DocumentTabKind documentTabKind)
            {
                var storedFilesPath = pathService.StoredFilesPath;

                for (int i = 0; i < documents.Count; i++)
                {
                    var document = documents[i];
                    var storedFilename = Path.Combine(storedFilesPath, $"{storedFileIndex++}.txt");

                    try
                    {
                        if (document.CanSave)
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
                                storedFile.LastModifiedDate.Value = textDocument.LastModificationDate.Ticks;
                                storedFile.DocumentTabKind.Value = documentTabKind;

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
                                storedFile.LastModifiedDate.Value = hexDocument.LastModificationDate.Ticks;
                                storedFile.DocumentTabKind.Value = documentTabKind;

                                configurationService.Configuration.Internal.StoredFiles.Add(storedFile);
                                break;
                            }
                        case BinDocumentViewModel binDocument:
                            {
                                var storedFile = new BinStoredFile();
                                storedFile.Filename.Value = binDocument.FileName;
                                storedFile.FilenameIsVirtual.Value = binDocument.FilenameVirtual;
                                storedFile.IsDirty.Value = binDocument.Changed;
                                storedFile.StoredFilename.Value = document.FileName;
                                storedFile.DefinitionUid.Value = binDocument.Definition.Uid.Value;
                                storedFile.LastModifiedDate.Value = binDocument.LastModificationDate.Ticks;
                                storedFile.DocumentTabKind.Value = documentTabKind;

                                configurationService.Configuration.Internal.StoredFiles.Add(storedFile);
                                break;
                            }
                        default:
                            throw new InvalidOperationException("Unsupported document type!");
                    }
                }

                return true;
            }

            configurationService.Configuration.Internal.StoredFiles.Clear();

            return StoreDocuments(primaryDocuments, DocumentTabKind.Primary) && StoreDocuments(secondaryDocuments, DocumentTabKind.Secondary);
        }

        private void RestoreFiles()
        {
            for (int i = 0; i < configurationService.Configuration.Internal.StoredFiles.Count; i++)
            {
                var file = configurationService.Configuration.Internal.StoredFiles[i];

                BaseDocumentViewModel document = null;

                switch (file)
                {
                    case TextStoredFile textStoredFile:
                        {
                            try
                            {
                                var textDocument = new TextDocumentViewModel(this);

                                InternalReadTextDocument(textDocument, textStoredFile.StoredFilename.Value);

                                textDocument.SetFilename(textStoredFile.Filename.Value, fileIconProvider.GetImageForFile(textStoredFile.Filename.Value));
                                textDocument.FilenameVirtual = textStoredFile.FilenameIsVirtual.Value;
                                textDocument.LastModificationDate = new DateTime(textStoredFile.LastModifiedDate.Value, DateTimeKind.Utc);

                                if (!textStoredFile.IsDirty.Value)
                                {
                                    textDocument.Document.UndoStack.MarkAsOriginalFile();
                                }
                                else
                                {
                                    textDocument.Document.UndoStack.DiscardOriginalFileMarker();
                                }
                                textDocument.Highlighting = highlightingProvider.GetDefinitionByName(textStoredFile.HighlightingName.Value);

                                document = textDocument;
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
                                var hexDocument = new HexDocumentViewModel(this);

                                InternalReadHexDocument((HexDocumentViewModel)hexDocument, hexStoredFile.StoredFilename.Value);

                                hexDocument.SetFilename(hexStoredFile.Filename.Value, fileIconProvider.GetImageForFile(hexStoredFile.Filename.Value));
                                hexDocument.FilenameVirtual = hexStoredFile.FilenameIsVirtual.Value;
                                hexDocument.Changed = hexStoredFile.IsDirty.Value;
                                hexDocument.LastModificationDate = new DateTime(hexStoredFile.LastModifiedDate.Value, DateTimeKind.Utc);

                                document = hexDocument;
                            }
                            catch (Exception e)
                            {                                
                                messagingService.ShowError(string.Format(Resources.Strings.Message_CannotRestoreFile, hexStoredFile.Filename, e.Message));
                            }

                            break;
                        }
                    case BinStoredFile binStoredFile:
                        {
                            try
                            {
                                var def = configurationService.Configuration.BinDefinitions.FirstOrDefault(bd => bd.Uid.Value == binStoredFile.DefinitionUid.Value);
                                if (def == null)
                                    continue;

                                var binDocument = new BinDocumentViewModel(this);

                                InternalReadBinDocument((BinDocumentViewModel)binDocument, binStoredFile.StoredFilename.Value, def);

                                binDocument.SetFilename(binStoredFile.Filename.Value, fileIconProvider.GetImageForFile(binStoredFile.Filename.Value));
                                binDocument.FilenameVirtual = binStoredFile.FilenameIsVirtual.Value;
                                binDocument.Changed = binStoredFile.IsDirty.Value;
                                binDocument.LastModificationDate = new DateTime(binStoredFile.LastModifiedDate.Value, DateTimeKind.Utc);

                                document = binDocument;
                            }
                            catch (Exception e)
                            {
                                messagingService.ShowError(string.Format(Resources.Strings.Message_CannotRestoreFile, binStoredFile.Filename, e.Message));
                            }

                            break;
                        }
                    default:
                        throw new InvalidOperationException("Unsupported document type!");
                }

                if (document != null)
                {
                    switch (file.DocumentTabKind.Value)
                    {
                        case DocumentTabKind.Secondary:
                            secondaryDocuments.Add(document);
                            break;
                        case DocumentTabKind.Primary:
                        default:
                            primaryDocuments.Add(document);
                            break;
                    }
                }
            }
        }

        private bool DoOpenParameters(IEnumerable<string> args)
        {
            bool anyDocumentLoaded = false;

            foreach (var param in args)
            {
                if (File.Exists(param))
                {
                    try
                    {
                        LoadTextDocument(CurrentDocuments, param);
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

        private bool OpenParameters()
        {
            return DoOpenParameters(startupInfoService.Parameters);
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

        private void DoOpenConfiguration()
        {
            dialogService.ShowConfigurationDialog();
        }

        private void DoOpenFileAndFocus(string filename, int line, int column)
        {
            LoadTextDocument(CurrentDocuments, filename);


            TextDocumentViewModel textDoc = ActiveDocument as TextDocumentViewModel;
            var offset = textDoc.Document.GetOffset(line + 1, column);
            textDoc.SetSelection(offset, 0, true);

            textDoc.FocusDocument();
        }

        private void BuildStoredSearchReplaceViewModels()
        {
            storedSearches.Clear();
            configurationService.Configuration.SearchConfig.StoredSearchReplaces
                .Where(sr => sr.Operation.Value == Types.Search.SearchReplaceOperation.Search)
                .OrderBy(sr => sr.SearchName.Value)
                .Select(sr => new StoredSearchReplaceViewModel(sr, RunStoredSearchCommand))
                .ToList()
                .ForEach(srvm => storedSearches.Add(srvm));

            storedReplaces.Clear();
            configurationService.Configuration.SearchConfig.StoredSearchReplaces
                .Where(sr => sr.Operation.Value == Types.Search.SearchReplaceOperation.Replace)
                .OrderBy(sr => sr.SearchName.Value)
                .Select(sr => new StoredSearchReplaceViewModel(sr, RunStoredSearchCommand))
                .ToList()
                .ForEach(srvm => storedReplaces.Add(srvm));
        }

        private void DoToggleLineNumbers()
        {
            LineNumbers = !LineNumbers;
        }

        private void DoToggleWordWrap()
        {
            WordWrap = !WordWrap;
        }

        private void DoSetHighlighting(HighlightingInfo highlighting)
        {
            activeDocument.Highlighting = highlighting;
        }

        private void DoRunStoredSearch(StoredSearchReplace storedSearchReplace)
        {
            var desc = new SearchReplaceDescription(storedSearchReplace.Search.Value,
                storedSearchReplace.Replace.Value,
                storedSearchReplace.Operation.Value,
                storedSearchReplace.SearchMode.Value,
                storedSearchReplace.IsCaseSensitive.Value,
                (selectionAvailableCondition.GetValue() && regularSelectionAvailableCondition.GetValue()),
                storedSearchReplace.IsSearchBackwards.Value,
                storedSearchReplace.IsWholeWordsOnly.Value,
                storedSearchReplace.ShowReplaceSummary.Value);

            var searchModel = searchEncoder.SearchDescriptionToModel(desc);

            switch (storedSearchReplace.Operation.Value)
            {
                case Types.Search.SearchReplaceOperation.Search:
                    FindNext(searchModel);
                    break;
                case Types.Search.SearchReplaceOperation.Replace:
                    ReplaceAll(searchModel);
                    break;
                default:
                    throw new InvalidEnumArgumentException("Unsupported search/replace operation!");
            }
        }

        // Private properties -------------------------------------------------

        private IEnumerable<BaseDocumentViewModel> AllDocuments => primaryDocuments.Union(secondaryDocuments);

        private IList<BaseDocumentViewModel> CurrentDocuments => activeDocumentTab == DocumentTabKind.Primary ? primaryDocuments : secondaryDocuments;

        // IDocumentHandler implementation ------------------------------------

        void IDocumentHandler.RequestClose(BaseDocumentViewModel documentViewModel)
        {
            if (CanCloseDocument(documentViewModel))
                RemoveDocument(documentViewModel);
        }

        void IDocumentHandler.ChildActivated(BaseDocumentViewModel documentViewModel)
        {
            if (activeDocument != documentViewModel)
                SetActiveDocument(activeDocument);
        }

        ICommand IDocumentHandler.CopyCommand => CopyCommand;

        ICommand IDocumentHandler.CutCommand => CutCommand;

        ICommand IDocumentHandler.PasteCommand => PasteCommand;

        // IToolHandler implementation ----------------------------------------

        void IToolHandler.CloseTools()
        {
            SidePanelPlacement = SidePanelPlacement.Hidden;
        }

        // IExplorerHandler implementation ------------------------------------

        void IExplorerHandler.OpenTextFile(string path)
        {
            LoadTextDocument(CurrentDocuments, path);
        }

        string IExplorerHandler.GetCurrentDocumentPath()
        {
            return activeDocument.FileName;
        }

        BaseCondition IExplorerHandler.CurrentDocumentHasPathCondition => documentHasPathCondition;

        // IBinDefinitionsHandler implementation ------------------------------

        void IBinDefinitionsHandler.OpenTextFile(string path)
        {
            LoadTextDocument(CurrentDocuments, path);
        }

        void IBinDefinitionsHandler.NewTextDocument(string text)
        {
            DoNewTextDocument(CurrentDocuments);

            (activeDocument as TextDocumentViewModel).Document.Insert(0, text);
        }

        void IBinDefinitionsHandler.RequestOpenBinFile(BinDefinition binDefinition)
        {
            DoOpenBinDocument(CurrentDocuments, binDefinition);
        }

        // IBottomToolsHandler implementation ---------------------------------

        void IBottomToolHandler.CloseBottomTools()
        {
            BottomPanelVisibility = BottomPanelVisibility.Hidden;
        }

        // IMessagesHandler implementation ------------------------------------

        void IMessagesHandler.OpenFileAndFocus(string filename)
        {
            LoadTextDocument(CurrentDocuments, filename);
        }

        // IEventListener<StoredSearchesChangedEvent> implementation ----------

        void IEventListener<StoredSearchesChangedEvent>.Receive(StoredSearchesChangedEvent @event)
        {
            BuildStoredSearchReplaceViewModels();
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
            IFileIconProvider fileIconProvider,
            ISearchEncoderService searchEncoder,
            IEventBus eventBus,
            IPlatformService platformService)
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
            this.searchEncoder = searchEncoder;
            this.eventBus = eventBus;

            wordWrap = configurationService.Configuration.Editor.WordWrap.Value;
            lineNumbers = configurationService.Configuration.Editor.LineNumbers.Value;

            // Side panel

            sidePanelPlacement = configurationService.Configuration.UI.SidePanelPlacement.Value;

            sidePanelPlacements = new List<SidePanelPlacementModel>
            {
                new SidePanelPlacementModel(Strings.Ribbon_View_SidePanelPlacement_Left, SidePanelPlacement.Left),
                new SidePanelPlacementModel(Strings.Ribbon_View_SidePanelPlacement_Right, SidePanelPlacement.Right),
                new SidePanelPlacementModel(Strings.Ribbon_View_SidePanelPlacement_Hidden, SidePanelPlacement.Hidden)
            };

            sidePanelSize = configurationService.Configuration.UI.SidePanelSize.Value;

            // BottomPanel

            bottomPanelVisibility = configurationService.Configuration.UI.BottomPanelVisibility.Value;

            bottomPanelVisibilities = new List<BottomPanelVisibilityModel>
            {
                new BottomPanelVisibilityModel(Strings.Ribbon_View_BottomPanelVisibility_Visible, BottomPanelVisibility.Visible),
                new BottomPanelVisibilityModel(Strings.Ribbon_View_BottomPanelVisibility_Hidden, BottomPanelVisibility.Hidden)
            };

            bottomPanelSize = configurationService.Configuration.UI.BottomPanelSize.Value;

            primaryDocuments = new ObservableCollection<BaseDocumentViewModel>();
            secondaryDocuments = new ObservableCollection<BaseDocumentViewModel>();
            activeDocumentTab = DocumentTabKind.Primary;

            highlightings = new List<HighlightingInfo>(highlightingProvider.HighlightingDefinitions);
            highlightings.Sort((h1, h2) => h1.Name.CompareTo(h2.Name));

            // Initializing conditions

            documentExistsCondition = new Condition(ActiveDocument != null);
            documentIsTextCondition = new Condition(ActiveDocument is TextDocumentViewModel);
            canUndoCondition = new MutableSourcePropertyWatchCondition<MainWindowViewModel, BaseDocumentViewModel>(this, vm => vm.ActiveDocument, doc => doc.CanUndo, false);
            canRedoCondition = new MutableSourcePropertyWatchCondition<MainWindowViewModel, BaseDocumentViewModel>(this, vm => vm.ActiveDocument, doc => doc.CanRedo, false);
            canSaveCondition = new MutableSourcePropertyWatchCondition<MainWindowViewModel, BaseDocumentViewModel>(this, vm => vm.ActiveDocument, doc => doc.CanSave, false);
            selectionAvailableCondition = new MutableSourcePropertyWatchCondition<MainWindowViewModel, BaseDocumentViewModel>(this, vm => ActiveDocument, doc => doc.SelectionAvailable, false);
            regularSelectionAvailableCondition = new MutableSourcePropertyWatchCondition<MainWindowViewModel, BaseDocumentViewModel>(this, vm => ActiveDocument, doc => doc.RegularSelectionAvailable, false);
            searchPerformedCondition = new MutableSourcePropertyNotNullWatchCondition<MainWindowViewModel, BaseDocumentViewModel>(this, vm => ActiveDocument, doc => doc.LastSearch);
            xmlToolsetAvailableCondition = new MutableSourcePropertyFuncCondition<MainWindowViewModel, BaseDocumentViewModel, HighlightingInfo>(this, vm => ActiveDocument, doc => doc.Highlighting, hi => (hi?.AdditionalToolset ?? AdditionalToolset.None) == AdditionalToolset.Xml, false);
            markdownToolsetAvailableCondition = new MutableSourcePropertyFuncCondition<MainWindowViewModel, BaseDocumentViewModel, HighlightingInfo>(this, vm => ActiveDocument, doc => doc.Highlighting, hi => (hi?.AdditionalToolset ?? AdditionalToolset.None) == AdditionalToolset.Markdown, false);
            documentPathVirtualCondition = new MutableSourcePropertyWatchCondition<MainWindowViewModel, BaseDocumentViewModel>(this, vm => vm.ActiveDocument, doc => doc.FilenameVirtual, true);
            documentHasPathCondition = documentExistsCondition & !documentPathVirtualCondition;

            // Initializing tools

            explorerToolViewModel = new ExplorerToolViewModel(fileIconProvider, imageResources, configurationService, this, eventBus, platformService);
            binDefinitionsToolViewModel = new BinDefinitionsToolViewModel(this,
                imageResources,
                configurationService,
                dialogService,
                messagingService,
                pathService);

            // Initializeing bottom tools

            messagesBottomToolViewModel = new MessagesBottomToolViewModel(this, imageResources);

            // Initializing commands

            // Some commands should not be registered in command repository service
            NavigateCommand = new AppCommand(obj => DoNavigate());
            NewPrimaryTextCommand = new AppCommand(obj => DoNewPrimaryText());
            NewSecondaryTextCommand = new AppCommand(obj => DoNewSecondaryText());

            ConfigCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Configuration, "Settings16.png", obj => DoOpenConfiguration());
            RunStoredSearchCommand = new AppCommand(obj => DoRunStoredSearch(obj as StoredSearchReplace), documentIsTextCondition);

            NewTextCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Home_File_New, "New16.png", obj => DoNewTextDocument(CurrentDocuments));
            NewHexCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Home_File_NewHex, "New16.png", obj => DoNewHexDocument(CurrentDocuments));
            OpenTextCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Home_File_Open, "Open16.png", obj => DoOpenTextDocument(CurrentDocuments));
            OpenHexCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Home_File_OpenHex, "Open16.png", obj => DoOpenHexDocument(CurrentDocuments));
            OpenBinCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Home_File_OpenBin, "Open16.png", obj => DoOpenBinDocument());
            SaveCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Home_File_Save, "Save16.png", obj => DoSaveDocument(), canSaveCondition);
            SaveAsCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Home_File_SaveAs, "Save16.png", obj => DoSaveDocumentAs(), canSaveCondition);

            UndoCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Home_Edit_Undo, "Undo16.png", obj => DoUndo(), canUndoCondition);
            RedoCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Home_Edit_Redo, "Redo16.png", obj => DoRedo(), canRedoCondition);
            CopyCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Home_Edit_Copy, "Copy16.png", obj => DoCopy(), selectionAvailableCondition);
            CutCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Home_Edit_Cut, "Cut16.png", obj => DoCut(), selectionAvailableCondition);
            PasteCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Home_Edit_Paste, "Paste16.png", obj => DoPaste(), documentExistsCondition);

            SearchCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Home_Search_Search, "Search16.png", obj => DoSearch(), documentIsTextCondition);
            ReplaceCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Home_Search_Replace, "Replace16.png", obj => DoReplace(), documentIsTextCondition);
            FindNextCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Home_Search_FindNext, "FindNext16.png", obj => DoFindNext(), documentIsTextCondition & searchPerformedCondition);

            SortLinesAscendingCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Lines_Ordering_SortAscending, "SortAscending16.png", obj => DoSortAscending(), documentIsTextCondition);
            SortLinesDescendingCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Lines_Ordering_SortDescending, "SortDescending16.png", obj => DoSortDescending(), documentIsTextCondition);
            RemoveEmptyLinesCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Lines_Cleanup_RemoveEmptyLines, null, obj => DoRemoveEmptyLines(), documentIsTextCondition);
            RemoveWhitespaceLinesCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Lines_Cleanup_RemoveWhitespaceLines, null, obj => DoRemoveWhitespaceLines(), documentIsTextCondition);

            LowercaseCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Text_Convert_Case_Lowercase, null, obj => DoLowercase(), documentIsTextCondition);
            UppercaseCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Text_Convert_Case_Uppercase, null, obj => DoUppercase(), documentIsTextCondition);
            NamingCaseCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Text_Convert_Case_Naming, null, obj => DoNamingCase(), documentIsTextCondition);
            SentenceCaseCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Text_Convert_Case_Sentence, null, obj => DoSentenceCase(), documentIsTextCondition);
            InvertCaseCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Text_Convert_Case_Invert, null, obj => DoInvertCase(), documentIsTextCondition);

            Base64EncodeCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Text_Convert_Base64_ToBase64, null, obj => DoBase64Encode(), documentIsTextCondition);
            Base64DecodeCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Text_Convert_Base64_FromBase64, null, obj => DoBase64Decode(), documentIsTextCondition);

            FormatXmlCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_XmlTools_Formatting_Format, "FormatXml16.png", obj => DoFormatXml(), xmlToolsetAvailableCondition);
            TransformXsltCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_XmlTools_Transform_XSLT, null, obj => DoTransformXslt(), xmlToolsetAvailableCondition);

            InsertMarkdownHeader1Command = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Markdown_Headers_H1, "H116.png", obj => DoInsertMarkdownHeader1(), markdownToolsetAvailableCondition);
            InsertMarkdownHeader2Command = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Markdown_Headers_H2, "H216.png", obj => DoInsertMarkdownHeader2(), markdownToolsetAvailableCondition);
            InsertMarkdownHeader3Command = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Markdown_Headers_H3, "H316.png", obj => DoInsertMarkdownHeader3(), markdownToolsetAvailableCondition);
            InsertMarkdownHeader4Command = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Markdown_Headers_H4, "H416.png", obj => DoInsertMarkdownHeader4(), markdownToolsetAvailableCondition);
            InsertMarkdownHeader5Command = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Markdown_Headers_H5, "H516.png", obj => DoInsertMarkdownHeader5(), markdownToolsetAvailableCondition);
            InsertMarkdownHeader6Command = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Markdown_Headers_H6, "H616.png", obj => DoInsertMarkdownHeader6(), markdownToolsetAvailableCondition);

            InsertMarkdownBlockCodeCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Markdown_Blocks_Code, null, obj => DoInsertMarkdownBlockCodeCommand(), markdownToolsetAvailableCondition);
            InsertMarkdownBlockquoteCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Markdown_Blocks_Blockquote, null, obj => DoInsertMarkdownBlockquoteCommand(), markdownToolsetAvailableCondition);
            InsertMarkdownOrderedListCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Markdown_Blocks_OrderedList, null, obj => DoInsertMarkdownOrderedListCommand(), markdownToolsetAvailableCondition);
            InsertMarkdownUnorderedListCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Markdown_Blocks_UnorderedList, null, obj => DoInsertMarkdownUnorderedListCommand(), markdownToolsetAvailableCondition);
            InsertMarkdownInlineCodeCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Markdown_Inline_Code, null, obj => DoInsertMarkdownInlineCodeCommand(), markdownToolsetAvailableCondition);
            InsertMarkdownEmphasisCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Markdown_Inline_Emphasis, null, obj => DoInsertMarkdownEmphasisCommand(), markdownToolsetAvailableCondition);
            InsertMarkdownStrongCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Markdown_Inline_Strong, null, obj => DoInsertMarkdownStrongCommand(), markdownToolsetAvailableCondition);
            InsertMarkdownLinkCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Markdown_Insert_Link, null, obj => DoInsertMarkdownLinkCommand(), markdownToolsetAvailableCondition);
            InsertMarkdownPictureCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Markdown_Insert_Picture, null, obj => DoInsertMarkdownPictureCommand(), markdownToolsetAvailableCondition);
            InsertMarkdownHorizontalRuleCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Markdown_Insert_Hr, null, obj => DoInsertMarkdownHorizontalRuleCommand(), markdownToolsetAvailableCondition);

            commandRepositoryService.RegisterCommand(Resources.Strings.Command_ToggleWordWrap, "WordWrap16.png", obj => DoToggleWordWrap());
            commandRepositoryService.RegisterCommand(Resources.Strings.Command_ToggleLineNumbers, "LineNumbers16.png", obj => DoToggleLineNumbers());

            // Registering commands for syntax highlightings
            foreach (var highlighting in highlightings)
            {
                commandRepositoryService.RegisterCommand(String.Format(Resources.Strings.Command_SetHighlighting, highlighting.Name),
                    "Highlighting16.png",
                    obj => DoSetHighlighting(highlighting),
                    documentIsTextCondition);
            }

            // Initializing stored search/replaces

            storedSearches = new ObservableCollection<StoredSearchReplaceViewModel>();
            storedReplaces = new ObservableCollection<StoredSearchReplaceViewModel>();
            BuildStoredSearchReplaceViewModels();

            // Navigation

            navigationText = String.Empty;
            navigationItems = new ObservableCollection<BaseNavigationModel>();

            // Applying current close behavior

            if (configurationService.Configuration.Behavior.CloseBehavior.Value == CloseBehavior.Fluent)
            {
                RestoreFiles();

                OpenParameters();

                if (primaryDocuments.Count == 0)
                    DoNewTextDocument(CurrentDocuments);
            }
            else if (configurationService.Configuration.Behavior.CloseBehavior.Value == CloseBehavior.Standard)
            {
                if (!OpenParameters())
                    DoNewTextDocument(CurrentDocuments);
            }
            else
                throw new InvalidOperationException("Invalid close behavior!");

            showSecondaryDocumentTab = secondaryDocuments.Count > 0;

            if (primaryDocuments.Count > 0)
                SelectedPrimaryDocument = primaryDocuments.First();
            if (secondaryDocuments.Count > 0)
                SelectedSecondaryDocument = secondaryDocuments.First();

            UpdateActiveDocument();
        }

        private void DoNewSecondaryText()
        {
            DoNewTextDocument(secondaryDocuments);
        }

        private void DoNewPrimaryText()
        {
            DoNewTextDocument(primaryDocuments);
        }

        public void NotifyActivated()
        {
            VerifyRemovedAndModifiedDocuments();
        }

        private void VerifyRemovedAndModifiedDocuments()
        {
            void InternalVerifyRemovedAndModifiedDocuments(IList<BaseDocumentViewModel> documents)
            {
                // Check documents
                int i = 0;

                while (i < documents.Count)
                {
                    var document = documents[i];

                    if (document.FilenameVirtual)
                    {
                        i++;
                        continue;
                    }

                    // Checking if file still exists

                    if (!File.Exists(document.FileName))
                    {
                        ActiveDocument = document;

                        if (messagingService.AskYesNo(String.Format(Strings.Message_DocumentDeleted, document.FileName)) == false)
                        {
                            // Remove document
                            RemoveDocument(document);
                            continue;
                        }
                        else
                        {
                            string newFilename = System.IO.Path.GetFileName(document.FileName);

                            document.SetFilename(newFilename, fileIconProvider.GetImageForFile(newFilename));
                            document.FilenameVirtual = true;
                        }
                    }
                    else
                    {
                        try
                        {
                            var fileModificationDate = System.IO.File.GetLastWriteTimeUtc(document.FileName);

                            if (document.LastModificationDate < fileModificationDate)
                            {
                                ActiveDocument = document;

                                if (messagingService.AskYesNo(String.Format(Strings.Message_DocumentModifiedOutsideEditor, document.FileName)) == true)
                                {
                                    int documentIndex = i;

                                    ReplaceReloadDocument(documents, document);
                                }
                                else
                                {
                                    // Set this date as new reference point. User will no
                                    // longer be notified about this change, but will be
                                    // notified about any next change.

                                    document.LastModificationDate = fileModificationDate;
                                }
                            }
                        }
                        catch
                        {
                            // This is just for user's convenience.
                            // Cannot get the date? Just ignore this document.                        
                        }

                        i++;
                    }
                }
            }

            InternalVerifyRemovedAndModifiedDocuments(primaryDocuments);
            InternalVerifyRemovedAndModifiedDocuments(secondaryDocuments);
        }

        public void NotifyLoaded()
        {
            var uiConfig = configurationService.Configuration.UI;

            if (uiConfig.MainWindowSizeSet.Value)
                access.SetWindowSize(new System.Windows.Size(uiConfig.MainWindowWidth.Value,
                    uiConfig.MainWindowHeight.Value));

            if (uiConfig.MainWindowLocationSet.Value)
                access.SetWindowLocation(new System.Windows.Point(uiConfig.MainWindowX.Value,
                    uiConfig.MainWindowY.Value));

            if (uiConfig.MainWindowMaximized.Value)
                access.SetWindowMaximized(true);
        }

        public bool CanCloseApplication()
        {
            bool CloseDocuments(IList<BaseDocumentViewModel> documents)
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

                return true;
            }

            switch (configurationService.Configuration.Behavior.CloseBehavior.Value)
            {
                case CloseBehavior.Standard:
                    {
                        if (!(CloseDocuments(primaryDocuments) && CloseDocuments(secondaryDocuments)))
                            return false;

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
            ActiveDocument?.FocusDocument();
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

        public void NotifyFilesDropped(string[] files)
        {
            foreach (var file in files)
            {
                LoadTextDocument(CurrentDocuments, file);
            }
        }

        public void NotifyClosingWindow()
        {
            var windowSize = access.GetWindowSize();
            var windowLocation = access.GetWindowLocation();

            var maximized = access.GetMaximized();
            configurationService.Configuration.UI.MainWindowMaximized.Value = maximized;

            if (!maximized)
            {
                Models.Configuration.UI.UIConfig uiConfig = configurationService.Configuration.UI;

                uiConfig.MainWindowWidth.Value = windowSize.Width;
                uiConfig.MainWindowHeight.Value = windowSize.Height;
                uiConfig.MainWindowSizeSet.Value = true;

                uiConfig.MainWindowX.Value = windowLocation.X;
                uiConfig.MainWindowY.Value = windowLocation.Y;
                uiConfig.MainWindowLocationSet.Value = true;
            }

            // Allow all other interested parties to write configuration
            eventBus.Send(this, new ApplicationShutdownEvent());

            configurationService.Save();
        }

        public void NotifyArgsReceived(List<string> args)
        {
            access.BringToFront();
            DoOpenParameters(args);
        }

        public void ReorderDocument(BaseDocumentViewModel fromDoc, BaseDocumentViewModel toDoc)
        {
            void MoveInside(ObservableCollection<BaseDocumentViewModel> documents, BaseDocumentViewModel fromDocument, BaseDocumentViewModel toDocument)
            {
                var fromIndex = documents.IndexOf(fromDocument);
                var toIndex = documents.IndexOf(toDocument);

                documents.Move(fromIndex, toIndex);
            }

            void MoveFromTo(ObservableCollection<BaseDocumentViewModel> fromDocuments, BaseDocumentViewModel fromDocument,
                ObservableCollection<BaseDocumentViewModel> toDocuments, BaseDocumentViewModel toDocument)
            {
                int fromIndex = fromDocuments.IndexOf(fromDocument);
                int toIndex = toDocuments.IndexOf(toDocument);

                var doc = fromDocuments[fromIndex];
                fromDocuments.RemoveAt(fromIndex);
                toDocuments.Insert(toIndex, doc);
            }

            if (fromDoc == toDoc)
                return;

            if (primaryDocuments.Contains(fromDoc))
            {
                int fromIndex = primaryDocuments.IndexOf(fromDoc);

                if (primaryDocuments.Contains(toDoc))
                {
                    MoveInside(primaryDocuments, fromDoc, toDoc);
                }
                else if (secondaryDocuments.Contains(toDoc))
                {
                    MoveFromTo(primaryDocuments, fromDoc, secondaryDocuments, toDoc);
                }
                else
                    throw new InvalidOperationException("Target document does not exist in internal collections!");
            }
            else if (secondaryDocuments.Contains(fromDoc))
            {
                int fromIndex = secondaryDocuments.IndexOf(fromDoc);

                if (primaryDocuments.Contains(toDoc))
                {
                    MoveFromTo(secondaryDocuments, fromDoc, primaryDocuments, toDoc);
                }
                else if (secondaryDocuments.Contains(toDoc))
                {
                    MoveInside(secondaryDocuments, fromDoc, toDoc);
                }
                else
                    throw new InvalidOperationException("Target document does not exist in internal collections!");
            }
            else
                throw new InvalidOperationException("Target document does not exist in internal collections!");

            ActiveDocument = fromDoc;
        }

        public void MoveDocumentTo(BaseDocumentViewModel documentViewModel, ObservableCollection<BaseDocumentViewModel> destinationDocuments)
        {
            ObservableCollection<BaseDocumentViewModel> sourceDocuments = null;

            if (destinationDocuments != primaryDocuments && destinationDocuments != secondaryDocuments)
                throw new ArgumentException(nameof(destinationDocuments));

            if (primaryDocuments.Contains(documentViewModel))
                sourceDocuments = primaryDocuments;
            else if (secondaryDocuments.Contains(documentViewModel))
                sourceDocuments = secondaryDocuments;
            else
                throw new ArgumentException(nameof(documentViewModel));

            // The only case when nothing needs to be done
            if (sourceDocuments == destinationDocuments && sourceDocuments.IndexOf(documentViewModel) == sourceDocuments.Count - 1)
                return;

            sourceDocuments.Remove(documentViewModel);
            destinationDocuments.Add(documentViewModel);

            ActiveDocument = documentViewModel;
        }

        // Public properties --------------------------------------------------

        public ObservableCollection<BaseDocumentViewModel> PrimaryDocuments => primaryDocuments;

        public ObservableCollection<BaseDocumentViewModel> SecondaryDocuments => secondaryDocuments;

        public BaseDocumentViewModel ActiveDocument
        {
            get => activeDocument;
            set => SetActiveDocument(value);
        }

        public IReadOnlyList<HighlightingInfo> Highlightings => highlightings;

        public string NavigationText
        {
            get => navigationText;
            set => Set(ref navigationText, () => NavigationText, value);
        }

        public ObservableCollection<BaseNavigationModel> NavigationItems => navigationItems;

        public BaseNavigationModel SelectedNavigationItem
        {
            get => selectedNavigationItem;
            set => Set(ref selectedNavigationItem, () => SelectedNavigationItem, value);
        }

        public SidePanelPlacement SidePanelPlacement
        {
            get => sidePanelPlacement;
            set => Set(ref sidePanelPlacement, () => SidePanelPlacement, value, HandleSidePanelPlacementChanged);
        }

        public IEnumerable<SidePanelPlacementModel> SidePanelPlacements => sidePanelPlacements;

        public double SidePanelSize
        {
            get => sidePanelSize;
            set => Set(ref sidePanelSize, () => SidePanelSize, value, HandleSidePanelSizeChanged);
        }

        public BottomPanelVisibility BottomPanelVisibility
        {
            get => bottomPanelVisibility;
            set => Set(ref bottomPanelVisibility, () => BottomPanelVisibility, value, HandleBottomPanelVisibilityChanged);
        }

        public IEnumerable<BottomPanelVisibilityModel> BottomPanelVisibilities => bottomPanelVisibilities;

        public double BottomPanelSize
        {
            get => bottomPanelSize;
            set => Set(ref bottomPanelSize, () => BottomPanelSize, value, HandleBottomPanelSizeChanged);
        }

        public ExplorerToolViewModel ExplorerToolViewModel => explorerToolViewModel;

        public BinDefinitionsToolViewModel BinDefinitionsToolViewModel => binDefinitionsToolViewModel;

        public MessagesBottomToolViewModel MessagesBottomToolViewModel => messagesBottomToolViewModel;

        public ObservableCollection<StoredSearchReplaceViewModel> StoredSearches => storedSearches;

        public ObservableCollection<StoredSearchReplaceViewModel> StoredReplaces => storedReplaces;

        public DocumentTabKind ActiveDocumentTab
        {
            get => activeDocumentTab;
            set => Set(ref activeDocumentTab, () => ActiveDocumentTab, value, HandleActiveDocumentTabChanged);
        }

        public BaseDocumentViewModel SelectedPrimaryDocument
        {
            get => selectedPrimaryDocument;
            set => Set(ref selectedPrimaryDocument, () => SelectedPrimaryDocument, value, HandleSelectedPrimaryDocumentChanged);
        }

        public BaseDocumentViewModel SelectedSecondaryDocument
        {
            get => selectedSecondaryDocument;
            set => Set(ref selectedSecondaryDocument, () => SelectedSecondaryDocument, value, HandleSelectedSecondaryDocumentChanged);
        }

        public bool ShowSecondaryDocumentTab
        {
            get => showSecondaryDocumentTab;
            set => Set(ref showSecondaryDocumentTab, () => ShowSecondaryDocumentTab, value, HandleShowSecondaryDocumentTabChanged);
        }
    }
}
