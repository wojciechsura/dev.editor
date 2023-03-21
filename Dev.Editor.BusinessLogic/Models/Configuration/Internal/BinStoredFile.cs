using Spooksoft.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.Configuration.Internal
{
    public class BinStoredFile : BaseStoredFile
    {
        internal const string NAME = "BinStoredFile";

        private ConfigValue<string> definitionUid;

        public BinStoredFile()
            : base(NAME)
        {
            definitionUid = new ConfigValue<string>("DefinitionUid", this);            
        }

        public ConfigValue<string> DefinitionUid => definitionUid;
    }
}
