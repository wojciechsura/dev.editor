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

        private BaseSearchResultViewModel searchResults;

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

        public SearchResultsBottomToolViewModel(ISearchResultsHandler searchResultsHandler, IImageResources imageResources)
            : base(searchResultsHandler)
        {
            this.searchResultsHandler = searchResultsHandler;
            this.imageResources = imageResources;

            searchResults = null;
            icon = imageResources.GetIconByName("Search16.png");

            resultsNonEmptyCondition = new LambdaCondition<SearchResultsBottomToolViewModel>(this, vm => vm.SearchResults != null);
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

        public void SetResults(BaseSearchResultViewModel results)
        {
            this.searchResults = results;
            OnPropertyChanged(() => SearchResults);
        }

        public override string Title => Strings.BottomTool_SearchResults_Title;

        public override ImageSource Icon => icon;

        public override string Uid => SearchResultsUid;

        public IEnumerable<BaseSearchResultViewModel> SearchResults
        {
            get 
            { 
                yield return searchResults; 
            }
        }

        public ICommand ClearSearchResultsCommand { get; }

        public ICommand PerformReplaceCommand { get; }
    }
}
