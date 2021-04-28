using Dev.Editor.BusinessLogic.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.ViewModels.WebBrowserWindow
{
    public class WebBrowserWindowViewModel : BaseViewModel
    {
        private readonly IWebBrowserHost webBrowserHost;
        private readonly IWebBrowserWindowAccess access;

        public WebBrowserWindowViewModel(IWebBrowserHost webBrowserHost, IWebBrowserWindowAccess access)
        {
            this.webBrowserHost = webBrowserHost;
            this.access = access;
        }

        internal void ShowWindow()
        {
            access.Show();
        }

        internal void DisplayHtml(string html)
        {
            access.ShowHtml(html);
        }
    }
}
