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
using Dev.Editor.Common.Conditions;
using System.Windows.Input;
using Dev.Editor.Common.Commands;

namespace Dev.Editor.BusinessLogic.ViewModels.BottomTools.SearchResults
{
    public class SearchResultsBottomToolViewModel : BaseBottomToolViewModel
    {
        private readonly ISearchResultsHandler searchResultsHandler;
        private readonly IImageResources imageResources;

        private readonly ImageSource icon;
        private readonly ObservableCollection<RootSearchResultViewModel> searchResults = new ObservableCollection<RootSearchResultViewModel>();

        private readonly Condition resultsNonEmptyCondition;

        private void DoClearSearchResults()
        {
            searchResults.Clear();
            resultsNonEmptyCondition.Value = false;
        }

        public SearchResultsBottomToolViewModel(ISearchResultsHandler searchResultsHandler, IImageResources imageResources)
            : base(searchResultsHandler)
        {
            this.searchResultsHandler = searchResultsHandler;
            this.imageResources = imageResources;

            icon = imageResources.GetIconByName("Search16.png");

            resultsNonEmptyCondition = new Condition(false);

            ClearSearchResultsCommand = new AppCommand(obj => DoClearSearchResults(), resultsNonEmptyCondition);
        }

        public void NotifyItemDoubleClicked(object selectedResult)
        {
            if (selectedResult is SearchResultViewModel searchResult)
            {
                searchResultsHandler.OpenFileSearchResult(searchResult.FullPath, searchResult.Line, searchResult.Column, searchResult.Length);
            }
        }

        public void AddResults(RootSearchResultViewModel results, bool clearExisting = true)
        {
            if (clearExisting)
                searchResults.Clear();

            searchResults.Insert(0, results);

            resultsNonEmptyCondition.Value = true;
        }

        public override string Title => Strings.BottomTool_SearchResults_Title;

        public override ImageSource Icon => icon;

        public override string Uid => SearchResultsUid;

        public ObservableCollection<RootSearchResultViewModel> SearchResults => searchResults;

        public ICommand ClearSearchResultsCommand { get; }
    }
}
