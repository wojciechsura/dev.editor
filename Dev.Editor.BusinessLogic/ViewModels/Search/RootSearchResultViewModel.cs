using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.ViewModels.Search
{
    public class RootSearchResultViewModel : BaseFilesystemSearchResultViewModel
    {
        public RootSearchResultViewModel(List<BaseFilesystemSearchResultViewModel> results)
        {
            Results = results;
        }

        public List<BaseFilesystemSearchResultViewModel> Results { get; }
    }
}
