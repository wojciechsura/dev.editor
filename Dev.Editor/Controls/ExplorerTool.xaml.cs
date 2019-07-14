using Dev.Editor.BusinessLogic.ViewModels.Tools.Explorer;
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
    }
}
