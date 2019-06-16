using Dev.Editor.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.Configuration
{
    public class EditorData : ConfigItem
    {
        internal const string NAME = "Editor";

        public EditorData(BaseItemContainer parent) : base(NAME, parent)
        {
            WordWrap = new ConfigValue<bool>("WordWrap", this, false);
            LineNumbers = new ConfigValue<bool>("LineNumbers", this, false);
        }

        public ConfigValue<bool> WordWrap { get; }
        public ConfigValue<bool> LineNumbers { get; }
    }
}
