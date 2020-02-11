using Dev.Editor.BusinessLogic.Models.Configuration.Search;
using Dev.Editor.BusinessLogic.Types.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Dev.Editor.BusinessLogic.ViewModels.Search
{
    public class StoredSearchViewModel
    {
        public StoredSearchViewModel(StoredSearchReplace storedSearch)
        {
            this.StoredSearch = storedSearch;
        }

        public string SearchName => StoredSearch.SearchName.Value;
        public SearchReplaceOperation Operation => StoredSearch.Operation.Value;

        public StoredSearchReplace StoredSearch { get; }
    }
}
