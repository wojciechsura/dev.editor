using Dev.Editor.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.Configuration.StoredDefaults
{
    public class StoredDefaultsConfig : ConfigItem
    {
        internal const string NAME = "StoredDefaults";

        public StoredDefaultsConfig(BaseItemContainer parent)
            : base(NAME, parent)
        {
            EscapeDefaults = new EscapeDefaults(this);
        }

        public EscapeDefaults EscapeDefaults { get; }
    }
}
