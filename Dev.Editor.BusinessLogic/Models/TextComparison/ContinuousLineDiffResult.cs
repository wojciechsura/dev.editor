using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.TextComparison
{
    public class ContinuousLineDiffResult
    {
        public ContinuousLineDiffResult(List<LineDiffInfo> lines)
        {
            Lines = lines;
        }

        public List<LineDiffInfo> Lines { get; }
    }
}
