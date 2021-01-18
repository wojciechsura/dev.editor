using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.DuplicatedLines
{
    public class DuplicatedLinesResult
    {
        public DuplicatedLinesResult(List<DuplicatedLinesResultEntry> entries)
        {
            Entries = entries;
        }

        public List<DuplicatedLinesResultEntry> Entries { get; }
    }
}
