using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.Search
{
    public class ReplaceModel : SearchModel
    {
        public ReplaceModel(Regex regex, string replace, bool searchBackwards, bool isRegexReplace)
            : base(regex, searchBackwards)
        {
            Replace = replace;
            IsRegexReplace = isRegexReplace;
        }

        public bool IsRegexReplace { get; }
        public string Replace { get; }
    }
}
