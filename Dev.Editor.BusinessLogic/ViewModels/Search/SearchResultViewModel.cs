using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Dev.Editor.BusinessLogic.ViewModels.Search
{
    public class SearchResultViewModel
    {
        public SearchResultViewModel(string display, int line, int column) 
        {
            Display = display;
            Line = line;
            Column = column;
        }

        public string Display { get; }
        public int Line { get; }
        public int Column { get; }
    }
}
