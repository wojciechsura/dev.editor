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

namespace Dev.Editor.BusinessLogic.ViewModels.BottomTools.SearchResults
{
    public class SearchResultsBottomToolViewModel : BaseBottomToolViewModel
    {
        private readonly ISearchResultsHandler searchResultsHandler;
        private readonly IImageResources imageResources;

        private readonly ImageSource icon;
        private readonly ObservableCollection<RootSearchResultViewModel> searchResults = new ObservableCollection<RootSearchResultViewModel>();

        public SearchResultsBottomToolViewModel(ISearchResultsHandler searchResultsHandler, IImageResources imageResources)
            : base(searchResultsHandler)
        {
            this.searchResultsHandler = searchResultsHandler;
            this.imageResources = imageResources;

            icon = imageResources.GetIconByName("Search16.png");
        }

        public void NotifyItemDoubleClicked(object selectedResult)
        {
            if (selectedResult is SearchResultViewModel searchResult)
            {
                searchResultsHandler.OpenFileSearchResult(searchResult.FullPath, searchResult.Line, searchResult.Column);
            }
        }

        public void AddResults(RootSearchResultViewModel results, bool clearExisting = true)
        {
            if (clearExisting)
                searchResults.Clear();

            searchResults.Insert(0, results);
        }

        public override string Title => Strings.BottomTool_SearchResults_Title;

        public override ImageSource Icon => icon;

        public override string Uid => SearchResultsUid;

        public ObservableCollection<RootSearchResultViewModel> SearchResults => searchResults;
    }
}
