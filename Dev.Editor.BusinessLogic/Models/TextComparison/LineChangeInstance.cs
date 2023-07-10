using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.TextComparison
{
    public class LineChangeInstance
    {
        public LineChangeInstance(int start, int end)
        {
            Start = start;
            End = end;
        }

        public int Start { get; }
        public int End { get; }
    }
}
