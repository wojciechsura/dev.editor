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
    /// Logika interakcji dla klasy DiffConfigDialog.xaml
    /// </summary>
    public partial class DiffConfigDialog : Window, IDiffConfigDialogAccess
    {
        private DiffConfigDialogViewModel viewModel;

        public DiffConfigDialog(DiffConfigDialogModel model)
        {
            InitializeComponent();

            viewModel = new DiffConfigDialogViewModel(this, model);
            DataContext = viewModel;
        }

        public void Close(DiffConfigDialogResult model, bool result)
        {
            DialogResult = result;
            Result = model;
            Close();
        }

        public DiffConfigDialogResult Result { get; private set; }
    }
}
