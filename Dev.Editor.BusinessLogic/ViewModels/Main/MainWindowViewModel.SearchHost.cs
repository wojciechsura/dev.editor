using Dev.Editor.BusinessLogic.Models.Search;
using Dev.Editor.BusinessLogic.ViewModels.Search;
using Dev.Editor.Common.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Dev.Editor.BusinessLogic.ViewModels.Document;
using Dev.Editor.Resources;
using System.Collections.ObjectModel;
using ICSharpCode.AvalonEdit.Document;
using Dev.Editor.BusinessLogic.ViewModels.Main.Search;
using System.ComponentModel;
using Dev.Editor.BusinessLogic.Types.BottomTools;
using Dev.Editor.BusinessLogic.Types.UI;
using Dev.Editor.BusinessLogic.Models.FindInFiles;
using Dev.Editor.BusinessLogic.ViewModels.FindInFiles;

namespace Dev.Editor.BusinessLogic.ViewModels.Main
{
    public partial class MainWindowViewModel : ISearchHost
    {
        BaseCondition ISearchHost.CanSearchCondition => documentExistsCondition;

        BaseCondition ISearchHost.SelectionAvailableCondition => selectionAvailableForSearchCondition;

        void ISearchHost.SetFindReplaceSegmentToSelection(bool searchBackwards)
        {
            var document = (TextDocumentViewModel)documentsManager.ActiveDocument;

            if (document.FindReplaceSegment == null)
            {
                var selection = document.GetSelection();

                document.FindReplaceSegment = new AnchorSegment(document.Document, selection.selStart, selection.selLength);
                if (searchBackwards)
                {
                    document.SetSelection(selection.selStart + selection.selLength, 0);
                }
                else
                {
                    document.SetSelection(selection.selStart, 0);
                }
            }
        }

        void ISearchHost.ClearFindReplaceSegment()
        {
            var document = (TextDocumentViewModel)documentsManager.ActiveDocument;

            document.FindReplaceSegment = null;
        }

        private void InternalFindNext(SearchReplaceModel searchModel)
        {
            var document = (TextDocumentViewModel)documentsManager.ActiveDocument;

            bool MatchWithinBounds(Match m)
            {
                if (document.FindReplaceSegment != null)
                    return m.Index >= document.FindReplaceSegment.Offset && m.Index + m.Length <= document.FindReplaceSegment.EndOffset;
                else
                    return true;
            }

            // Mark search as performed at least once
            searchModel.SearchPerformed = true;

            // Search origin
            (int selStart, int selLength) = document.GetSelection();

            int start;
            if (searchModel.SearchBackwards)
            {
                start = selStart;
            }
            else
            {
                start = selStart + selLength;
            }

            Match match = searchModel.Regex.Match(document.Document.Text, start);

            if (!match.Success || !MatchWithinBounds(match))  // start again from beginning or end
            {
                bool continueFromBoundary;

                if (searchModel.SearchBackwards)
                    continueFromBoundary = messagingService.AskYesNo(Strings.Message_SearchReachedBeginning);
                else
                    continueFromBoundary = messagingService.AskYesNo(Strings.Message_SearchReachedEnd);

                if (continueFromBoundary)
                {
                    searchModel.SearchedFromBoundary = true;

                    if (searchModel.SearchBackwards)
                    {
                        if (document.FindReplaceSegment != null)
                            match = searchModel.Regex.Match(document.Document.Text, document.FindReplaceSegment.EndOffset);
                        else
                            match = searchModel.Regex.Match(document.Document.Text, document.Document.Text.Length);
                    }
                    else
                    {
                        if (document.FindReplaceSegment != null)
                            match = searchModel.Regex.Match(document.Document.Text, document.FindReplaceSegment.Offset);
                        else
                            match = searchModel.Regex.Match(document.Document.Text, 0);
                    }
                }
            }

            if (match.Success && MatchWithinBounds(match))
            {
                document.SetSelection(match.Index, match.Length, true);
            }
            else
            {
                messagingService.Inform(Resources.Strings.Message_NoMorePatternsFound);
            }
        }

        public void FindNext(SearchReplaceModel searchModel)
        {
            documentsManager.ActiveDocument.LastSearch = searchModel;
            InternalFindNext(searchModel);
        }

        public void Replace(SearchReplaceModel replaceModel)
        {
            var document = (TextDocumentViewModel)documentsManager.ActiveDocument;

            string input = document.GetSelectedText();

            Match match = replaceModel.Regex.Match(input);

            if (match.Success && match.Index == 0 && match.Length == input.Length)
            {
                (int selStart, int selLength) = document.GetSelection();

                if (replaceModel.IsRegexReplace)
                {
                    var newReplace = replaceModel.Regex.Replace(match.Value, replaceModel.Replace);
                    document.Document.Replace(selStart, selLength, newReplace);
                }
                else
                {
                    document.Document.Replace(selStart, selLength, replaceModel.Replace);
                }
            }

            FindNext(replaceModel);
        }

        (int selStart, int selLen) GetSelectionBlock(TextDocumentViewModel document)
        {
            if (document.FindReplaceSegment != null)
                return (document.FindReplaceSegment.Offset, document.FindReplaceSegment.Length);
            else
                return document.GetSelection();
        }

