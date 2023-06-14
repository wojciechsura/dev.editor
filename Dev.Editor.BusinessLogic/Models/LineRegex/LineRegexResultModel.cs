using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.LineRegex
{
    public class LineRegexResultModel
    {
        public LineRegexResultModel(string regex, bool notMatching)
        {
            Regex = regex;
            NotMatching = notMatching;
        }

        public string Regex { get; }
        public bool NotMatching { get; }
    }
}
