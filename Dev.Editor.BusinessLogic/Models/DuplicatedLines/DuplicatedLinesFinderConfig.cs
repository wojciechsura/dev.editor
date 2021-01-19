using Dev.Editor.BusinessLogic.Types.DuplicatedLines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.DuplicatedLines
{
    public class DuplicatedLinesFinderConfig
    {
        public bool Trim { get; set; }
        public int MinFiles { get; set; }
        public int MinLines { get; set; }
        public string ExcludeMasks { get; set; }
        public List<string> SourcePaths { get; set; }
        public DuplicatedLinesResultSortKind ResultSortKind { get; internal set; }
        public Regex LineExclusionRegex { get; internal set; }
    }
}
