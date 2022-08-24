using Autofac;
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

namespace Dev.Editor
{
    /// <summary>
    /// Interaction logic for BinDefinitionDialog.xaml
    /// </summary>
    public partial class NameDialog : Window, INameDialogAccess
    {
        private NameDialogViewModel viewModel;
        
        public NameDialog(NameDialogModel model)
        {
            InitializeComponent();

            viewModel = Dependencies.Container.Instance.Resolve<NameDialogViewModel>(
                new NamedParameter("access", this),
                new NamedParameter("model", model));
            DataContext = viewModel;
        }

        public void CloseDialog(NameDialogResult model, bool result)
        {
            DialogResult = result;
            Result = model;
            Close();
        }

        public NameDialogViewModel ViewModel => viewModel;

        public NameDialogResult Result { get; private set; }
    }
}
