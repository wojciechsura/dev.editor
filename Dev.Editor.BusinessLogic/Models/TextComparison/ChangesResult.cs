using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.TextComparison
{
    public class ChangesResult
    {
        public ChangesResult(bool[] changesA, bool[] changesB)
        {
            ChangesA = changesA;
            ChangesB = changesB;
        }

        public bool[] ChangesA { get; }
        public bool[] ChangesB { get; }
    }
}
