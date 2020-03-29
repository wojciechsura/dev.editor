using Dev.Editor.BusinessLogic.Types.TextComparison;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.TextComparison
{
    public class LineDiffInfo
    {
        public LineDiffInfo(ChangeType change, string line)
        {
            Change = change;
            Line = line;
        }

        public ChangeType Change { get; }
        public string Line { get; }
    }
}
