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
            throw new NotImplementedException();
        }

        private void DoCut()
        {
            throw new NotImplementedException();
        }

        private void DoCopy()
        {
            throw new NotImplementedException();
        }

        private void DoRedo()
        {
            activeDocument.Document.UndoStack.Redo();
        }

        private void DoUndo()
        {
            activeDocument.Document.UndoStack.Undo();
        }
    }
}
