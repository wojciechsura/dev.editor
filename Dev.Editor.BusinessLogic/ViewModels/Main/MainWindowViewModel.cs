using Dev.Editor.BusinessLogic.Models.Configuration.BinDefinitions;
using Dev.Editor.BusinessLogic.Models.Configuration.Internal;
using Dev.Editor.BusinessLogic.Models.Configuration.Search;
using Dev.Editor.BusinessLogic.Models.Dialogs;
using Dev.Editor.BusinessLogic.Models.Documents.Text;
using Dev.Editor.BusinessLogic.Models.Events;
using Dev.Editor.BusinessLogic.Models.Highlighting;
using Dev.Editor.BusinessLogic.Models.Navigation;
using Dev.Editor.BusinessLogic.Models.Search;
using Dev.Editor.BusinessLogic.Models.UI;
using Dev.Editor.BusinessLogic.Services.Commands;
using Dev.Editor.BusinessLogic.Services.Config;
using Dev.Editor.BusinessLogic.Services.Dialogs;
using Dev.Editor.BusinessLogic.Services.EventBus;
using Dev.Editor.BusinessLogic.Services.FileIcons;
using Dev.Editor.BusinessLogic.Services.Highlighting;
using Dev.Editor.BusinessLogic.Services.ImageResources;
using Dev.Editor.BusinessLogic.Services.Messaging;
using Dev.Editor.BusinessLogic.Services.Paths;
using Dev.Editor.BusinessLogic.Services.Platform;
using Dev.Editor.BusinessLogic.Services.SearchEncoder;
using Dev.Editor.BusinessLogic.Services.StartupInfo;
using Dev.Editor.BusinessLogic.Services.TextComparison;
using Dev.Editor.BusinessLogic.Services.TextTransform;
using Dev.Editor.BusinessLogic.Types.Behavior;
using Dev.Editor.BusinessLogic.Types.BottomTools;
using Dev.Editor.BusinessLogic.Types.Main;
using Dev.Editor.BusinessLogic.Types.Search;
using Dev.Editor.BusinessLogic.Types.Tools;
using Dev.Editor.BusinessLogic.Types.UI;
using Dev.Editor.BusinessLogic.ViewModels.Base;
using Dev.Editor.BusinessLogic.ViewModels.BottomTools.Base;
using Dev.Editor.BusinessLogic.ViewModels.BottomTools.Messages;
using Dev.Editor.BusinessLogic.ViewModels.BottomTools.SearchResults;
using Dev.Editor.BusinessLogic.ViewModels.Document;
using Dev.Editor.BusinessLogic.ViewModels.FindInFiles;
using Dev.Editor.BusinessLogic.ViewModels.Main.Documents;
using Dev.Editor.BusinessLogic.ViewModels.Search;
using Dev.Editor.BusinessLogic.ViewModels.Tools.Base;
using Dev.Editor.BusinessLogic.ViewModels.Tools.BinDefinitions;
using Dev.Editor.BusinessLogic.ViewModels.Tools.Explorer;
using Dev.Editor.Common.Tools;
using Dev.Editor.Resources;
using ICSharpCode.AvalonEdit.Document;
using Microsoft.Win32;
using Spooksoft.VisualStateManager.Commands;
using Spooksoft.VisualStateManager.Conditions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Threading;
using System;
using Dev.Editor.BusinessLogic.Services.AppVersion;
using Dev.Editor.BusinessLogic.ViewModels.Tools.Project;
using Dev.Editor.BusinessLogic.Models.DuplicatedLines;

namespace Dev.Editor.BusinessLogic.ViewModels.Main
{
    public partial class MainWindowViewModel : BaseViewModel, IDocumentHandler,
        IExplorerHandler, IBinDefinitionsHandler, IMessagesHandler, ISearchResultsHandler, 
        IProjectHandler, IEventListener<StoredSearchesChangedEvent>
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
        private readonly ITextComparisonService textComparisonService;
        private readonly ITextTransformService textTransformService;
        private readonly IAppVersionService appVersionService;
        private readonly DocumentsManager documentsManager;

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
        private SideTool selectedSideTool;

        private BottomPanelVisibility bottomPanelVisibility;
        private List<BottomPanelVisibilityModel> bottomPanelVisibilities;
        private double bottomPanelSize;
        private BottomTool selectedBottomTool;

        // Tools

        private readonly ExplorerToolViewModel explorerToolViewModel;
        private readonly ProjectToolViewModel projectToolViewModel;
        private readonly BinDefinitionsToolViewModel binDefinitionsToolViewModel;

        private readonly MessagesBottomToolViewModel messagesBottomToolViewModel;
        private readonly SearchResultsBottomToolViewModel searchResultsBottomToolViewModel;

