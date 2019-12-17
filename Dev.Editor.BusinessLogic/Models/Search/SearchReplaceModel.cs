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
        public SearchReplaceModel(Regex regex, string replace, bool searchBackwards, bool isRegexReplace, bool inSelection)
        {
            Regex = regex;
            Replace = replace;
            SearchBackwards = searchBackwards;
            IsRegexReplace = isRegexReplace;
            InSelection = inSelection;
        }

        public Regex Regex { get; }
        public string Replace { get; }
        public bool SearchBackwards { get; }
        public bool IsRegexReplace { get; }
        public bool InSelection { get; }

        public bool SearchedFromBoundary { get; set; } = false;
    }
}
