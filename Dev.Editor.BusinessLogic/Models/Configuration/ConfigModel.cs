using Dev.Editor.BusinessLogic.Models.Configuration.Behavior;
using Dev.Editor.BusinessLogic.Models.Configuration.Editor;
using Dev.Editor.BusinessLogic.Models.Configuration.Internal;
using Dev.Editor.BusinessLogic.Models.Configuration.Tools;
using Dev.Editor.BusinessLogic.Models.Configuration.UI;
using Dev.Editor.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }

        public EditorConfig Editor { get; }
        public BehaviorConfig Behavior { get; }
        public InternalConfig Internal { get; }
        public UIConfig UI { get; }
        public ToolsConfig Tools { get; }
    }
}
