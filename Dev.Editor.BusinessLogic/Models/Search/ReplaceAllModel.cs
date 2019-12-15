using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.Search
{
    public class ReplaceAllModel : ReplaceModel
    {
        public ReplaceAllModel(Regex regex, string replace, bool searchBackwards, bool isRegexReplace, bool inSelection) 
            : base(regex, replace, searchBackwards, isRegexReplace)
        {
            InSelection = inSelection;
        }

        public bool InSelection { get; }
    }
}
