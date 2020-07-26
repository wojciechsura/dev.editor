using Dev.Editor.BusinessLogic.ViewModels.BottomTools.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Dev.Editor.Resources;
using Dev.Editor.BusinessLogic.Services.ImageResources;

namespace Dev.Editor.BusinessLogic.ViewModels.BottomTools.SearchResults
{
    public class SearchResultsBottomToolViewModel : BaseBottomToolViewModel
    {
        private readonly ISearchResultsHandler searchResultsHandler;
        private readonly IImageResources imageResources;

        private readonly ImageSource icon;

        public SearchResultsBottomToolViewModel(ISearchResultsHandler searchResultsHandler, IImageResources imageResources)
            : base(searchResultsHandler)
        {
            this.searchResultsHandler = searchResultsHandler;
            this.imageResources = imageResources;

            icon = imageResources.GetIconByName("Search16.png");
        }

        public override string Title => Strings.BottomTool_SearchResults_Title;

        public override ImageSource Icon => icon;

        public override string Uid => SearchResultsUid;
    }
}
