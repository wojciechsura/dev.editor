using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
