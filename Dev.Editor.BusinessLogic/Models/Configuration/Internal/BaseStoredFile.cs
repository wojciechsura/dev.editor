using Dev.Editor.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.Configuration.Internal
{
    public class BaseStoredFile : BaseCollectionItem
    {
        private readonly ConfigValue<string> filename;
        private readonly ConfigValue<bool> isDirty;
        private readonly ConfigValue<bool> flienameIsVirtual;
        private readonly ConfigValue<string> storedFilename;
        private readonly ConfigValue<long> lastModifiedDate;

        public BaseStoredFile(string name)             
            : base(name)
        {
            filename = new ConfigValue<string>("Filename", this);
            isDirty = new ConfigValue<bool>("IsDirty", this);
            flienameIsVirtual = new ConfigValue<bool>("FilenameIsVirtual", this);
            storedFilename = new ConfigValue<string>("StoredFilename", this);
            lastModifiedDate = new ConfigValue<long>("LastModifiedDate", this);
        }

        public ConfigValue<string> Filename => filename;

        public ConfigValue<bool> IsDirty => isDirty;

        public ConfigValue<bool> FilenameIsVirtual => flienameIsVirtual;

        public ConfigValue<string> StoredFilename => storedFilename;

        public ConfigValue<long> LastModifiedDate => lastModifiedDate;
    }
}
