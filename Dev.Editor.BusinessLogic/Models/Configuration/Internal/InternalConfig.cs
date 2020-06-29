using Dev.Editor.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.Configuration.Internal
{
    public class InternalConfig : ConfigItem
    {
        internal const string NAME = "Internal";

        private readonly StoredFiles storedFiles;
        private readonly ConfigValue<string> leftSelectedDocument;
        private readonly ConfigValue<string> rightSelectedDocument;
        private readonly ConfigValue<string> activeDocument;

        public InternalConfig(BaseItemContainer parent) : base(NAME, parent)
        {
            storedFiles = new StoredFiles(this);

            leftSelectedDocument = new ConfigValue<string>("LeftSelectedDocument", this, String.Empty);
            rightSelectedDocument = new ConfigValue<string>("RightSelectedDocument", this, String.Empty);
            activeDocument = new ConfigValue<string>("ActiveDocument", this, String.Empty);
        }

        public StoredFiles StoredFiles => storedFiles;

        public ConfigValue<string> PrimarySelectedDocument => leftSelectedDocument;

        public ConfigValue<string> SecondarySelectedDocument => rightSelectedDocument;

        public ConfigValue<string> ActiveDocument => activeDocument;
    }
}
