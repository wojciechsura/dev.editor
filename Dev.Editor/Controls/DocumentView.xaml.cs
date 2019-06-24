using Dev.Editor.BusinessLogic.Models.Documents;
using Dev.Editor.BusinessLogic.Types.Folding;
using Dev.Editor.BusinessLogic.ViewModels.Document;
using ICSharpCode.AvalonEdit.Dev.Editor.Folding.Strategies;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Threading;
using XmlFoldingStrategy = ICSharpCode.AvalonEdit.Dev.Editor.Folding.Strategies.XmlFoldingStrategy;

namespace Dev.Editor.Controls
{
    /// <summary>
    /// Logika interakcji dla klasy DocumentView.xaml
    /// </summary>
    public partial class DocumentView : UserControl, IEditorAccess
    {
        // Private classes ----------------------------------------------------

        private struct FoldingIndex
        {
            public FoldingIndex(FoldingSection section)
            {
                StartOffset = section.StartOffset;
                EndOffset = section.EndOffset;
            }

            public FoldingIndex(FoldSectionState state)
            {
                StartOffset = state.StartOffset;
                EndOffset = state.EndOffset;
            }

            public int StartOffset { get; }
            public int EndOffset { get; }
        }

        // Private fields -----------------------------------------------------

        private DocumentViewModel viewModel;
        private FoldingManager foldingManager;
        private BaseFoldingStrategy foldingStrategy;

        private DispatcherTimer foldingTimer;

        // Private methods ----------------------------------------------------

        private void UpdateSelectionInfo()
        {
            viewModel.NotifySelectionAvailable(teEditor.SelectionLength > 0);
            viewModel.NotifyRegularSelectionAvailable(teEditor.SelectionLength > 0 && teEditor.TextArea.Selection.Segments.Count() == 1);
        }

        private void HandleViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(viewModel.Highlighting))
            {
                SetupFolding();
            }
        }

        private void HandleTextChanged(object sender, EventArgs e)
        {
            foldingTimer?.Stop();
            foldingTimer?.Start();
        }

        private void UpdateFolding(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Updating folding...");

            if (foldingManager != null && foldingStrategy != null)
                foldingStrategy.UpdateFoldings(foldingManager, viewModel.Document);

            foldingTimer.Stop();
        }

        private void SetupFolding()
        {
            // Choose folding strategy
            switch (viewModel.Highlighting?.FoldingKind)
            {
                case FoldingKind.None:
                    {
                        foldingStrategy = null;
                        break;
                    }
                case FoldingKind.Braces:
                    {
                        foldingStrategy = new BraceFoldingStrategy();
                        break;
                    }
                case FoldingKind.Xml:
                    {
                        foldingStrategy = new XmlFoldingStrategy();
                        break;
                    }
                default:
                    throw new InvalidOperationException("Unsupported folding kind!");
            }

            if (foldingStrategy != null)
            {
                // Install folding manager with chosen strategy

                if (foldingManager == null)
                    foldingManager = FoldingManager.Install(teEditor.TextArea);

                foldingStrategy.UpdateFoldings(foldingManager, viewModel.Document);

                foldingTimer = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Background, UpdateFolding, this.Dispatcher);
            }
            else
            {
                // Uninstall folding manager

                if (foldingManager != null)
                {
                    FoldingManager.Uninstall(foldingManager);
                    foldingManager = null;
                }
            }
        }

        private void ClearFolding()
        {
            foldingStrategy = null;
            if (foldingManager != null)
            {
                FoldingManager.Uninstall(foldingManager);
                foldingManager = null;
            }
        }

        private void HandleSelectionChanged(object sender, EventArgs e) => UpdateSelectionInfo();

        private DocumentState BuildCurrentState()
        {
            List<FoldSectionState> foldSections = null;

            if (foldingManager != null)
                foldSections = foldingManager.AllFoldings
                    .Select(f => new FoldSectionState(f))
                    .ToList();

            var state = new DocumentState(teEditor.CaretOffset,
                teEditor.SelectionStart,
                teEditor.SelectionLength,
                teEditor.HorizontalOffset,
                teEditor.VerticalOffset,
                foldSections);

            return state;
        }

        private void RestoreCurrentState(DocumentState state)
        {
            if (state != null)
            {
                // Restoring editor state

                teEditor.CaretOffset = state.CaretOffset;
                teEditor.SelectionStart = state.SelectionStart;
                teEditor.SelectionLength = state.SelectionLength;
                teEditor.ScrollToVerticalOffset(state.VerticalOffset);
                teEditor.ScrollToHorizontalOffset(state.HorizontalOffset);

                // Restoring foldings

                if (state.FoldSections != null && foldingManager != null)
                {
                    foldingManager.AllFoldings
                        .Join(state.FoldSections, 
                            fs => new FoldingIndex(fs), 
                            fss => new FoldingIndex(fss), 
                            (fs, fss) => new Tuple<FoldingSection, FoldSectionState>(fs, fss))
                        .ToList()
                        .ForEach(t => t.Item1.IsFolded = t.Item2.IsFolded);
                }
            }
        }

        private void HandleLoaded(object sender, RoutedEventArgs e)
        {
            viewModel = (DocumentViewModel)DataContext;

            viewModel.EditorAccess = this;
            viewModel.PropertyChanged += HandleViewModelPropertyChanged;

            // Setting folding strategy
            SetupFolding();
            teEditor.TextChanged += HandleTextChanged;

            // Restoring state from the viewmodel
            var state = viewModel.LoadState();
            RestoreCurrentState(state);

            // Hooking text editor
            teEditor.TextArea.SelectionChanged += HandleSelectionChanged;
            UpdateSelectionInfo();

            // Focusing editor
            teEditor.Focus();
        }

        private void HandleUnloaded(object sender, RoutedEventArgs e)
        {
            // Unhooking editor
            teEditor.TextArea.SelectionChanged -= HandleSelectionChanged;

            // Storing current state in the viewmodel
            DocumentState state = BuildCurrentState();
            viewModel.SaveState(state);

            // Clearing folding
            ClearFolding();
            teEditor.TextChanged -= HandleTextChanged;

            viewModel.PropertyChanged -= HandleViewModelPropertyChanged;
            viewModel.EditorAccess = null;
        }

        // Public methods -----------------------------------------------------

        public DocumentView()
        {
            InitializeComponent();
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

        public string GetSelectedText()
        {
            return teEditor.SelectedText;
        }

        public void BeginChange()
        {
            teEditor.BeginChange();
        }

        public void EndChange()
        {
            teEditor.EndChange();
        }

        public void ScrollTo(int line, int column)
        {
            teEditor.ScrollTo(line, column);
        }
    }
}
