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
            LastSearchTexts = new LastSearchTexts(this);
            LastReplaceTexts = new LastReplaceTexts(this);
        }

        public LastSearchTexts LastSearchTexts { get; }
        public LastReplaceTexts LastReplaceTexts { get; }
    }
}
