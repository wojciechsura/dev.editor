using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.Documents
{
    public class BinDocumentState
    {
        public BinDocumentState(double verticalOffset, double horizontalOffset)
        {
            VerticalOffset = verticalOffset;
            HorizontalOffset = horizontalOffset;
        }

        public double VerticalOffset { get; }
        public double HorizontalOffset { get; }
    }
}
