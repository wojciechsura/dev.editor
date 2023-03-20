using Dev.Editor.BusinessLogic.Models.Documents;
using Dev.Editor.BusinessLogic.ViewModels.Document;
using Spooksoft.HexEditor.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Logika interakcji dla klasy HexDocumentView.xaml
    /// </summary>
    public partial class HexDocumentView : UserControl, IHexEditorAccess, INotifyPropertyChanged
    {
        // Private fields -----------------------------------------------------

        private HexDocumentViewModel viewModel;
        private IDocumentHandler handler;

        // Private methods ----------------------------------------------------

        private void UpdateSelectionInfo()
        {
            viewModel.NotifySelectionAvailable(editor.Selection is RangeSelectionInfo);
            viewModel.NotifyRegularSelectionAvailable(editor.Selection is RangeSelectionInfo);
        }

        private void HandleSelectionChanged(object sender, EventArgs e)
        {
            UpdateSelectionInfo();
        }

        private HexDocumentState BuildCurrentState()
        {
            var state = new HexDocumentState(editor.Selection,
                editor.ScrollPosition);

            return state;
        }

        private void RestoreCurrentState(HexDocumentState state)
        {
            if (state != null)
            {
                editor.ScrollPosition = state.ScrollPosition;
                editor.Selection = state.Selection;
            }
        }

        private void InitializeViewModel(HexDocumentViewModel newViewModel)
        {
            // Setting handler
            Handler = newViewModel.Handler;

            // Setting editor document
            editor.Document = newViewModel.Document;

            // Setting editor access
            newViewModel.EditorAccess = this;

            // Restoring state from the viewmodel
            var state = newViewModel.LoadState();
            RestoreCurrentState(state);

            // Hooking editor
            editor.SelectionChanged += HandleSelectionChanged;
            UpdateSelectionInfo();
        }

        private void DeinitializeViewModel(HexDocumentViewModel viewModel)
        {
            // Unhooking editor
            editor.SelectionChanged -= HandleSelectionChanged;

            // Storing current state in the viewmodel
            HexDocumentState state = BuildCurrentState();
            viewModel.SaveState(state);

            // Clearing editor access
            viewModel.EditorAccess = null;

            // Clearing editor document
            editor.Document = null;

            // Clearing handler
            Handler = null;
        }

        private void HandleLoaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => editor.Focus()),
                DispatcherPriority.Normal);
        }

        private void HandleDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null && !(e.NewValue is HexDocumentViewModel))
                throw new InvalidOperationException("Invalid data context for DocumentView!");

            if (viewModel != null)
            {
                DeinitializeViewModel(viewModel);
                viewModel = null;
            }

            if (e.NewValue != null)
            {
                viewModel = e.NewValue as HexDocumentViewModel;
                InitializeViewModel(viewModel);

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    // Focus fix, see TextDocumentView

                    if (viewModel.IsActive)
                        editor.Focus();
                }), DispatcherPriority.Normal);
            }
        }

        // Protected methods --------------------------------------------------

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Public methods -----------------------------------------------------

        public HexDocumentView()
        {
            InitializeComponent();
        }

        public void Copy() => editor.Copy();

        public void Cut() => editor.Cut();

        public void Paste() => editor.Paste();

        public void FocusDocument() => editor.Focus();

        // Public properties --------------------------------------------------

        public IDocumentHandler Handler
        {
            get => handler;
            set
            {
                handler = value;
                OnPropertyChanged(nameof(Handler));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