        public void ReplaceAll(SearchReplaceModel replaceModel)
        {
            var document = (TextDocumentViewModel)documentsManager.ActiveDocument;

            document.LastSearch = replaceModel;

            int replaceCount = 0;

            if (!replaceModel.InSelection)
            {
                int offset = 0;
                
                replaceCount = document.RunAsSingleHistoryEntry(() =>
                {
                    MatchCollection matches = replaceModel.Regex.Matches(document.Document.Text);

                    foreach (Match match in matches)
                    {
                        if (replaceModel.IsRegexReplace)
                        {
                            var newReplace = replaceModel.Regex.Replace(match.Value, replaceModel.Replace);
                            document.Document.Replace(offset + match.Index, match.Length, newReplace);
                            offset += newReplace.Length - match.Length;
                        }
                        else
                        {
                            document.Document.Replace(offset + match.Index, match.Length, replaceModel.Replace);
                            offset += replaceModel.Replace.Length - match.Length;
                        }
                    }

                    return matches.Count;
                });
            }
            else
            {
                (int selStart, int selLen) = GetSelectionBlock(document);
               
                string selection = document.Document.GetText(selStart, selLen);

                int offset = 0;

                replaceCount = document.RunAsSingleHistoryEntry(() =>
                {
                    MatchCollection matches = replaceModel.Regex.Matches(selection);                    

                    foreach (Match match in matches)
                    {
                        document.Document.Replace(offset + selStart + match.Index, match.Length, replaceModel.Replace);
                        offset += replaceModel.Replace.Length - match.Length;
                    }

                    return matches.Count;
                });
            }

            if (replaceCount > 0)
            {
                if (replaceModel.ShowReplaceSummary)
                    messagingService.Inform(String.Format(Strings.Message_ReplacedOccurrences, replaceCount));
            }
            else
            {
                messagingService.Inform(Strings.Message_NoMorePatternsFound);
            }
        }

        public void CountOccurrences(SearchReplaceModel searchModel)
        {
            var document = (TextDocumentViewModel)documentsManager.ActiveDocument;

            (int selStart, int selLength) = GetSelectionBlock(document);

            string textToSearch;
            
            if (searchModel.InSelection && selLength > 0)
            {
                textToSearch = document.Document.GetText(selStart, selLength);
            }
            else
            {
                textToSearch = document.Document.Text;
            }

            var matches = searchModel.CountOccurrencesRegex.Matches(textToSearch, 0);

            if (matches.Count == 0)
                messagingService.Inform(Resources.Strings.Message_NoMorePatternsFound);
            else
                messagingService.Inform(String.Format(Resources.Strings.Message_FoundOccurrences, matches.Count));
        }

        public void FindInFiles(SearchReplaceModel searchReplaceModel)
        {
            var worker = new FindInFilesWorker(searchReplaceModel);
            worker.RunWorkerCompleted += HandleFindInFilesCompleted;

            dialogService.ShowProgressDialog(Strings.Operation_SearchingInFiles, worker);
        }

        private void HandleFindInFilesCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            List<BaseSearchResultViewModel> GenerateResultList(BaseSearchContainerItem container)
            {
                var results = new List<BaseSearchResultViewModel>();

                foreach (var item in container)
                {
                    var builtItem = BuildItemRecursive(item);
                    if (builtItem != null)
                        results.Add(builtItem);
                }

                return results;
            }

            BaseSearchResultViewModel BuildItemRecursive(BaseSearchItem item)
            {
                if (item is FolderSearchItem searchedFolder)
                {
                    List<BaseSearchResultViewModel> results = GenerateResultList(searchedFolder);

                    if (results.Any())
                        return new FolderSearchResultViewModel(System.IO.Path.GetFileName(searchedFolder.Path), fileIconProvider.GetImageForFolder(searchedFolder.Path), results);
                    else
                        return null;
                }
                else if (item is FileSearchItem searchedFile)
                {
                    if (searchedFile.Any())
                    {
                        var resultList = new List<SearchResultViewModel>();
                        foreach (var searchResult in searchedFile)
                        {
                            var resultViewModel = new SearchResultViewModel(searchResult.FullPath,
                                searchResult.Before, 
                                searchResult.Match, 
                                searchResult.After, 
                                searchResult.Row + 1, 
                                searchResult.Col + 1);

                            resultList.Add(resultViewModel);
                        }

                        return new FileSearchResultViewModel(System.IO.Path.GetFileName(searchedFile.Path), fileIconProvider.GetImageForFile(searchedFile.Path), resultList);
                    }
                    else
                        return null;
                }
                else
                    throw new ArgumentException("Unsupported search item!");
            }

            RootSearchResultViewModel BuildResults(RootSearchItem root)
            {
                List<BaseSearchResultViewModel> results = GenerateResultList(root);

                return new RootSearchResultViewModel(root.Path, root.SearchPattern, imageResources.GetIconByName("Search16.png"), results);
            }

            var result = e.Result as RootSearchItem;
            var searchResults = BuildResults(result);

            if (result != null)
            {
                SearchResultsBottomToolViewModel.AddResults(searchResults);

                BottomPanelVisibility = BottomPanelVisibility.Visible;
                SelectedBottomTool = BottomTool.SearchResults;
            }
        }
    }
}
