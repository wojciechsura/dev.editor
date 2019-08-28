using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.ViewModels.Main
{
    public partial class MainWindowViewModel
    {
        // Private methods ----------------------------------------------------

        private void DoPaste()
        {
            activeDocument.Paste();
        }

        private void DoCut()
        {
            activeDocument.Cut();
        }

        private void DoCopy()
        {
            activeDocument.Copy();
        }

        private void DoRedo()
        {
            activeDocument.Redo();
        }

        private void DoUndo()
        {
            activeDocument.Undo();
        }
    }
}
