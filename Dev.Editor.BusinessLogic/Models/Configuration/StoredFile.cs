using Dev.Editor.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.Configuration
{
    public class StoredFile : BaseCollectionItem
    {
        internal const string NAME = "StoredFile";

        private readonly ConfigValue<string> filename;
        private readonly ConfigValue<bool> isDirty;
        private readonly ConfigValue<bool> nameIsVirtual;
        private readonly ConfigValue<string> storedFilename;

        public StoredFile() 
            : base(NAME)
        {
            filename = new ConfigValue<string>("Filename", this);
            isDirty = new ConfigValue<bool>("IsDirty", this);
            nameIsVirtual = new ConfigValue<bool>("NameIsVirtual", this);
            storedFilename = new ConfigValue<string>("StoredFilename", this);
        }

        public ConfigValue<string> Filename => filename;

        public ConfigValue<bool> IsDirty => isDirty;

        public ConfigValue<bool> NameIsVirtual => nameIsVirtual;

        public ConfigValue<string> StoredFilename => storedFilename;
    }
}
