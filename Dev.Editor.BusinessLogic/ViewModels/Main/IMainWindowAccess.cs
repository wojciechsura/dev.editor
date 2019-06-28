using Dev.Editor.BusinessLogic.ViewModels.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.ViewModels.Main
{
    public interface IMainWindowAccess
    {
        void ShowNavigationPopup();
        void EnsureSelectedNavigationItemVisible();
    }
}
