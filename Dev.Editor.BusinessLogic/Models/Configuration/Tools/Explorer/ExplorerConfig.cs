using Spooksoft.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.Configuration.Tools.Explorer
{
    public class ExplorerConfig : ConfigItem
    {
        internal const string NAME = "Explorer";

        public ExplorerConfig(BaseItemContainer parent) : base(NAME, parent)
        {
            FolderTreeHeight = new ConfigValue<double>("FolderTreeHeight", this, 200.0);
            LastFolder = new ConfigValue<string>("LastFolder", this, null);
        }

        public ConfigValue<double> FolderTreeHeight { get; }
        public ConfigValue<string> LastFolder { get; }
    }
}
