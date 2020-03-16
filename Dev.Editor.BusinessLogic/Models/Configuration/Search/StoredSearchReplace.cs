using Dev.Editor.BusinessLogic.Types.Search;
using Dev.Editor.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.Configuration.Search
{
    public class StoredSearchReplace : BaseCollectionItem
    {
        public const string NAME = "StoredSearchReplace";

        public StoredSearchReplace()
            : base(NAME)
        {
            SearchName = new ConfigValue<string>("SearchName", this, "");
            IsCaseSensitive = new ConfigValue<bool>("IsCaseSensitive", this, false);
            IsSearchBackwards = new ConfigValue<bool>("IsSearchBackwards", this, false);
            IsWholeWordsOnly = new ConfigValue<bool>("IsWholeWordsOnly", this, false);
            ShowReplaceSummary = new ConfigValue<bool>("ShowReplaceSummary", this, false);
            Operation = new ConfigValue<SearchReplaceOperation>("Operation", this, SearchReplaceOperation.Search);
            Replace = new ConfigValue<string>("Replace", this, "");
            Search = new ConfigValue<string>("Search", this, "");
            SearchMode = new ConfigValue<SearchMode>("SearchMode", this, Types.Search.SearchMode.Normal);
        }

        public ConfigValue<string> SearchName { get; }
        public ConfigValue<bool> IsCaseSensitive { get; }
        public ConfigValue<bool> IsSearchBackwards { get; }
        public ConfigValue<bool> IsWholeWordsOnly { get; }
        public ConfigValue<bool> ShowReplaceSummary { get; }
        public ConfigValue<SearchReplaceOperation> Operation { get; }
        public ConfigValue<string> Replace { get; }
        public ConfigValue<string> Search { get; }
        public ConfigValue<SearchMode> SearchMode { get; }
    }
}
