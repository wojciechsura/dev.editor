using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.DuplicatedLines
{
    public class DuplicatedLinesResultEntry
    {
        public DuplicatedLinesResultEntry(List<DuplicatedLinesFileInfo> filenames, List<string> lines)
        {
            Filenames = filenames;
            Lines = lines;
        }

        public List<DuplicatedLinesFileInfo> Filenames { get; }
        public List<string> Lines { get; }
    }
}
