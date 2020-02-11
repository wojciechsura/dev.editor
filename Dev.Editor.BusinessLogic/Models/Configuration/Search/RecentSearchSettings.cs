using Dev.Editor.BusinessLogic.Types.Search;
using Dev.Editor.Configuration;

namespace Dev.Editor.BusinessLogic.Models.Configuration.Search
{
    public class RecentSearchSettings : ConfigItem
    {
        private const string NAME = "RecentSearchSettings";

        public RecentSearchSettings(BaseItemContainer parent)
            : base(NAME, parent)
        {
            CaseSensitive = new ConfigValue<bool>("CaseSensitive", this, false);
            ReplaceAllInSelection = new ConfigValue<bool>("ReplaceAllInSelection", this, false);
            SearchBackwards = new ConfigValue<bool>("SearchBackwards", this, false);
            SearchMode = new ConfigValue<SearchMode>("SearchMode", this, Types.Search.SearchMode.Normal);
            WholeWordsOnly = new ConfigValue<bool>("WholeWordsOnly", this, false);
            ShowReplaceSummary = new ConfigValue<bool>("ShowReplaceSummary", this, true);
        }

        public ConfigValue<bool> CaseSensitive { get; }
        public ConfigValue<bool> ReplaceAllInSelection { get; }
        public ConfigValue<bool> SearchBackwards { get; }
        public ConfigValue<SearchMode> SearchMode { get; }
        public ConfigValue<bool> ShowReplaceSummary { get; }
        public ConfigValue<bool> WholeWordsOnly { get; }
    }
}