using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spooksoft.HexEditor.Models;

namespace Dev.Editor.BusinessLogic.Models.Documents
{
    public class HexDocumentState
    {
        public HexDocumentState(BaseSelectionInfo selection, int scrollPosition)
        {
            this.Selection = selection;
            this.ScrollPosition = scrollPosition;
        }

        public BaseSelectionInfo Selection { get; }

        public int ScrollPosition { get; }
    }
}
