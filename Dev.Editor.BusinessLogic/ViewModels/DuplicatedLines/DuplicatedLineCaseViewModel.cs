using Dev.Editor.BusinessLogic.ViewModels.BottomTools.SearchResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.ViewModels.DuplicatedLines
{
    public class DuplicatedLineCaseViewModel : BaseSearchResultViewModel
    {
        public DuplicatedLineCaseViewModel(int lines, int files, List<BaseDuplicatedLineDetailsViewModel> details)
        {
            Lines = lines;
            Files = files;
            Details = details;

            IsExpanded = false;
        }

        public int Lines { get; }
        public int Files { get; }
        public List<BaseDuplicatedLineDetailsViewModel> Details { get; }
    }
}
