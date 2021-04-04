using Dev.Editor.BusinessLogic.ViewModels.Alphabet;
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
    /// Logika interakcji dla klasy AlphabetDialogWindow.xaml
    /// </summary>
    public partial class AlphabetDialogWindow : Window, IAlphabetDialogWindowAccess
    {
        private readonly AlphabetDialogViewModel viewModel;

        public AlphabetDialogWindow(string message, string previousAlphabet = null)
        {
            InitializeComponent();

            viewModel = Dependencies.Container.Instance.Resolve<AlphabetDialogViewModel>(new ParameterOverride("access", this),
                new ParameterOverride("message", message),
                new ParameterOverride("previousAlphabet", previousAlphabet));
            this.DataContext = viewModel;
        }

        public void Close(bool result)
        {
            DialogResult = result;
            Close();
        }

        public string Result => viewModel.Alphabet;
    }
}
