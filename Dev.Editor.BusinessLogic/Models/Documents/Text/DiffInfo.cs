using Dev.Editor.BusinessLogic.Types.Document.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.Documents.Text
{
    public class DiffInfo
    {
        public DiffInfo(bool[] changes, DiffDisplayMode mode)
        {
            Changes = changes;
            Mode = mode;
        }

        public bool[] Changes { get; }
        public DiffDisplayMode Mode { get; }
    }
}
