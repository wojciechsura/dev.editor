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
        public SearchReplaceModel(Regex searchRegex,
            Regex countOccurrencesRegex,
            Regex findInFilesRegex,
            string replace,
            bool searchBackwards,
            bool isRegexReplace,
            bool inSelection,
            bool showReplaceSummary,
            string location,
            string fileMask)
        {
            Regex = searchRegex;
            CountOccurrencesRegex = countOccurrencesRegex;
            FindInFilesRegex = findInFilesRegex;
            Replace = replace;
            SearchBackwards = searchBackwards;
            IsRegexReplace = isRegexReplace;
            InSelection = inSelection;
            ShowReplaceSummary = showReplaceSummary;
            Location = location;
            FileMask = fileMask;
        }

        public Regex Regex { get; }
        public Regex CountOccurrencesRegex { get; }
        public Regex FindInFilesRegex { get; }
        public string Replace { get; }
        public bool SearchBackwards { get; }
        public bool IsRegexReplace { get; }
        public bool InSelection { get; }
        public bool ShowReplaceSummary { get; }
        public string Location { get; }
        public string FileMask { get; }
        public bool SearchedFromBoundary { get; set; } = false;
        public bool SearchPerformed { get; set; } = false;
    }
}
