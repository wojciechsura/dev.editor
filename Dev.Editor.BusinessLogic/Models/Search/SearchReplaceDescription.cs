﻿using Dev.Editor.BusinessLogic.Types.Search;
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
            bool isReplaceAllInSelection,
            bool isSearchBackwards,
            bool isWholeWordsOnly)
        {
            Search = search;
            Replace = replace;
            Operation = operation;
            SearchMode = searchMode;
            IsCaseSensitive = isCaseSensitive;
            IsReplaceAllInSelection = isReplaceAllInSelection;
            IsSearchBackwards = isSearchBackwards;
            IsWholeWordsOnly = isWholeWordsOnly;
        }

        public bool IsCaseSensitive { get; }
        public bool IsReplaceAllInSelection { get; }
        public bool IsSearchBackwards { get; }
        public bool IsWholeWordsOnly { get; }
        public SearchReplaceOperation Operation { get; }
        public string Replace { get; }
        public string Search { get; }
        public SearchMode SearchMode { get; }
    }
}