using Dev.Editor.BusinessLogic.Models.DuplicatedLines;
using Dev.Editor.BusinessLogic.ViewModels.Dialogs.DuplicatedLines;
using Dev.Editor.Helpers;
using System.Text.RegularExpressions;
using System.Windows;
using Unity;
using Unity.Resolution;

namespace Dev.Editor
{
    /// <summary>
    /// Interaction logic for DuplicatedLinesFinderConfigWindow.xaml
    /// </summary>
    public partial class DuplicatedLinesFinderConfigDialog : Window, IDuplicatedLinesFinderConfigDialogAccess
    {
        private readonly Regex numericsOnly = new Regex("^[0-9]*$");

        private DuplicatedLinesFinderConfigDialogViewModel viewModel;

        public DuplicatedLinesFinderConfigDialog(DuplicatedLinesFinderConfigModel model)
        {
            InitializeComponent();

            viewModel = Dependencies.Container.Instance.Resolve<DuplicatedLinesFinderConfigDialogViewModel>(
                new ParameterOverride("access", this),
                new ParameterOverride("model", model));
            DataContext = viewModel;
        }

        public void CloseDialog(DuplicatedLinesFinderConfig model, bool result)
        {
            DialogResult = result;
            Result = model;
            Close();
        }

        public DuplicatedLinesFinderConfig Result { get; internal set; }

        private void AcceptOnlyNumeric(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !numericsOnly.IsMatch(e.Text);
        }

        private void SourceListDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ListBoxHelper.VisualUpwardSearch(e.OriginalSource as DependencyObject) != null)
                viewModel.NotifyEntryDoubleClicked();
        }
    }
}
