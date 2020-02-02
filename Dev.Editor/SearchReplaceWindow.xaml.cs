using Dev.Editor.BusinessLogic.ViewModels.Search;
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
using System.Windows.Threading;
using Unity;
using Unity.Resolution;

namespace Dev.Editor
{
    /// <summary>
    /// Logika interakcji dla klasy SearchReplaceWindow.xaml
    /// </summary>
    public partial class SearchReplaceWindow : Window, ISearchReplaceWindowAccess
    {
        private readonly SearchReplaceWindowViewModel viewModel;

        private void HandleWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
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

        void ISearchReplaceWindowAccess.Close()
        {
            Close();
        }

        public SearchReplaceWindow(ISearchHost searchHost)
        {
            InitializeComponent();

            viewModel = Dependencies.Container.Instance.Resolve<SearchReplaceWindowViewModel>(new ParameterOverride("searchHost", searchHost),
                new ParameterOverride("access", this));

            DataContext = viewModel;
        }

        public SearchReplaceWindowViewModel ViewModel => viewModel;

        public void ChooseReplaceTab()
        {
            tcTabs.SelectedItem = tReplace;

            Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle,
                new Action(delegate()
                {
                    FocusManager.SetFocusedElement(this, cbReplaceSearch);
                    cbReplaceSearch.SelectAll();
                    cbReplaceSearch.Focus();
                }));
            
        }

        public void ChooseSearchTab()
        {
            tcTabs.SelectedItem = tSearch;

            Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle,
                new Action(delegate ()
                {
                    FocusManager.SetFocusedElement(this, cbSearchSearch);
                    cbSearchSearch.SelectAll();
                    cbSearchSearch.Focus();
                }));
        }

        public void ShowAndFocus()
        {
            Show();
            Focus();
        }
    }
}
