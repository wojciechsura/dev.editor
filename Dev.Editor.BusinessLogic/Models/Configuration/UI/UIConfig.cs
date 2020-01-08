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
            BottomPanelSize = new ConfigValue<double>("BottomPanelSize", this, 200.0);
            BottomPanelVisibility = new ConfigValue<BottomPanelVisibility>("BottomPanelVisibility", this, Types.UI.BottomPanelVisibility.Hidden);

            MainWindowMaximized = new ConfigValue<bool>("MainWindowMaximized", this, false);

            MainWindowLocationSet = new ConfigValue<bool>("MainWindowLocationSet", this, false);
            MainWindowX = new ConfigValue<double>("MainWindowX", this, 0.0);
            MainWindowY = new ConfigValue<double>("MainWindowY", this, 0.0);
            MainWindowSizeSet = new ConfigValue<bool>("MainWindowSizeSet", this, false);
            MainWindowWidth = new ConfigValue<double>("MainWindowWidth", this, 0.0);
            MainWindowHeight = new ConfigValue<double>("MainWindowHeight", this, 0.0);
        }

        public ConfigValue<SidePanelPlacement> SidePanelPlacement { get; }
        public ConfigValue<double> SidePanelSize { get; }
        public ConfigValue<string> SidePanelActiveTab { get; }
        public ConfigValue<double> BottomPanelSize { get; }
        public ConfigValue<BottomPanelVisibility> BottomPanelVisibility { get; }
        public ConfigValue<string> BottomPanelActiveTab { get; }

        public ConfigValue<bool> MainWindowMaximized { get; }
        public ConfigValue<bool> MainWindowLocationSet { get; }
        public ConfigValue<double> MainWindowX { get; }
        public ConfigValue<double> MainWindowY { get; }
        public ConfigValue<bool> MainWindowSizeSet { get; }
        public ConfigValue<double> MainWindowWidth { get; }
        public ConfigValue<double> MainWindowHeight { get; }
    }
}
