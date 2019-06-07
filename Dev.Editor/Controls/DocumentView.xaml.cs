using Dev.Editor.BusinessLogic.Models.Documents;
using Dev.Editor.BusinessLogic.ViewModels.Document;
using ICSharpCode.AvalonEdit.Document;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Logika interakcji dla klasy DocumentView.xaml
    /// </summary>
    public partial class DocumentView : UserControl, IEditorAccess
    {
        private DocumentViewModel viewModel;

        private void UpdateSelectionInfo() => viewModel.NotifySelectionAvailable(teEditor.SelectionLength > 0);

        private void HandleSelectionChanged(object sender, EventArgs e) => UpdateSelectionInfo();

        private void HandleLoaded(object sender, RoutedEventArgs e)
        {
            viewModel = (DocumentViewModel)DataContext;

            // Restoring state from the viewmodel
            var state = viewModel.LoadState();

            if (state != null)
            {
                teEditor.CaretOffset = state.CaretOffset;
                teEditor.SelectionStart = state.SelectionStart;
                teEditor.SelectionLength = state.SelectionLength;
                teEditor.ScrollToVerticalOffset(state.VerticalOffset);
                teEditor.ScrollToHorizontalOffset(state.HorizontalOffset);
            }

            // Hooking text editor
            teEditor.TextArea.SelectionChanged += HandleSelectionChanged;
            UpdateSelectionInfo();

            // Focusing editor
            teEditor.Focus();

            viewModel.EditorAccess = this;
        }

        private void HandleUnloaded(object sender, RoutedEventArgs e)
        {
            // Unhooking editor
            teEditor.TextArea.SelectionChanged -= HandleSelectionChanged;

            var state = new DocumentState(teEditor.CaretOffset,
                teEditor.SelectionStart,
                teEditor.SelectionLength,
                teEditor.HorizontalOffset,
                teEditor.VerticalOffset);
            viewModel.SaveState(state);

            viewModel.EditorAccess = null;
        }

        public void Copy() => teEditor.Copy();
        public void Cut() => teEditor.Cut();
        public void Paste() => teEditor.Paste();
        public (int selStart, int selLength) GetSelection()
        {
            return (teEditor.SelectionStart, teEditor.SelectionLength);
        }

        public void SetSelection(int selStart, int selLength)
        {
            teEditor.SelectionStart = selStart;
            teEditor.SelectionLength = selLength;
        }

        public void ScrollTo(int line, int column)
        {
            teEditor.ScrollTo(line, column);
        }

        public DocumentView()
        {
            InitializeComponent();
        }
    }
}
