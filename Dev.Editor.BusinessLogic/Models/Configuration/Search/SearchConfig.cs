using Spooksoft.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.Configuration.Search
{
    public class SearchConfig : ConfigItem
    {
        internal const string NAME = "Search";

        public SearchConfig(BaseItemContainer parent) 
            : base(NAME, parent)
        {
            LastSearchTexts = new LastSearchTexts(this);
            LastReplaceTexts = new LastReplaceTexts(this);
            RecentSearchSettings = new RecentSearchSettings(this);
            StoredSearchReplaces = new StoredSearchReplaces(this);
        }

        public LastSearchTexts LastSearchTexts { get; }
        public LastReplaceTexts LastReplaceTexts { get; }
        public StoredSearchReplaces StoredSearchReplaces { get; }
        public RecentSearchSettings RecentSearchSettings { get; }
    }
}
