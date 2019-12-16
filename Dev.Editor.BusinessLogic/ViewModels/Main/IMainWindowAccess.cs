using Dev.Editor.BusinessLogic.ViewModels.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Dev.Editor.BusinessLogic.ViewModels.Main
{
    public interface IMainWindowAccess
    {
        void ShowNavigationPopup();
        void EnsureSelectedNavigationItemVisible();
        void HideNavigationPopup();

        bool GetMaximized();
        Size GetWindowSize();
        Point GetWindowLocation();
        void SetWindowSize(Size size);
        void SetWindowLocation(Point point);
        void SetWindowMaximized(bool maximized);
    }
}
