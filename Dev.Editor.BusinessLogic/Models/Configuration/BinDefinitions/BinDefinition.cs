using Dev.Editor.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.Configuration.BinDefinitions
{
    public class BinDefinition : BaseCollectionItem
    {
        internal const string NAME = "BinDefinition";

        private readonly ConfigValue<string> filename;
        private readonly ConfigValue<string> definitionName;

        public BinDefinition() 
            :base(NAME)            
        {
            filename = new ConfigValue<string>("Filename", this);
            definitionName = new ConfigValue<string>("DefinitionName", this);
        }

        public ConfigValue<string> Filename { get; }
        public ConfigValue<string> DefinitionName { get; }
    }
}
