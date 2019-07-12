using Dev.Editor.BusinessLogic.Models.Configuration.Behavior;
using Dev.Editor.BusinessLogic.Models.Configuration.Editor;
using Dev.Editor.BusinessLogic.Models.Configuration.Internal;
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

        private readonly EditorConfig editorConfiguration;
        private readonly BehaviorConfig behaviorConfiguration;
        private readonly InternalConfig internalConfiguration;
        private readonly UIConfig uiConfig;

        public ConfigModel() : base(NAME)
        {
            editorConfiguration = new EditorConfig(this);
            behaviorConfiguration = new BehaviorConfig(this);
            internalConfiguration = new InternalConfig(this);
            uiConfig = new UIConfig(this);
        }

        public EditorConfig Editor => editorConfiguration;
        public BehaviorConfig Behavior => behaviorConfiguration;
        public InternalConfig Internal => internalConfiguration;
        public UIConfig UI => uiConfig;
    }
}
