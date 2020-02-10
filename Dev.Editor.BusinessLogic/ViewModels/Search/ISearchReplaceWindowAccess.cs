using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.ViewModels.Search
{
    public interface ISearchReplaceWindowAccess
    {
        void ShowAndFocus();
        void FocusSearch();
        void FocusReplace();
        void Close();
    }
}
