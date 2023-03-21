using Spooksoft.Configuration;
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
        private readonly ConfigValue<Types.Main.DocumentTabKind> documentTabKind;
        private readonly ConfigValue<Types.Document.TabColor> tabColor;
        private readonly ConfigValue<bool> isPinned;
        private readonly ConfigValue<string> guid;

        public BaseStoredFile(string name)             
            : base(name)
        {
            filename = new ConfigValue<string>("Filename", this);
            isDirty = new ConfigValue<bool>("IsDirty", this);
            flienameIsVirtual = new ConfigValue<bool>("FilenameIsVirtual", this);
            storedFilename = new ConfigValue<string>("StoredFilename", this);
            lastModifiedDate = new ConfigValue<long>("LastModifiedDate", this);
            documentTabKind = new ConfigValue<Types.Main.DocumentTabKind>("DocumentTabKind", this, Types.Main.DocumentTabKind.Primary);
            tabColor = new ConfigValue<Types.Document.TabColor>("TabColor", this, Types.Document.TabColor.Default);
            isPinned = new ConfigValue<bool>("IsPinned", this, false);
            guid = new ConfigValue<string>("Guid", this, String.Empty);
        }

        public ConfigValue<string> Filename => filename;

        public ConfigValue<bool> IsDirty => isDirty;

        public ConfigValue<bool> FilenameIsVirtual => flienameIsVirtual;

        public ConfigValue<string> StoredFilename => storedFilename;

        public ConfigValue<long> LastModifiedDate => lastModifiedDate;

        public ConfigValue<Types.Main.DocumentTabKind> DocumentTabKind => documentTabKind;

        public ConfigValue<Types.Document.TabColor> TabColor => tabColor;

        public ConfigValue<bool> IsPinned => isPinned;

        public ConfigValue<string> Guid => guid;
    }
}
