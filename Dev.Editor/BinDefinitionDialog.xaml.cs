using Dev.Editor.BusinessLogic.Models.Configuration.BinDefinitions;
using Dev.Editor.BusinessLogic.Models.Dialogs;
using Dev.Editor.BusinessLogic.ViewModels.Dialogs;
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
    /// Interaction logic for BinDefinitionDialog.xaml
    /// </summary>
    public partial class BinDefinitionDialog : Window, IBinDefinitionDialogAccess
    {
        private BinDefinitionDialogViewModel viewModel;

        public BinDefinitionDialog()
        {
            InitializeComponent();

            viewModel = Dependencies.Container.Instance.Resolve<BinDefinitionDialogViewModel>(
                new ParameterOverride("access", this));
            DataContext = viewModel;
        }

        public BinDefinitionDialog(BinDefinition model)
        {
            InitializeComponent();

            viewModel = Dependencies.Container.Instance.Resolve<BinDefinitionDialogViewModel>(
                new ParameterOverride("access", this),
                new ParameterOverride("model", model));
            DataContext = viewModel;
        }

        public void CloseDialog(BinDefinitionDialogResult model, bool result)
        {
            DialogResult = result;
            Result = model;
            Close();
        }

        public BinDefinitionDialogViewModel ViewModel => viewModel;

        public BinDefinitionDialogResult Result { get; private set; }
    }
}
