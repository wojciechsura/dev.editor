using Dev.Editor.Configuration;
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
            SearchTexts = new SearchTexts(this);
            ReplaceTexts = new ReplaceTexts(this);
        }

        public SearchTexts SearchTexts { get; }
        public ReplaceTexts ReplaceTexts { get; }
    }
}
