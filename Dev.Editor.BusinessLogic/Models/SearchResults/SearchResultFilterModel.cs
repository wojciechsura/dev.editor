using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.SearchResults
{
    public class SearchResultFilterModel
    {
        public SearchResultFilterModel(string filter,
            bool caseSensitive,
            bool filterExcludes,
            bool filterFilenames,
            bool filterContents)
        {
            Filter = filter;
            CaseSensitive = caseSensitive;
            FilterExcludes = filterExcludes;
            FilterFilenames = filterFilenames;
            FilterContents = filterContents;
        }

        public string Filter { get; }
        public bool CaseSensitive { get; }
        public bool FilterExcludes { get; }
        public bool FilterFilenames { get; }
        public bool FilterContents { get; }
    }
}
