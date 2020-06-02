using Dev.Editor.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace Dev.Editor.BusinessLogic.Models.Configuration.StoredDefaults
{
    public class EscapeDefaults : ConfigItem
    {
        internal const string NAME = "EscapeDefaults";

        public EscapeDefaults(BaseItemContainer parent)
            : base(NAME, parent)
        {
            EscapeCharacter = new ConfigValue<string>("EscapeCharacter", this, "\\");
            IncludeSingleQuotes = new ConfigValue<bool>("IncludeSingleQuotes", this, false);
            IncludeDoubleQuotes = new ConfigValue<bool>("IncludeDoubleQuotes", this, true);
            IncludeSpecialCharacters = new ConfigValue<bool>("IncludeSpecialCharacters", this, true);
        }

        public ConfigValue<string> EscapeCharacter { get; }
        public ConfigValue<bool> IncludeSingleQuotes { get; }
        public ConfigValue<bool> IncludeDoubleQuotes { get; }
        public ConfigValue<bool> IncludeSpecialCharacters { get; }
    }
}
