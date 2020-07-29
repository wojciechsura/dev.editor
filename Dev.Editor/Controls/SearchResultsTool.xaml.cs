using Dev.Editor.BusinessLogic.ViewModels.BottomTools.SearchResults;
using Dev.Editor.Helpers;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Dev.Editor.Controls
{
    /// <summary>
    /// Logika interakcji dla klasy SearchResultsTool.xaml
    /// </summary>
    public partial class SearchResultsTool : UserControl
    {
        private SearchResultsBottomToolViewModel viewModel;

        private void HandleDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            viewModel = e.NewValue as SearchResultsBottomToolViewModel;
        }

        public SearchResultsTool()
        {
            InitializeComponent();
        }

        private void HandleTreeDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = TreeViewHelper.VisualUpwardSearch(e.OriginalSource as DependencyObject);
            if (treeViewItem != null)
            {
                viewModel.NotifyItemDoubleClicked(treeViewItem.DataContext);
            }            
        }
    }
}
