using Dev.Editor.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.Configuration.Search
{
    public class ReplaceText : BaseCollectionItem
    {
        public const string NAME = "ReplaceText";

        public ReplaceText() : base(NAME)
        {
            Text = new ConfigValue<string>("Text", this, "");
        }

        public ConfigValue<string> Text { get; set; }
    }
}
