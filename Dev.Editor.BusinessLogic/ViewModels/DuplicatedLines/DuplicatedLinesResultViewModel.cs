using Dev.Editor.BusinessLogic.ViewModels.BottomTools.SearchResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.ViewModels.DuplicatedLines
{
    public class DuplicatedLinesResultViewModel : BaseSearchResultViewModel
    {
        public DuplicatedLinesResultViewModel(int totalLines, List<DuplicatedLineCaseViewModel> cases)
        {
            TotalLines = totalLines;
            Cases = cases;
        }

        public int TotalLines { get; }

        public List<DuplicatedLineCaseViewModel> Cases { get; }
    }
}
