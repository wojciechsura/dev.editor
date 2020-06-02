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
    /// Interaction logic for EscapeConfigDialog.xaml
    /// </summary>
    public partial class EscapeConfigDialog : Window, IEscapeConfigDialogAccess
    {
        private readonly EscapeConfigDialogViewModel viewModel;

        public EscapeConfigDialog(EscapeConfigModel model)
        {
            InitializeComponent();

            viewModel = Dependencies.Container.Instance.Resolve<EscapeConfigDialogViewModel>(
                new ParameterOverride("access", this),
                new ParameterOverride("model", model));
            DataContext = viewModel;
        }

        public void Close(EscapeConfigResult model, bool result)
        {
            DialogResult = result;
            Result = model;
            Close();
        }

        public EscapeConfigResult Result { get; private set; }
    }
}
