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

namespace Dev.Editor.BusinessLogic.ViewModels.BottomTools.SearchResults
{
    public class SearchResultsBottomToolViewModel : BaseBottomToolViewModel
    {
        private readonly ISearchResultsHandler searchResultsHandler;
        private readonly IImageResources imageResources;

        private readonly ImageSource icon;

        private readonly BaseCondition resultsNonEmptyCondition;
        private readonly BaseCondition resultsAreReplaceCondition;

        private readonly ObservableCollection<BaseSearchResultRootViewModel> searchResults;
        private string filter;
        private bool filterCaseSensitive;
        private bool filterExcludes;

        private void DoClearSearchResults()
        {
            searchResults.Clear();
        }

        private void DoPerformReplace()
        {
            var replaceResults = searchResults.OfType<ReplaceResultsViewModel>().ToList();

            foreach (var result in replaceResults)
            {
                searchResultsHandler.PerformReplaceInFiles((ReplaceResultsViewModel)result);
                searchResults.Remove(result);
            }
        }

        private void HandleFilterChanged()
        {
            if (SearchResults.Any())
                foreach (var result in SearchResults)
                {
                    if (!string.IsNullOrEmpty(filter))
                        result.ApplyFilter(filter, filterCaseSensitive, filterExcludes);
                    else
                        result.ClearFilter();
                }
        }

        public SearchResultsBottomToolViewModel(ISearchResultsHandler searchResultsHandler, IImageResources imageResources)
            : base(searchResultsHandler)
        {
            this.searchResultsHandler = searchResultsHandler;
            this.imageResources = imageResources;

            searchResults = new ObservableCollection<BaseSearchResultRootViewModel>();
            icon = imageResources.GetIconByName("Search16.png");

            resultsNonEmptyCondition = new LambdaCondition<SearchResultsBottomToolViewModel>(this, vm => vm.SearchResults.Any());
            resultsAreReplaceCondition = new LambdaCondition<SearchResultsBottomToolViewModel>(this, vm => vm.SearchResults.FirstOrDefault() is ReplaceResultsViewModel);

            ClearSearchResultsCommand = new AppCommand(obj => DoClearSearchResults(), resultsNonEmptyCondition);
            PerformReplaceCommand = new AppCommand(obj => DoPerformReplace(), resultsAreReplaceCondition);
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
            searchResults.Clear();
            Filter = null;

            searchResults.Add(results);
            OnPropertyChanged(() => SearchResults);
        }

        public override string Title => Strings.BottomTool_SearchResults_Title;

        public override ImageSource Icon => icon;

        public override string Uid => SearchResultsUid;

        public ObservableCollection<BaseSearchResultRootViewModel> SearchResults
        {
            get 
            {
                return searchResults;
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

        public ICommand ClearSearchResultsCommand { get; }

        public ICommand PerformReplaceCommand { get; }
    }
}
