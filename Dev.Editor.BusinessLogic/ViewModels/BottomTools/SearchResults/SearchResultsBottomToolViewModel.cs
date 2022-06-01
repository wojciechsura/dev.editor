using Dev.Editor.BusinessLogic.ViewModels.BottomTools.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Dev.Editor.Resources;
using Dev.Editor.BusinessLogic.Services.ImageResources;
using Dev.Editor.BusinessLogic.ViewModels.Search;
using System.Collections.ObjectModel;
using Dev.Editor.BusinessLogic.ViewModels.FindInFiles;
using Spooksoft.VisualStateManager.Conditions;
using System.Windows.Input;
using Spooksoft.VisualStateManager.Commands;
using Dev.Editor.BusinessLogic.ViewModels.DuplicatedLines;
using Dev.Editor.BusinessLogic.Models.SearchResults;

namespace Dev.Editor.BusinessLogic.ViewModels.BottomTools.SearchResults
{
    public class SearchResultsBottomToolViewModel : BaseBottomToolViewModel
    {
        private readonly ISearchResultsHandler searchResultsHandler;
        private readonly IImageResources imageResources;

        private readonly ImageSource icon;

        private readonly BaseCondition resultsNonEmptyCondition;
        private readonly BaseCondition resultsAreReplaceCondition;
        private readonly BaseCondition resultsAreSearchCondition;
        private readonly BaseCondition resultsCanFilterContents;

        private BaseSearchResultRootViewModel searchResults;
        private string filter;
        private bool filterCaseSensitive;
        private bool filterExcludes;
        private bool filterFiles;
        private bool filterContents;

        private void DoClearSearchResults()
        {
            searchResults = null;
            OnPropertyChanged(() => SearchResults);
        }

        private void DoPerformReplace()
        {
            searchResultsHandler.PerformReplaceInFiles((ReplaceResultsViewModel)searchResults);
            searchResults = null;
            OnPropertyChanged(() => SearchResults);
        }

        private void HandleFilterChanged()
        {
            if (searchResults != null)
            {
                if (!string.IsNullOrEmpty(filter))
                    searchResults.ApplyFilter(new SearchResultFilterModel(filter, filterCaseSensitive, filterExcludes, filterFiles, filterContents));
                else
                    searchResults.ClearFilter();
            }
        }

        private void CollectResultsRecursive(List<BaseFilesystemSearchResultViewModel> items, List<string> strings)
        {
            foreach (var item in items)
            {
                if (item is FileSearchResultViewModel file)
                {
                    foreach (var result in file.Results)
                    {
                        strings.Add(result.Match);
                    }
                }
                else if (item is FolderSearchResultViewModel folder)
                {
                    CollectResultsRecursive(folder.Files, strings);
                }
            }
        }

        private void DoExportResultsToDocument()
        {
            var results = this.searchResults as SearchResultsViewModel;

            List<string> strings = new List<string>();
            CollectResultsRecursive(results.Results, strings);

            searchResultsHandler.CreateNewDocument(strings);
        }

        public SearchResultsBottomToolViewModel(ISearchResultsHandler searchResultsHandler, IImageResources imageResources)
            : base(searchResultsHandler)
        {
            this.searchResultsHandler = searchResultsHandler;
            this.imageResources = imageResources;

            filter = null;
            filterCaseSensitive = false;
            filterExcludes = false;
            filterFiles = true;
            filterContents = true;

            searchResults = null;
            icon = imageResources.GetIconByName("Search16.png");

            resultsNonEmptyCondition = new ChainedLambdaCondition<SearchResultsBottomToolViewModel>(this, vm => vm.SearchResults.Single() != null, false);
            resultsAreReplaceCondition = new ChainedLambdaCondition<SearchResultsBottomToolViewModel>(this, vm => vm.SearchResults.SingleOrDefault() is ReplaceResultsViewModel, false);
            resultsAreSearchCondition = new ChainedLambdaCondition<SearchResultsBottomToolViewModel>(this, vm => vm.SearchResults.SingleOrDefault() is SearchResultsViewModel, false);

            resultsCanFilterContents = new ChainedLambdaCondition<SearchResultsBottomToolViewModel>(this, vm => vm.SearchResults.SingleOrDefault() is DuplicatedLinesResultViewModel, false);
            resultsCanFilterContents.ValueChanged += (s, e) => OnPropertyChanged(() => CanFilterContents);

            ClearSearchResultsCommand = new AppCommand(obj => DoClearSearchResults(), resultsNonEmptyCondition);
            PerformReplaceCommand = new AppCommand(obj => DoPerformReplace(), resultsAreReplaceCondition);
            ExportResultsToDocumentCommand = new AppCommand(obj => DoExportResultsToDocument(), resultsNonEmptyCondition & resultsAreSearchCondition);
        }

        public void NotifyItemDoubleClicked(object selectedResult)
        {
            if (selectedResult is SearchResultViewModel searchResult)
            {
                searchResultsHandler.OpenFileSearchResult(searchResult.FullPath, searchResult.Line, searchResult.Column, searchResult.Length);
            }
            else if (selectedResult is FileReferenceViewModel fileRef)
            {
                searchResultsHandler.OpenFileSearchResult(fileRef.Path, fileRef.StartLine, 0, 0);
            }
        }

        public void SetResults(BaseSearchResultRootViewModel results)
        {
            // Setting search results to null to prevent Filter to fire
            searchResults = null;
            Filter = null;

            searchResults = results;
            OnPropertyChanged(() => SearchResults);
        }

        public override string Title => Strings.BottomTool_SearchResults_Title;

        public override ImageSource Icon => icon;

        public override string Uid => SearchResultsUid;

        public IEnumerable<BaseSearchResultRootViewModel> SearchResults
        {
            get 
            {
                yield return searchResults;
            }
        }

        public string Filter
        {
            get => filter;
            set => Set(ref filter, () => Filter, value, () => HandleFilterChanged());
        }

        public bool FilterCaseSensitive
        {
            get => filterCaseSensitive;
            set => Set(ref filterCaseSensitive, () => FilterCaseSensitive, value, () => HandleFilterChanged());
        }

        public bool FilterExcludes
        {
            get => filterExcludes;
            set => Set(ref filterExcludes, () => FilterExcludes, value, () => HandleFilterChanged());
        }

        public bool FilterFiles
        {
            get => filterFiles;
            set => Set(ref filterFiles, () => FilterFiles, value, () => HandleFilterChanged());
        }

        public bool FilterContents
        {
            get => filterContents;
            set => Set(ref filterContents, () => FilterContents, value, () => HandleFilterChanged());
        }

        public ICommand ClearSearchResultsCommand { get; }

        public ICommand PerformReplaceCommand { get; }

        public ICommand ExportResultsToDocumentCommand { get; }

        public bool CanFilterContents => resultsCanFilterContents.GetValue();
    }
}
