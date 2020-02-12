using Dev.Editor.BusinessLogic.Models.Configuration.Search;
using Dev.Editor.BusinessLogic.Types.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dev.Editor.BusinessLogic.ViewModels.Main
{
    public class StoredSearchReplaceViewModel
    {
        public StoredSearchReplaceViewModel(StoredSearchReplace model, ICommand runStoredSearchCommand)
        {
            this.StoredSearchReplace = model;
            RunStoredSearchCommand = runStoredSearchCommand;
        }

        public string SearchName => StoredSearchReplace.SearchName.Value;
        public SearchReplaceOperation Operation => StoredSearchReplace.Operation.Value;

        public StoredSearchReplace StoredSearchReplace { get; }
        public ICommand RunStoredSearchCommand { get; }
    }
}
