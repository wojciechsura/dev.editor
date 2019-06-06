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

namespace Dev.Editor
{
    /// <summary>
    /// Logika interakcji dla klasy SearchReplaceWindow.xaml
    /// </summary>
    public partial class SearchReplaceWindow : Window, ISearchReplaceWindowAccess
    {
        private readonly SearchReplaceWindowViewModel viewModel;

        public SearchReplaceWindow(ISearchHost searchHost)
        {
            InitializeComponent();

            viewModel = new SearchReplaceWindowViewModel(searchHost, this);
            DataContext = viewModel;
        }

        public SearchReplaceWindowViewModel ViewModel => viewModel;

        public void ShowAndFocus()
        {
            Show();
            Focus();
        }

        private void HandleWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
