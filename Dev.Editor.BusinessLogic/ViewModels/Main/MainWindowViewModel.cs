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

        private readonly ObservableCollection<BaseDocumentViewModel> documents;
        private BaseDocumentViewModel activeDocument;

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

        private void HandleBottomPanelSizeChanged()
        {
            configurationService.Configuration.UI.BottomPanelSize.Value = bottomPanelSize;
        }

        private void HandleBottomPanelVisibilityChanged()
        {
            configurationService.Configuration.UI.BottomPanelVisibility.Value = bottomPanelVisibility;
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
                    case BinDocumentViewModel binDocument:
                        {
                            var storedFile = new BinStoredFile();
                            storedFile.Filename.Value = binDocument.FileName;
                            storedFile.FilenameIsVirtual.Value = binDocument.FilenameVirtual;
                            storedFile.IsDirty.Value = binDocument.Changed;
                            storedFile.StoredFilename.Value = document.FileName;
                            storedFile.DefinitionUid.Value = binDocument.Definition.Uid.Value;

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
                    case BinStoredFile binStoredFile:
                        {
                            try
                            {
                                var def = configurationService.Configuration.BinDefinitions.FirstOrDefault(bd => bd.Uid.Value == binStoredFile.DefinitionUid.Value);
                                if (def == null)
                                    continue;

                                InternalAddBinDocument(document =>
                                {
                                    InternalReadBinDocument(document, binStoredFile.StoredFilename.Value, def);

                                    document.SetFilename(binStoredFile.Filename.Value, fileIconProvider.GetImageForFile(binStoredFile.Filename.Value));
                                    document.FilenameVirtual = binStoredFile.FilenameIsVirtual.Value;
                                    document.Changed = binStoredFile.IsDirty.Value;
                                });
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
            LoadTextDocument(filename);

            
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

        // IDocumentHandler implementation ------------------------------------

        void IDocumentHandler.RequestClose(BaseDocumentViewModel documentViewModel)
        {
            if (CanCloseDocument(documentViewModel))
                RemoveDocument(documentViewModel);
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
            LoadTextDocument(path);
        }

        string IExplorerHandler.GetCurrentDocumentPath()
        {
            return ActiveDocument.FileName;
        }

        BaseCondition IExplorerHandler.CurrentDocumentHasPathCondition => documentHasPathCondition;

        // IBinDefinitionsHandler implementation ------------------------------

        void IBinDefinitionsHandler.OpenTextFile(string path)
        {
            LoadTextDocument(path);
        }

        void IBinDefinitionsHandler.NewTextDocument(string text)
        {
            DoNewTextDocument();

            (ActiveDocument as TextDocumentViewModel).Document.Insert(0, text);
        }

        void IBinDefinitionsHandler.RequestOpenBinFile(BinDefinition binDefinition)
        {
            DoOpenBinDocument(binDefinition);
        }

        // IBottomToolsHandler implementation ---------------------------------

        void IBottomToolHandler.CloseBottomTools()
        {
            BottomPanelVisibility = BottomPanelVisibility.Hidden;
        }

        // IMessagesHandler implementation ------------------------------------

        void IMessagesHandler.OpenFileAndFocus(string filename)
        {
            LoadTextDocument(filename);
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

            documents = new ObservableCollection<BaseDocumentViewModel>();

            highlightings = new List<HighlightingInfo>(highlightingProvider.HighlightingDefinitions);
            highlightings.Sort((h1, h2) => h1.Name.CompareTo(h2.Name));

            // Initializing conditions

            documentExistsCondition = new Condition(activeDocument != null);
            documentIsTextCondition = new Condition(activeDocument is TextDocumentViewModel);
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

            // This command should not be registered in command repository service
            NavigateCommand = new AppCommand(obj => DoNavigate());

            ConfigCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Configuration, "Settings16.png", obj => DoOpenConfiguration());
            RunStoredSearchCommand = new AppCommand(obj => DoRunStoredSearch(obj as StoredSearchReplace), documentIsTextCondition);

            NewTextCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Home_File_New, "New16.png", obj => DoNewTextDocument());
            NewHexCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Home_File_NewHex, "New16.png", obj => DoNewHexDocument());
            OpenTextCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Home_File_Open, "Open16.png", obj => DoOpenTextDocument());
            OpenHexCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Home_File_OpenHex, "Open16.png", obj => DoOpenHexDocument());
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
            UppercaseCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Text_Convert_Case_Lowercase, null, obj => DoUppercase(), documentIsTextCondition);
            NamingCaseCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Text_Convert_Case_Lowercase, null, obj => DoNamingCase(), documentIsTextCondition);
            SentenceCaseCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Text_Convert_Case_Lowercase, null, obj => DoSentenceCase(), documentIsTextCondition);
            InvertCaseCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Text_Convert_Case_Lowercase, null, obj => DoInvertCase(), documentIsTextCondition);

            Base64EncodeCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Text_Convert_Base64_ToBase64, null, obj => DoBase64Encode(), documentIsTextCondition);
            Base64DecodeCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Text_Convert_Base64_FromBase64, null, obj => DoBase64Decode(), documentIsTextCondition);

            FormatXmlCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_XmlTools_Formatting_Format, "FormatXml16.png", obj => DoFormatXml(), xmlToolsetAvailableCondition);
            TransformXsltCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_XmlTools_Transform_XSLT, null, obj => DoTransformXslt(), xmlToolsetAvailableCondition);

            InsertMarkdownHeader1Command = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Markdown_Headers_H1, null, obj => DoInsertMarkdownHeader1(), markdownToolsetAvailableCondition);
            InsertMarkdownHeader2Command = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Markdown_Headers_H2, null, obj => DoInsertMarkdownHeader2(), markdownToolsetAvailableCondition);
            InsertMarkdownHeader3Command = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Markdown_Headers_H3, null, obj => DoInsertMarkdownHeader3(), markdownToolsetAvailableCondition);
            InsertMarkdownHeader4Command = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Markdown_Headers_H4, null, obj => DoInsertMarkdownHeader4(), markdownToolsetAvailableCondition);
            InsertMarkdownHeader5Command = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Markdown_Headers_H5, null, obj => DoInsertMarkdownHeader5(), markdownToolsetAvailableCondition);
            InsertMarkdownHeader6Command = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Markdown_Headers_H6, null, obj => DoInsertMarkdownHeader6(), markdownToolsetAvailableCondition);

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

        private void DoSetHighlighting(HighlightingInfo highlighting)
        {
            ActiveDocument.Highlighting = highlighting;
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

        public void NotifyFilesDropped(string[] files)
        {
            foreach (var file in files)
            {
                LoadTextDocument(file);
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
            if (fromDoc == toDoc)
                return;

            int fromIndex = documents.IndexOf(fromDoc);
            int toIndex = documents.IndexOf(toDoc);

            documents.Move(fromIndex, toIndex);
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
    }
}
