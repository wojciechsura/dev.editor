using Dev.Editor.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.Configuration
{
    public class InternalData : ConfigItem
    {
        internal const string NAME = "Internal";

        private readonly StoredFiles storedFiles;

        public InternalData(BaseItemContainer parent) : base(NAME, parent)
        {
            storedFiles = new StoredFiles(this);
        }

        public StoredFiles StoredFiles => storedFiles;
    }
}
