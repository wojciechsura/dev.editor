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
            documentsManager.ActiveDocument.Paste();
        }

        private void DoCut()
        {
            documentsManager.ActiveDocument.Cut();
        }

        private void DoCopy()
        {
            documentsManager.ActiveDocument.Copy();
        }

        private void DoRedo()
        {
            documentsManager.ActiveDocument.Redo();
        }

        private void DoUndo()
        {
            documentsManager.ActiveDocument.Undo();
        }
    }
}
