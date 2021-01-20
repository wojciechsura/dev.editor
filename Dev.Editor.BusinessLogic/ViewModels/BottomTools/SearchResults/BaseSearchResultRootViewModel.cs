using Dev.Editor.BusinessLogic.Models.SearchResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.ViewModels.BottomTools.SearchResults
{
    public abstract class BaseSearchResultRootViewModel : BaseSearchResultViewModel
    {
        public abstract void ApplyFilter(SearchResultFilterModel model);
        public abstract void ClearFilter();

        public abstract bool IsFiltered { get; set; }
    }
}
