using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.Search
{
    public class ReplaceModel 
    {
        public ReplaceModel(Regex regex, string replace, bool searchBackwards)
        {
            Regex = regex;
            Replace = replace;
            SearchBackwards = searchBackwards;
        }

        public Regex Regex { get; set; }
        public string Replace { get; set; }
        public bool SearchBackwards { get; }
    }
}
