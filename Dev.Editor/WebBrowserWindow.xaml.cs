using Dev.Editor.BusinessLogic.ViewModels.WebBrowserWindow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Unity;
using Unity.Resolution;

namespace Dev.Editor
{
    /// <summary>
    /// Logika interakcji dla klasy WebBrowserWindow.xaml
    /// </summary>
    public partial class WebBrowserWindow : Window, IWebBrowserWindowAccess
    {
        private readonly WebBrowserWindowViewModel viewModel;

        void IWebBrowserWindowAccess.Show()
        {
            this.Show();
        }

        void IWebBrowserWindowAccess.ShowHtml(string html)
        {
            wbBrowser.NavigateToString(html);
        }

        public WebBrowserWindow(IWebBrowserHost webBrowserHost)
        {
            InitializeComponent();

            viewModel = Dependencies.Container.Instance.Resolve<WebBrowserWindowViewModel>(new ParameterOverride("webBrowserHost", webBrowserHost),
                new ParameterOverride("access", this));

            DataContext = viewModel;
        }

        public WebBrowserWindowViewModel ViewModel => viewModel;

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();

            // This window's focus and owner window's focus are independent
            // However, user expects, that focusing this window focuses
            // the application, so we have to ensure, that main window gains
            // focus at this point.
            if (Owner != null)
            {
                Owner.Focus();
            }
        }
    }
}