        // Private methods ----------------------------------------------------

        private void ClearAllDiffs()
        {
            foreach (var doc in documentsManager.AllDocuments.OfType<TextDocumentViewModel>())
            {
                doc.DiffResult = null;
            }
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

        private bool StoreFiles()
        {
            int storedFileIndex = 0;

            InternalConfig internalConfiguration = configurationService.Configuration.Internal;
            bool StoreDocuments(ITabDocumentCollection<BaseDocumentViewModel> documents)
            {
                void StoreGeneralDocumentInfo(BaseDocumentViewModel document, BaseStoredFile storedFile, string storedFilename)
                {
                    storedFile.Filename.Value = document.FileName;
                    storedFile.FilenameIsVirtual.Value = document.FilenameVirtual;
                    storedFile.IsDirty.Value = document.Changed;
                    storedFile.StoredFilename.Value = storedFilename;
                    storedFile.LastModifiedDate.Value = document.LastModificationDate.Ticks;
                    storedFile.DocumentTabKind.Value = documents.DocumentTabKind;
                    storedFile.TabColor.Value = document.TabColor;
                    storedFile.IsPinned.Value = document.IsPinned;
                    storedFile.Guid.Value = document.Guid.ToString();
                }

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

                    BaseStoredFile storedFile;

                    switch (document)
                    {
                        case TextDocumentViewModel textDocument:
                            {
                                storedFile = new TextStoredFile();
                                StoreGeneralDocumentInfo(textDocument, storedFile, storedFilename);

                                ((TextStoredFile)storedFile).HighlightingName.Value = textDocument.Highlighting?.Name;
                                
                                break;
                            }
                        case HexDocumentViewModel hexDocument:
                            {
                                storedFile = new HexStoredFile();
                                StoreGeneralDocumentInfo(hexDocument, storedFile, storedFilename);

                                break;
                            }
                        case BinDocumentViewModel binDocument:
                            {
                                storedFile = new BinStoredFile();
                                StoreGeneralDocumentInfo(binDocument, storedFile, storedFilename);

                                break;
                            }
                        default:
                            throw new InvalidOperationException("Unsupported document type!");
                    }

                    internalConfiguration.StoredFiles.Add(storedFile);
                }

                return true;
            }

            internalConfiguration.StoredFiles.Clear();

            bool stored = StoreDocuments(documentsManager.PrimaryDocuments) && 
                StoreDocuments(documentsManager.SecondaryDocuments);

            if (stored)
            {
                internalConfiguration.PrimarySelectedDocument.Value = documentsManager.SelectedPrimaryDocument?.Guid.ToString() ?? string.Empty;
                internalConfiguration.SecondarySelectedDocument.Value = documentsManager.SelectedSecondaryDocument?.Guid.ToString() ?? string.Empty;
                internalConfiguration.ActiveDocument.Value = documentsManager.ActiveDocument?.Guid.ToString() ?? string.Empty;
            }

            return stored;
        }

