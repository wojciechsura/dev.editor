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

        private void HandleDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (viewModel != null)
                viewModel.Access = null;

            viewModel = e.NewValue as ExplorerToolViewModel;

            if (viewModel != null)
            {
                viewModel.Access = this;
                viewModel.NotifyWindowLoaded();
            }
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

        private void OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as TreeViewItem;
        }

        private void HandleFolderListPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

                if (treeViewItem != null)
                {
                    treeViewItem.IsSelected = true;
                }
                else
                {
                    FolderItemViewModel selected = tvFolders.SelectedItem as FolderItemViewModel;
                    var item = TreeViewItemFromItem(selected);
                    if (item != null)
                        item.IsSelected = false;
                }
            }
        }

        private TreeViewItem TreeViewItemFromItem(FolderItemViewModel selected)
        {
            if (selected == null)
                return null;

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
                return null;

            for (int i = hierarchy.Count - 2; i >= 0; i--)
            {
                item = (TreeViewItem)item.ItemContainerGenerator.ContainerFromItem(hierarchy[i]);
            }

            return item;
        }

        public ExplorerTool()
        {
            InitializeComponent();
            viewModel = null;
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

        public void ScrollToSelectedFile()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle,
                new Action(() =>
                {
                    lbFiles.ScrollIntoView(lbFiles.SelectedItem);
                }));
        }

        public void ScrollToSelectedFolder()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, 
                new Action(() =>
                {
                    FolderItemViewModel selected = tvFolders.SelectedItem as FolderItemViewModel;
                    var item = TreeViewItemFromItem(selected);
                    item?.BringIntoView();
                }));
        }
    }
}
    