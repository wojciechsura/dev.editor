using Dev.Editor.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.Configuration
{
    public class ConfigurationModel : ConfigRoot
    {
        private const string NAME = "DevEditor";

        public ConfigurationModel() : base(NAME)
        {
            Editor = new EditorConfiguration(this);
        }

        public EditorConfiguration Editor { get; }
    }
}
