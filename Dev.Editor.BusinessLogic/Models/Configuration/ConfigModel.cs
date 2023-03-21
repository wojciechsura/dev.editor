using Dev.Editor.BusinessLogic.Models.Configuration.Behavior;
using Dev.Editor.BusinessLogic.Models.Configuration.BinDefinitions;
using Dev.Editor.BusinessLogic.Models.Configuration.Editor;
using Dev.Editor.BusinessLogic.Models.Configuration.Internal;
using Dev.Editor.BusinessLogic.Models.Configuration.Search;
using Dev.Editor.BusinessLogic.Models.Configuration.StoredDefaults;
using Dev.Editor.BusinessLogic.Models.Configuration.Tools;
using Dev.Editor.BusinessLogic.Models.Configuration.UI;
using Spooksoft.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace Dev.Editor.BusinessLogic.Models.Configuration
{
    public class ConfigModel : ConfigRoot
    {
        internal const string NAME = "DevEditor";

        public ConfigModel() : base(NAME)
        {
            Editor = new EditorConfig(this);
            Behavior = new BehaviorConfig(this);
            Internal = new InternalConfig(this);
            UI = new UIConfig(this);
            Tools = new ToolsConfig(this);
            BinDefinitions = new BinDefinitionsConfig(this);
            SearchConfig = new SearchConfig(this);
            StoredDefaults = new StoredDefaultsConfig(this);
        }

        public EditorConfig Editor { get; }
        public BehaviorConfig Behavior { get; }
        public InternalConfig Internal { get; }
        public UIConfig UI { get; }
        public ToolsConfig Tools { get; }
        public BinDefinitionsConfig BinDefinitions { get; }
        public SearchConfig SearchConfig { get; }
        public StoredDefaultsConfig StoredDefaults { get; }
    }
}
