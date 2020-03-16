using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.Search
{
    public class SearchReplaceModel 
    {
        public SearchReplaceModel(Regex regex, string replace, bool searchBackwards, bool isRegexReplace, bool inSelection, bool showReplaceSummary)
        {
            Regex = regex;
            Replace = replace;
            SearchBackwards = searchBackwards;
            IsRegexReplace = isRegexReplace;
            InSelection = inSelection;
            ShowReplaceSummary = showReplaceSummary;
        }

        public Regex Regex { get; }
        public string Replace { get; }
        public bool SearchBackwards { get; }
        public bool IsRegexReplace { get; }
        public bool InSelection { get; }
        public bool ShowReplaceSummary { get; }

        public bool SearchedFromBoundary { get; set; } = false;
        public bool SearchPerformed { get; set; } = false;
    }
}
