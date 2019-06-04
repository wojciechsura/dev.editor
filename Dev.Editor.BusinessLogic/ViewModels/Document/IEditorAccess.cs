using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.ViewModels.Document
{
    public interface IEditorAccess
    {
        void Copy();
        void Cut();
        void Paste();
    }
}
