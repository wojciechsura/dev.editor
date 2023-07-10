using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.TextComparison
{
    public class LineChangesResult
    {
        public LineChangesResult(List<List<LineChangeInstance>> changesA, List<List<LineChangeInstance>> changesB) 
        {
            ChangesA = changesA;
            ChangesB = changesB;
        }

        public List<List<LineChangeInstance>> ChangesA { get; }
        public List<List<LineChangeInstance>> ChangesB { get; }
    }
}
