using Dev.Editor.BusinessLogic.ViewModels.Tools.BinDefinitions;
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
    /// Interaction logic for BinDefinitionsTool.xaml
    /// </summary>
    public partial class BinDefinitionsTool : UserControl, IBinDefinitionsToolAccess
    {
        // Private fields -----------------------------------------------------

        private BinDefinitionsToolViewModel viewModel;

        // Private methods ----------------------------------------------------

        private void HandleBinDefinitionsDoubleClick(object sender, MouseButtonEventArgs e)
        {
            viewModel.BinDefinitionChosen();
        }

        private void HandleDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (viewModel != null)
                viewModel.Access = null;

            viewModel = e.NewValue as BinDefinitionsToolViewModel;

            if (viewModel != null)
                viewModel.Access = this;
        }

        // Public methods -----------------------------------------------------

        public BinDefinitionsTool()
        {
            InitializeComponent();
            viewModel = null;
        }
    }
}
