using Dev.Editor.BusinessLogic.Models.Documents;
using Dev.Editor.BusinessLogic.ViewModels.Document;
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
    /// Logika interakcji dla klasy BinDocumentView.xaml
    /// </summary>
    public partial class BinDocumentView : UserControl, IBinEditorAccess, INotifyPropertyChanged
    {
        private BinDocumentViewModel viewModel;

        private BinDocumentState BuildCurrentState()
        {
            return new BinDocumentState(tveItems.VerticalScrollOffset, tveItems.HorizontalScrollOffset);
        }

        private void RestoreCurrentState(BinDocumentState state)
        {
            // Allowing bindings to run, so that treeview becomes
            // populated with contents
            if (state != null)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    tveItems.VerticalScrollOffset = state.VerticalOffset;
                    tveItems.HorizontalScrollOffset = state.HorizontalOffset;
                }), DispatcherPriority.ContextIdle);
            }
        }

        private void InitializeViewModel(BinDocumentViewModel newViewModel)
        {
            // Setting handler
            Handler = newViewModel.Handler;

            // Setting editor access
            newViewModel.EditorAccess = this;

            // Restoring state from the viewmodel
            var state = newViewModel.LoadState();
            RestoreCurrentState(state);
        }

        private void DeinitializeViewModel(BinDocumentViewModel viewModel)
        {
            // Storing current state in the viewmodel
            BinDocumentState state = BuildCurrentState();
            viewModel.SaveState(state);

            // Clearing editor access
            viewModel.EditorAccess = null;

            // Clearing handler
            Handler = null;
        }
       
        private void HandleDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null && !(e.NewValue is BinDocumentViewModel))
                throw new InvalidOperationException("Invalid data context for DocumentView!");

            if (viewModel != null)
            {
                DeinitializeViewModel(viewModel);
                viewModel = null;
            }

            if (e.NewValue != null)
            {
                viewModel = e.NewValue as BinDocumentViewModel;
                InitializeViewModel(viewModel);

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    // Focus fix, see TextDocumentView

                    if (viewModel.IsActive)
                        tveItems.Focus();
                }));
            }
        }

        // Protected methods --------------------------------------------------

        protected void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        // Public methods -----------------------------------------------------

        public BinDocumentView()
        {
            InitializeComponent();
        }

        // Public properties --------------------------------------------------

        public IDocumentHandler Handler { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
