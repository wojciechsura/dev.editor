using Dev.Editor.BusinessLogic.Models.Configuration.Search;
using Dev.Editor.BusinessLogic.Types.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.Search
{
    public class SearchReplaceDescription
    {
        public SearchReplaceDescription(string search,
            string replace,
            SearchReplaceOperation operation,
            SearchMode searchMode,
            bool isCaseSensitive,
            bool isInSelection,
            bool isSearchBackwards,
            bool isWholeWordsOnly,
            bool showReplaceSummary,
            string location,
            string fileMask)
        {
            Search = search;
            Replace = replace;
            Operation = operation;
            SearchMode = searchMode;
            IsCaseSensitive = isCaseSensitive;
            IsInSelection = isInSelection;
            IsSearchBackwards = isSearchBackwards;
            IsWholeWordsOnly = isWholeWordsOnly;
            ShowReplaceSummary = showReplaceSummary;
            Location = location;
            FileMask = fileMask;
        }

        public bool IsCaseSensitive { get; }
        public bool IsInSelection { get; }
        public bool IsSearchBackwards { get; }
        public bool IsWholeWordsOnly { get; }
        public bool ShowReplaceSummary { get; }
        public string Location { get; }
        public string FileMask { get; }
        public SearchReplaceOperation Operation { get; }
        public string Replace { get; }
        public string Search { get; }
        public SearchMode SearchMode { get; }
    }
}
