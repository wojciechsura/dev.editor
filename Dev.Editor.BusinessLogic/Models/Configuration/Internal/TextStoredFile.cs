using Dev.Editor.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.Configuration.Internal
{
    public class TextStoredFile : BaseStoredFile
    {
        internal const string NAME = "TextStoredFile";

        private readonly ConfigValue<string> highlightingName;

        public TextStoredFile()
            : base(NAME)
        {
            highlightingName = new ConfigValue<string>("HighlightingName", this);
        }

        public ConfigValue<string> HighlightingName => highlightingName;
    }
}
