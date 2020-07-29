using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Dev.Editor.BusinessLogic.ViewModels.FindInFiles
{
    public class SearchResultViewModel : BaseSearchResultViewModel
    {
        public SearchResultViewModel(string fullPath, string before, string match, string after, int line, int column) 
        {
            FullPath = fullPath;
            Before = before;
            Match = match;
            After = after;
            Line = line;
            Column = column;
        }

        public string FullPath { get; }
        public string Before { get; }
        public string Match { get; }
        public string After { get; }
        public int Line { get; }
        public int Column { get; }

        public string Location => $"({Line}, {Column})";
    }
}
