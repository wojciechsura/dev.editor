using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.Dialogs
{
    public class EscapeConfigResult
    {
        public EscapeConfigResult(string escapeChar, bool includeSingleQuotes, bool includeDoubleQuotes, bool includeSpecialCharacters)
        {
            EscapeChar = escapeChar;            
            IncludeSingleQuotes = includeSingleQuotes;
            IncludeDoubleQuotes = includeDoubleQuotes;
            IncludeSpecialCharacters = includeSpecialCharacters;
        }

        public string EscapeChar { get; }

        public bool IncludeSingleQuotes { get; }

        public bool IncludeDoubleQuotes { get; }

        public bool IncludeSpecialCharacters { get; }
    }
}
