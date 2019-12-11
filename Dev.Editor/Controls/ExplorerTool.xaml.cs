using Dev.Editor.BusinessLogic.ViewModels.Tools.Explorer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Dev.Editor.Controls
{
    /// <summary>
    /// Logika interakcji dla klasy ExplorerTool.xaml
    /// </summary>
    public partial class ExplorerTool : UserControl, IExplorerToolAccess
    {
        private ExplorerToolViewModel viewModel;

        public ExplorerTool()
        {
            InitializeComponent();
            viewModel = null;
        }

        private void HandleFileListDoubleClick(object sender, MouseButtonEventArgs e)
        {
            viewModel.FileItemChosen();
        }

        private void HandleFileListKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                viewModel.FileItemChosen();
        }

        private void HandleDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (viewModel != null)
                viewModel.Access = null;

            viewModel = e.NewValue as ExplorerToolViewModel;

            if (viewModel != null)
                viewModel.Access = this;
        }

        public void FixListboxFocus()
        {
            if (lbFiles.SelectedItem != null)
            {
                lbFiles.ScrollIntoView(lbFiles.SelectedItem);
                lbFiles.UpdateLayout();

                var item = lbFiles.ItemContainerGenerator.ContainerFromItem(viewModel.SelectedFile);
                if (item != null && item is ListBoxItem listBoxItem && !listBoxItem.IsFocused)
                    listBoxItem.Focus();
            }
        }

        public void ScrollToSelectedFolder()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, 
                new Action(() =>
                {
                    FolderItemViewModel selected = tvFolders.SelectedItem as FolderItemViewModel;
                    if (selected == null)
                        return;

                    // Build hierarchy
                    List<FolderItemViewModel> hierarchy = new List<FolderItemViewModel>();
                    var current = selected;
                    while (current != null)
                    {
                        hierarchy.Add(current);
                        current = current.Parent;
                    }

                    TreeViewItem item = (TreeViewItem)tvFolders.ItemContainerGenerator.ContainerFromItem(hierarchy.Last());
                    
                    if (item == null)
                        return;

                    for (int i = hierarchy.Count - 2; i >= 0; i--)
                    {
                        item = (TreeViewItem)item.ItemContainerGenerator.ContainerFromItem(hierarchy[i]);
                    }
                    
                    item.BringIntoView();
                }));
        }

        public void ScrollToSelectedFile()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle,
                new Action(() =>
                {
                    lbFiles.ScrollIntoView(lbFiles.SelectedItem);
                }));
        }
    }
}
