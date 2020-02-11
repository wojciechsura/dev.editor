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
        private StoredSearchReplace storedSearch;

        public StoredSearchViewModel(StoredSearchReplace storedSearch)
        {
            this.storedSearch = storedSearch;
        }

        public string SearchName => storedSearch.SearchName.Value;
        public SearchReplaceOperation Operation => storedSearch.Operation.Value;
    }
}
