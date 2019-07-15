using Dev.Editor.BusinessLogic.Types.UI;
using Dev.Editor.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.Configuration.UI
{
    public class UIConfig : ConfigItem
    {
        internal const string NAME = "UI";

        public UIConfig(BaseItemContainer parent) : base(NAME, parent)
        {
            SidePanelPlacement = new ConfigValue<SidePanelPlacement>("SidePanelPlacement", this, Types.UI.SidePanelPlacement.Right);
            SidePanelSize = new ConfigValue<double>("SidePanelSize", this, 200.0);
            SidePanelActiveTab = new ConfigValue<string>("SidePanelActiveTab", this, ViewModels.Tools.Base.BaseToolViewModel.ExplorerUid);
        }

        public ConfigValue<SidePanelPlacement> SidePanelPlacement { get; }
        public ConfigValue<double> SidePanelSize { get; }
        public ConfigValue<string> SidePanelActiveTab { get; }
    }
}