        private void RestoreFiles()
        {
            InternalConfig internalConfg = configurationService.Configuration.Internal;

            for (int i = 0; i < internalConfg.StoredFiles.Count; i++)
            {
                var file = internalConfg.StoredFiles[i];

                Guid fileGuid;
                if (!Guid.TryParse(file.Guid.Value, out fileGuid))
                    fileGuid = Guid.NewGuid(); ;

                BaseDocumentViewModel document = null;

                // TODO remove duplicated code
                switch (file)
                {
                    case TextStoredFile textStoredFile:
                        {
                            try
                            {
                                var textDocument = new TextDocumentViewModel(this, fileGuid);

                                InternalReadTextDocument(textDocument, textStoredFile.StoredFilename.Value);

                                textDocument.SetFilename(textStoredFile.Filename.Value, fileIconProvider.GetImageForFile(textStoredFile.Filename.Value));
                                textDocument.FilenameVirtual = textStoredFile.FilenameIsVirtual.Value;
                                textDocument.Changed = textStoredFile.IsDirty.Value;
                                textDocument.LastModificationDate = new DateTime(textStoredFile.LastModifiedDate.Value, DateTimeKind.Utc);
                                textDocument.TabColor = textStoredFile.TabColor.Value;
                                textDocument.IsPinned = textStoredFile.IsPinned.Value;

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
                                var hexDocument = new HexDocumentViewModel(this, fileGuid);

                                InternalReadHexDocument((HexDocumentViewModel)hexDocument, hexStoredFile.StoredFilename.Value);

                                hexDocument.SetFilename(hexStoredFile.Filename.Value, fileIconProvider.GetImageForFile(hexStoredFile.Filename.Value));
                                hexDocument.FilenameVirtual = hexStoredFile.FilenameIsVirtual.Value;
                                hexDocument.Changed = hexStoredFile.IsDirty.Value;
                                hexDocument.LastModificationDate = new DateTime(hexStoredFile.LastModifiedDate.Value, DateTimeKind.Utc);
                                hexDocument.TabColor = hexStoredFile.TabColor.Value;
                                hexDocument.IsPinned = hexStoredFile.IsPinned.Value;


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

                                var binDocument = new BinDocumentViewModel(this, fileGuid);

                                InternalReadBinDocument((BinDocumentViewModel)binDocument, binStoredFile.StoredFilename.Value, def);

                                binDocument.SetFilename(binStoredFile.Filename.Value, fileIconProvider.GetImageForFile(binStoredFile.Filename.Value));
                                binDocument.FilenameVirtual = binStoredFile.FilenameIsVirtual.Value;
                                binDocument.Changed = binStoredFile.IsDirty.Value;
                                binDocument.LastModificationDate = new DateTime(binStoredFile.LastModifiedDate.Value, DateTimeKind.Utc);
                                binDocument.TabColor = binStoredFile.TabColor.Value;
                                binDocument.IsPinned = binStoredFile.IsPinned.Value;


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
                    documentsManager.AddDocument(document, file.DocumentTabKind.Value);
            }

            // Restore left selected, right selected and active document

            if (Guid.TryParse(internalConfg.PrimarySelectedDocument.Value, out Guid selectedPrimaryGuid))
            {
                var document = documentsManager.PrimaryDocuments.FirstOrDefault(doc => doc.Guid == selectedPrimaryGuid);
                if (document != null)
                    documentsManager.SelectedPrimaryDocument = document;
            }

            if (Guid.TryParse(internalConfg.SecondarySelectedDocument.Value, out Guid selectedSecondaryGuid))
            {
                var document = documentsManager.SecondaryDocuments.FirstOrDefault(doc => doc.Guid == selectedSecondaryGuid);
                if (document != null)
                    documentsManager.SelectedSecondaryDocument = document;
            }

            if (Guid.TryParse(internalConfg.ActiveDocument.Value, out Guid activeGuid))
            {
                var document = documentsManager.AllDocuments.FirstOrDefault(doc => doc.Guid == activeGuid);
                if (document != null)
                    documentsManager.ActiveDocument = document;
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
                        LoadTextDocument(documentsManager.ActiveDocumentTab, param);
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
            LoadTextDocument(documentsManager.ActiveDocumentTab, filename);

            TextDocumentViewModel textDoc = documentsManager.ActiveDocument as TextDocumentViewModel;
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
            documentsManager.ActiveDocument.Highlighting = highlighting;
        }

        private void DoRunStoredSearch(StoredSearchReplace storedSearchReplace)
        {
            var desc = new SearchReplaceDescription(storedSearchReplace.Search.Value,
                storedSearchReplace.Replace.Value,
                storedSearchReplace.Operation.Value,
                storedSearchReplace.SearchMode.Value,
                storedSearchReplace.IsCaseSensitive.Value,
                selectionAvailableForSearchCondition.GetValue(),
                storedSearchReplace.IsSearchBackwards.Value,
                storedSearchReplace.IsWholeWordsOnly.Value,
                storedSearchReplace.ShowReplaceSummary.Value,
                storedSearchReplace.Location.Value,
                storedSearchReplace.FileMask.Value);

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

        private void DoNewPrimaryText()
        {
            DoNewTextDocument(DocumentTabKind.Primary);
        }

        private void DoNewSecondaryText()
        {
            DoNewTextDocument(DocumentTabKind.Secondary);
        }

        private void VerifyRemovedAndModifiedDocuments()
        {
            void InternalVerifyRemovedAndModifiedDocuments(ITabDocumentCollection<BaseDocumentViewModel> documents)
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
                        documentsManager.ActiveDocument = document;

                        if (messagingService.AskYesNo(String.Format(Strings.Message_DocumentDeleted, document.FileName)) == false)
                        {
                            // Remove document
                            documentsManager.RemoveDocument(document);
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
                                documentsManager.ActiveDocument = document;

                                if (messagingService.AskYesNo(String.Format(Strings.Message_DocumentModifiedOutsideEditor, document.FileName)) == true)
                                {
                                    int documentIndex = i;

                                    ReplaceReloadDocument(documents.DocumentTabKind, document);
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

            InternalVerifyRemovedAndModifiedDocuments(documentsManager.PrimaryDocuments);
            InternalVerifyRemovedAndModifiedDocuments(documentsManager.SecondaryDocuments);
        }

        private bool CloseDocumentsWhere(Func<BaseDocumentViewModel, bool> predicate)
        {
            bool InternalCloseDocuments(DocumentTabKind documentTabKind)
            {
                int i = 0;
                while (i < documentsManager[documentTabKind].Count)
                {
                    if (predicate(documentsManager[documentTabKind][i]))
                    {
                        if (CanCloseDocument(documentsManager[documentTabKind][i]))
                        {
                            documentsManager.RemoveDocumentAt(documentTabKind, i);
                        }
                        else
                            return false;
                    }
                    else
                    {
                        i++;
                    }
                }

                return true;
            }

            return InternalCloseDocuments(DocumentTabKind.Primary) && InternalCloseDocuments(DocumentTabKind.Secondary);
        }

        private void DoTryCloseDocument(BaseDocumentViewModel document)
        {
            if (CanCloseDocument(document))
                documentsManager.RemoveDocument(document);
        }

        private void PerformReplaceInFile(FileSearchResultViewModel file)
        {
            try
            {
                var result = new StringBuilder();
                var searchResults = new List<SearchResultViewModel>();

                using (var fs = new FileStream(file.FullPath, FileMode.Open, FileAccess.Read))
                using (var reader = new StreamReader(fs))
                {
                    int readChars = 0;

                    for (int i = 0; i < file.Results.Count; i++)
                    {
                        var replaceResult = (ReplaceResultViewModel)file.Results[i];

                        if (replaceResult.IsChecked)
                        {
                            // Append characters until found match
                            int countBeforeMatch = replaceResult.Offset - readChars;
                            if (countBeforeMatch > 0)
                            {
                                char[] data = new char[countBeforeMatch];
                                reader.ReadBlock(data, 0, countBeforeMatch);
                                result.Append(data);

                                readChars += countBeforeMatch;
                            }

                            // Append replaced text to the result buffer
                            result.Append(replaceResult.ReplaceWith);

                            // Skip old text in the original file
                            char[] tmp = new char[replaceResult.Match.Length];
                            reader.ReadBlock(tmp, 0, replaceResult.Match.Length);
                            readChars += replaceResult.Match.Length;
                        }
                    }

                    // Read remaining data to end
                    char[] buffer = new char[1024];
                    int count;
                    do
                    {
                        count = reader.ReadBlock(buffer, 0, 1024);
                        if (count > 0)
                        {
                            result.Append(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                }

                using (var fs = new FileStream(file.FullPath, FileMode.Create, FileAccess.Write))
                using (var writer = new StreamWriter(fs))
                {
                    writer.Write(result.ToString());
                }
            }
            catch
            {

            }
        }

        private void PerformReplaceInFilesRecursive(BaseFilesystemSearchResultViewModel item)
        {
            if (item is FolderSearchResultViewModel folder)
            {
                foreach (var entry in folder.Files)
                {
                    PerformReplaceInFilesRecursive(entry);
                }
            }
            else if (item is FileSearchResultViewModel file)
            {
                PerformReplaceInFile(file);
            }
        }

        private void DoOpenProject()
        {
            string startFolder = GetExplorerFolderIfPossible();

            (bool result, string location) = dialogService.ShowChooseFolderDialog(startFolder);
            if (result)
            {
                projectToolViewModel.OpenProject(location);
            }
        }

        private string GetExplorerFolderIfPossible()
        {
            string startFolder = null;
            if (this.sidePanelPlacement != SidePanelPlacement.Hidden)
                startFolder = explorerToolViewModel.SelectedFolder.GetFullPath();
            return startFolder;
        }

        // IDocumentHandler implementation ------------------------------------

        void IDocumentHandler.RequestClose(BaseDocumentViewModel document)
        {
            DoTryCloseDocument(document);
        }

        void IDocumentHandler.RequestCloseOthers(BaseDocumentViewModel baseDocumentViewModel)
        {
            CloseDocumentsWhere(doc => doc != baseDocumentViewModel);
        }

        void IDocumentHandler.RequestCloseAllButPinned()
        {
            CloseDocumentsWhere(doc => !doc.IsPinned);
        }

        void IDocumentHandler.RequestCloseAll()
        {
            CloseDocumentsWhere(doc => true);
        }

        void IDocumentHandler.ChildActivated(BaseDocumentViewModel document)
        {
            documentsManager.ActiveDocument = document;
        }

        ICommand IDocumentHandler.CopyCommand => CopyCommand;

        ICommand IDocumentHandler.CutCommand => CutCommand;

        ICommand IDocumentHandler.PasteCommand => PasteCommand;

        void IDocumentHandler.MoveToOtherView(BaseDocumentViewModel document)
        {
            var tab = documentsManager.GetTabOf(document);
            switch (tab)
            {
                case DocumentTabKind.Primary:
                    documentsManager.MoveDocumentTo(document, DocumentTabKind.Secondary);
                    break;
                case DocumentTabKind.Secondary:
                    documentsManager.MoveDocumentTo(document, DocumentTabKind.Primary);
                    break;
                default:
                    throw new InvalidEnumArgumentException("Unsupported tab kind!");
            }
            
        }

        void IDocumentHandler.RequestClearAllDiffs()
        {
            ClearAllDiffs();
        }

        bool IDocumentHandler.PerformQuickSearch(string quickSearchText, bool next, bool caseSensitive, bool wholeWord, bool regex)
        {
            return DoPerformQuickSearch(quickSearchText, next, caseSensitive, wholeWord, regex);
        }

        // IToolHandler implementation ----------------------------------------

        void IToolHandler.CloseTools()
        {
            SidePanelPlacement = SidePanelPlacement.Hidden;
        }

        // IExplorerHandler implementation ------------------------------------

        void IExplorerHandler.OpenTextFile(string path)
        {
            LoadTextDocument(documentsManager.ActiveDocumentTab, path);
        }

        void IExplorerHandler.OpenHexFile(string path)
        {
            LoadHexDocument(documentsManager.ActiveDocumentTab, path);
        }

        void IExplorerHandler.Execute(string path)
        {
            platformService.Execute(path);
        }

        string IExplorerHandler.GetCurrentDocumentPath()
        {
            return documentsManager.ActiveDocument.FileName;
        }

        BaseCondition IExplorerHandler.CurrentDocumentHasPathCondition => documentHasPathCondition;

        // IProjectHandler implementation -------------------------------------

        void IProjectHandler.OpenFolderAsProject()
        {
            DoOpenProject();
        }

        void IProjectHandler.TryOpenFile(string path)
        {
            LoadTextDocument(documentsManager.ActiveDocumentTab, path);
        }

        // IBinDefinitionsHandler implementation ------------------------------

        void IBinDefinitionsHandler.OpenTextFile(string path)
        {
            LoadTextDocument(documentsManager.ActiveDocumentTab, path);
        }

        void IBinDefinitionsHandler.NewTextDocument(string text)
        {
            DoNewTextDocument(documentsManager.ActiveDocumentTab, text);
        }

        void IBinDefinitionsHandler.RequestOpenBinFile(BinDefinition binDefinition)
        {
            DoOpenBinDocument(documentsManager.ActiveDocumentTab, binDefinition);
        }

        // IBottomToolsHandler implementation ---------------------------------

        void IBottomToolHandler.CloseBottomTools()
        {
            BottomPanelVisibility = BottomPanelVisibility.Hidden;
        }

        // IMessagesHandler implementation ------------------------------------

        void IMessagesHandler.OpenFileAndFocus(string filename)
        {
            LoadTextDocument(documentsManager.ActiveDocumentTab, filename);
        }

        // IEventListener<StoredSearchesChangedEvent> implementation ----------

        void IEventListener<StoredSearchesChangedEvent>.Receive(StoredSearchesChangedEvent @event)
        {
            BuildStoredSearchReplaceViewModels();
        }

        // ISearchResultsHandler implementation -------------------------------

        void ISearchResultsHandler.OpenFileSearchResult(string fullPath, int line, int column, int length)
        {
            var document = LoadTextDocument(documentsManager.ActiveDocumentTab, fullPath);

            if (document is TextDocumentViewModel textDocument)
            {
                access.WhenUIReady(() =>
                {
                    var offset = textDocument.Document.GetOffset(line, column);
                    textDocument.SetSelection(offset, length, true);
                    textDocument.FocusDocument();
                });
            }
        }

        void ISearchResultsHandler.PerformReplaceInFiles(ReplaceResultsViewModel replaceResults)
        {
            foreach (var item in replaceResults.Results)
            {
                PerformReplaceInFilesRecursive(item);
            }
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
            IPlatformService platformService,
            ITextComparisonService textComparisonService,
            ITextTransformService textTransformService,
            IAppVersionService appVersionService)
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
            this.platformService = platformService;
            this.textComparisonService = textComparisonService;
            this.textTransformService = textTransformService;
            this.appVersionService = appVersionService;

            Title = string.Format(Resources.Strings.MainWindow_Title, appVersionService.GetAppVersion());

            documentsManager = new DocumentsManager();

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
            selectedBottomTool = BottomTool.Messages;
            selectedSideTool = SideTool.Explorer;

            bottomPanelVisibilities = new List<BottomPanelVisibilityModel>
            {
                new BottomPanelVisibilityModel(Strings.Ribbon_View_BottomPanelVisibility_Visible, BottomPanelVisibility.Visible),
                new BottomPanelVisibilityModel(Strings.Ribbon_View_BottomPanelVisibility_Hidden, BottomPanelVisibility.Hidden)
            };

            bottomPanelSize = configurationService.Configuration.UI.BottomPanelSize.Value;

            highlightings = new List<HighlightingInfo>(highlightingProvider.HighlightingDefinitions);
            highlightings.Sort((h1, h2) => h1.Name.CompareTo(h2.Name));

            // Initializing conditions

            documentExistsCondition = new LambdaCondition<DocumentsManager>(documentsManager, dm => dm.ActiveDocument != null, false);
            documentIsTextCondition = new LambdaCondition<DocumentsManager>(documentsManager, dm => dm.ActiveDocument is TextDocumentViewModel, false);
            canUndoCondition = new LambdaCondition<DocumentsManager>(documentsManager, dm => dm.ActiveDocument.CanUndo, false);
            canRedoCondition = new LambdaCondition<DocumentsManager>(documentsManager, dm => dm.ActiveDocument.CanRedo, false);
            canSaveCondition = new LambdaCondition<DocumentsManager>(documentsManager, dm => dm.ActiveDocument.CanSave, false);
            selectionAvailableCondition = new LambdaCondition<DocumentsManager>(documentsManager, dm => dm.ActiveDocument.SelectionAvailable, false);
            regularSelectionAvailableCondition = new LambdaCondition<DocumentsManager>(documentsManager, dm => dm.ActiveDocument.RegularSelectionAvailable, false);
            searchAreaAvailableCondition = new LambdaCondition<DocumentsManager>(documentsManager, dm => (dm.ActiveDocument as TextDocumentViewModel).FindReplaceSegment != null, false);
            selectionAvailableForSearchCondition = regularSelectionAvailableCondition | searchAreaAvailableCondition;
            searchPerformedCondition = new LambdaCondition<DocumentsManager>(documentsManager, dm => dm.ActiveDocument.LastSearch != null, false);
            xmlToolsetAvailableCondition = new LambdaCondition<DocumentsManager>(documentsManager, dm => dm.ActiveDocument.HighlightingToolset == AdditionalToolset.Xml, false);
            markdownToolsetAvailableCondition = new LambdaCondition<DocumentsManager>(documentsManager, dm => dm.ActiveDocument.HighlightingToolset == AdditionalToolset.Markdown, false);
            jsonToolsetAvailableCondition = new LambdaCondition<DocumentsManager>(documentsManager, dm => dm.ActiveDocument.HighlightingToolset == AdditionalToolset.Json, false);
            documentPathVirtualCondition = new LambdaCondition<DocumentsManager>(documentsManager, dm => dm.ActiveDocument.FilenameVirtual, true);
            documentHasPathCondition = documentExistsCondition & !documentPathVirtualCondition;
            diffDataAvailableCondition = new LambdaCondition<DocumentsManager>(documentsManager, dm => (dm.ActiveDocument as TextDocumentViewModel).DiffResult != null, false);

            // Initializing tools

            explorerToolViewModel = new ExplorerToolViewModel(fileIconProvider, 
                imageResources, 
                configurationService, 
                this, 
                eventBus, 
                platformService);
            projectToolViewModel = new ProjectToolViewModel(this,
                this,
                eventBus,
                imageResources,
                fileIconProvider);
            binDefinitionsToolViewModel = new BinDefinitionsToolViewModel(this,
                imageResources,
                configurationService,
                dialogService,
                messagingService,
                pathService);

            // Initializeing bottom tools

            messagesBottomToolViewModel = new MessagesBottomToolViewModel(this, imageResources);
            searchResultsBottomToolViewModel = new SearchResultsBottomToolViewModel(this, imageResources);

            // Initializing commands

            // Some commands should not be registered in command repository service
            NavigateCommand = new AppCommand(obj => DoNavigate());
            NewPrimaryTextCommand = new AppCommand(obj => DoNewPrimaryText());
            NewSecondaryTextCommand = new AppCommand(obj => DoNewSecondaryText());

            ConfigCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Configuration, "Settings16.png", obj => DoOpenConfiguration());
            RunStoredSearchCommand = new AppCommand(obj => DoRunStoredSearch(obj as StoredSearchReplace), documentIsTextCondition);

            NewTextCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Home_File_New, "New16.png", obj => DoNewTextDocument(documentsManager.ActiveDocumentTab));
            NewHexCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Home_File_NewHex, "New16.png", obj => DoNewHexDocument(documentsManager.ActiveDocumentTab));
            OpenTextCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Home_File_Open, "Open16.png", obj => DoOpenTextDocument(documentsManager.ActiveDocumentTab));
            OpenHexCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Home_File_OpenHex, "Open16.png", obj => DoOpenHexDocument(documentsManager.ActiveDocumentTab));
            OpenBinCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Home_File_OpenBin, "Open16.png", obj => DoOpenBinDocument());
            SaveCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Home_File_Save, "Save16.png", obj => DoSaveDocument(), canSaveCondition);
            SaveAsCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Home_File_SaveAs, "Save16.png", obj => DoSaveDocumentAs(), canSaveCondition);
            CloseCurrentDocumentCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Command_CloseCurrentDocument, null, obj => DoCloseCurrentDocument(), documentExistsCondition);

            UndoCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Home_Edit_Undo, "Undo16.png", obj => DoUndo(), canUndoCondition);
            RedoCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Home_Edit_Redo, "Redo16.png", obj => DoRedo(), canRedoCondition);
            CopyCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Home_Edit_Copy, "Copy16.png", obj => DoCopy(), selectionAvailableCondition);
            CutCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Home_Edit_Cut, "Cut16.png", obj => DoCut(), selectionAvailableCondition);
            PasteCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Home_Edit_Paste, "Paste16.png", obj => DoPaste(), documentExistsCondition);

            SearchCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Home_Search_Search, "Search16.png", obj => DoSearch(), documentIsTextCondition);
            ReplaceCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Home_Search_Replace, "Replace16.png", obj => DoReplace(), documentIsTextCondition);
            FindNextCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Home_Search_FindNext, "FindNext16.png", obj => DoFindNext(), documentIsTextCondition & searchPerformedCondition);
            FindInFilesCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Home_Search_FindInFiles, "Search16.png", obj => DoFindInFiles());
            ReplaceInFilesCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Home_Search_ReplaceInFiles, "Replace16.png", obj => DoReplaceInFiles());

            OpenProjectCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Home_Project_Open, "Open16.png", obj => DoOpenProject());

            SortLinesAscendingCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Lines_Ordering_SortAscending, "SortAscending16.png", obj => DoSortAscending(), documentIsTextCondition);
            SortLinesDescendingCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Lines_Ordering_SortDescending, "SortDescending16.png", obj => DoSortDescending(), documentIsTextCondition);
            RemoveEmptyLinesCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Lines_Cleanup_RemoveEmptyLines, "DeleteLine16.png", obj => DoRemoveEmptyLines(), documentIsTextCondition);
            RemoveWhitespaceLinesCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Lines_Cleanup_RemoveWhitespaceLines, "DeleteLine16.png", obj => DoRemoveWhitespaceLines(), documentIsTextCondition);
            FindDuplicatedLinesCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Lines_Tools_FindDuplicatedLines, "Duplicates16.png", obj => DoFindDuplicatedLines());

            LowercaseCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Text_Transform_Case_Lowercase, "Case16.png", obj => DoLowercase(), documentIsTextCondition);
            UppercaseCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Text_Transform_Case_Uppercase, "Case16.png", obj => DoUppercase(), documentIsTextCondition);
            NamingCaseCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Text_Transform_Case_Naming, "Case16.png", obj => DoNamingCase(), documentIsTextCondition);
            SentenceCaseCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Text_Transform_Case_Sentence, "Case16.png", obj => DoSentenceCase(), documentIsTextCondition);
            InvertCaseCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Text_Transform_Case_Invert, "Case16.png", obj => DoInvertCase(), documentIsTextCondition);

            Base64EncodeCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Text_Convert_Base64_ToBase64, "ConvertBase6416.png", obj => DoBase64Encode(), documentIsTextCondition);
            Base64DecodeCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Text_Convert_Base64_FromBase64, "ConvertBase6416.png", obj => DoBase64Decode(), documentIsTextCondition);
            HtmlEntitiesEncodeCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Text_Escape_Htm_Encode, "ConvertHtml16.png", obj => DoHtmlEntitiesEncode(), documentIsTextCondition);
            HtmlEntitiesDecodeCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Text_Escape_Htm_Decode, "ConvertHtml16.png", obj => DoHtmlEntitiesDecode(), documentIsTextCondition);
            EscapeCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Text_Escape_Escape, null, obj => DoEscape(obj), documentIsTextCondition);
            UnescapeCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Text_Escape_Unescape, null, obj => DoUnescape(obj), documentIsTextCondition);
            OpenSubstitutionCipherCommand = commandRepositoryService.RegisterCommand("#Substitution cipher", null, obj => DoOpenSubstitutionCipher());

            FormatXmlCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_XmlTools_Formatting_Format, "FormatXml16.png", obj => DoFormatXml(), xmlToolsetAvailableCondition);
            TransformXsltCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_XmlTools_Transform_XSLT, null, obj => DoTransformXslt(), xmlToolsetAvailableCondition);

            MarkdownHtmlPreviewCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Command_Markdown_HtmlPreview, "Search16.png", obj => DoMarkdownHtmlPreview(), markdownToolsetAvailableCondition);
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

            FormatJsonCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_JsonTools_Formatting_Format, "FormatXml16.png", obj => DoFormatJson(), jsonToolsetAvailableCondition);

            CompareCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Home_Comparing_Compare, "Compare16.png", obj => DoCompare());
            PreviousChangeCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Home_Comparing_PreviousChange, "Up16.png", obj => DoPreviousChange(), diffDataAvailableCondition);
            NextChangeCommand = commandRepositoryService.RegisterCommand(Resources.Strings.Ribbon_Home_Comparing_NextChange, "Down16.png", obj => DoNextChange(), diffDataAvailableCondition);

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

            // Events

            eventBus.Register<StoredSearchesChangedEvent>(this);

            // Applying current close behavior

            if (configurationService.Configuration.Behavior.CloseBehavior.Value == CloseBehavior.Fluent)
            {
                RestoreFiles();

                OpenParameters();

                if (!documentsManager.AllDocuments.Any())
                    DoNewTextDocument(DocumentTabKind.Primary);
            }
            else if (configurationService.Configuration.Behavior.CloseBehavior.Value == CloseBehavior.Standard)
            {
                if (!OpenParameters())
                    DoNewTextDocument(DocumentTabKind.Primary);
            }
            else
                throw new InvalidOperationException("Invalid close behavior!");
        }

        public void NotifyActivated()
        {
            VerifyRemovedAndModifiedDocuments();
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

        public void RequestCloseDocument(BaseDocumentViewModel document)
        {
            DoTryCloseDocument(document);
        }

        public bool CanCloseApplication()
        {
            switch (configurationService.Configuration.Behavior.CloseBehavior.Value)
            {
                case CloseBehavior.Standard:
                    {
                        if (!CloseDocumentsWhere(d => true))
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

        public void SelectPreviousNavigationItem()
        {
            if (selectedNavigationItem != null)
            {
                int index = navigationItems.IndexOf(selectedNavigationItem);
                if (index > 0)
                    SelectedNavigationItem = navigationItems[index - 1];
            }
            else
            {
                SelectedNavigationItem = navigationItems.LastOrDefault();
            }

            access.EnsureSelectedNavigationItemVisible();
        }

        public void SelectNextNavigationItem()
        {
            if (selectedNavigationItem != null)
            {
                int index = navigationItems.IndexOf(selectedNavigationItem);
                if (index < navigationItems.Count - 1)
                    SelectedNavigationItem = navigationItems[index + 1];
            }
            else
            {
                SelectedNavigationItem = navigationItems.FirstOrDefault();
            }

            access.EnsureSelectedNavigationItemVisible();
        }

        public void NotifyFilesDropped(string[] files)
        {
            foreach (var file in files)
            {
                LoadTextDocument(documentsManager.ActiveDocumentTab, file);
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
            documentsManager.ReorderDocument(fromDoc, toDoc);
        }

        public void MoveDocumentTo(BaseDocumentViewModel documentViewModel, ITabDocumentCollection<BaseDocumentViewModel> destinationDocuments)
        {
            documentsManager.MoveDocumentTo(documentViewModel, destinationDocuments.DocumentTabKind);
        }

        // Public properties --------------------------------------------------

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

        public BottomTool SelectedBottomTool
        {
            get => selectedBottomTool;
            set => Set(ref selectedBottomTool, () => SelectedBottomTool, value);
        }

        public SideTool SelectedSideTool
        {
            get => selectedSideTool;
            set => Set(ref selectedSideTool, () => SelectedSideTool, value);
        }

        public ExplorerToolViewModel ExplorerToolViewModel => explorerToolViewModel;

        public ProjectToolViewModel ProjectToolViewModel => projectToolViewModel;

        public BinDefinitionsToolViewModel BinDefinitionsToolViewModel => binDefinitionsToolViewModel;

        public MessagesBottomToolViewModel MessagesBottomToolViewModel => messagesBottomToolViewModel;

        public SearchResultsBottomToolViewModel SearchResultsBottomToolViewModel => searchResultsBottomToolViewModel;

        public ObservableCollection<StoredSearchReplaceViewModel> StoredSearches => storedSearches;

        public ObservableCollection<StoredSearchReplaceViewModel> StoredReplaces => storedReplaces;

        public DocumentsManager Documents => documentsManager;

        bool IDocumentHandler.WordWrap => throw new NotImplementedException();

        bool IDocumentHandler.LineNumbers => throw new NotImplementedException();

        public string Title { get; }
    }
}
