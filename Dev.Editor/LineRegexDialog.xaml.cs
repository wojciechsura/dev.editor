using Autofac;
using Dev.Editor.BusinessLogic.Models.LineRegex;
using Dev.Editor.BusinessLogic.ViewModels.LineRegex;
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
    /// Logika interakcji dla klasy LineRegexWindow.xaml
    /// </summary>
    public partial class LineRegexDialog : Window, ILineRegexDialogViewModelAccess
    {
        private readonly LineRegexDialogViewModel viewModel;

        public LineRegexDialog()
        {
            InitializeComponent();

            viewModel = Dependencies.Container.Instance.Resolve<LineRegexDialogViewModel>(new NamedParameter("access", this));
            DataContext = viewModel;
        }

        public void Close(bool result)
        {
            DialogResult = result;
            Close();
        }

        public LineRegexResultModel Result => viewModel.Result;
    }
}
