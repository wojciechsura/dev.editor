using Dev.Editor.BusinessLogic.Models.Documents;
using Dev.Editor.BusinessLogic.Types.Folding;
using Dev.Editor.BusinessLogic.ViewModels.Document;
using Dev.Editor.Converters;
using ICSharpCode.AvalonEdit;
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
    public partial class TextDocumentView : UserControl, ITextEditorAccess, INotifyPropertyChanged
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

        private TextDocumentViewModel viewModel;
        private FoldingManager foldingManager;
        private FoldingManager foldingManager2;
        private BaseFoldingStrategy foldingStrategy;
        private IDocumentHandler handler;

        private DispatcherTimer foldingTimer;
        private DispatcherTimer foldingTimer2;

        private TextEditor currentEditor;

        // Private methods ----------------------------------------------------

        private TextEditor GetActiveEditor()
        {
            return currentEditor;
        }

        private void SetActiveEditor(TextEditor editor)
        {
            currentEditor = editor;

            UpdateSelectionInfo(currentEditor);
        }

        private void RunOnAllEditors(Action<TextEditor> action)
        {
            action(teEditor);
            action(teEditor2);
        }

        private void UpdateSelectionInfo(TextEditor editor)
        {
            viewModel.NotifySelectionAvailable(editor.SelectionLength > 0);
            viewModel.NotifyRegularSelectionAvailable(editor.SelectionLength > 0 && editor.TextArea.Selection.Segments.Count() == 1);
        }

        private void HandleViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(viewModel.Highlighting))
            {
                RunOnAllEditors(editor =>
                {
                    editor.SyntaxHighlighting = viewModel.Highlighting.Definition;                    
                });
                
                SetupFolding();
            }
            else if (e.PropertyName == nameof(viewModel.Editor2Visible))
            {
                SetupEditor2Panel();
            }
        }

        private void HandleTextChanged(object sender, EventArgs e)
        {
            foldingTimer?.Stop();
            foldingTimer?.Start();
        }

        private void HandleTextChanged2(object sender, EventArgs e)
        {
            foldingTimer2?.Stop();
            foldingTimer2?.Start();
        }

        private void UpdateFolding(object sender, EventArgs e)
        {
            if (foldingManager != null && foldingStrategy != null)
                foldingStrategy.UpdateFoldings(foldingManager, viewModel.Document);

            foldingTimer.Stop();
        }

        private void UpdateFolding2(object sender, EventArgs e)
        {
            if (foldingManager2 != null && foldingStrategy != null)
                foldingStrategy.UpdateFoldings(foldingManager2, viewModel.Document);

            foldingTimer2.Stop();
        }

        private void SetupFolding()
        {
            if (foldingTimer != null)
            {
                foldingTimer.Stop();
                foldingTimer = null;
            }

            if (foldingTimer2 != null)
            {
                foldingTimer2.Stop();
                foldingTimer2 = null;
            }

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
                {
                    foldingManager = FoldingManager.Install(teEditor.TextArea);
                    foldingManager2 = FoldingManager.Install(teEditor2.TextArea);
                }

                foldingStrategy.UpdateFoldings(foldingManager, viewModel.Document);

                foldingTimer = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Background, UpdateFolding, this.Dispatcher);
                foldingTimer2 = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Background, UpdateFolding2, this.Dispatcher);
            }
            else
            {
                // Uninstall folding manager

                if (foldingManager != null)
                {
                    FoldingManager.Uninstall(foldingManager);
                    foldingManager = null;
                }

                if (foldingManager2 != null)
                {
                    FoldingManager.Uninstall(foldingManager2);
                    foldingManager2 = null;
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

            if (foldingManager2 != null)
            {
                FoldingManager.Uninstall(foldingManager2);
                foldingManager2 = null;
            }
        }

        private void HandleSelectionChanged(object sender, EventArgs e) => UpdateSelectionInfo(teEditor);

        private void HandleSelectionChanged2(object sender, EventArgs e) => UpdateSelectionInfo(teEditor2);

        private TextDocumentState BuildCurrentState()
        {
            TextEditorState GenerateEditorState(TextEditor editor, FoldingManager manager)
            {
                List<FoldSectionState> foldSections = null;
                if (manager != null)
                    foldSections = manager.AllFoldings
                        .Select(f => new FoldSectionState(f))
                        .ToList();

                var editorState = new TextEditorState(editor.CaretOffset,
                    editor.SelectionStart,
                    editor.SelectionLength,
                    editor.HorizontalOffset,
                    editor.VerticalOffset,
                    foldSections);

                return editorState;
            }

            var state = GenerateEditorState(teEditor, foldingManager);
            var state2 = GenerateEditorState(teEditor2, foldingManager2);

            return new TextDocumentState(state, state2);
        }

        private void RestoreCurrentState(TextDocumentState state)
        {
            void RestoreEditorState(TextEditor editor, FoldingManager manager, TextEditorState editorState)
            {
                // Restoring editor state

                editor.CaretOffset = editorState.CaretOffset;
                editor.SelectionStart = editorState.SelectionStart;
                editor.SelectionLength = editorState.SelectionLength;
                editor.ScrollToVerticalOffset(editorState.VerticalOffset);
                editor.ScrollToHorizontalOffset(editorState.HorizontalOffset);

                // Restoring foldings

                if (editorState.FoldSections != null && foldingManager != null)
                {
                    manager.AllFoldings
                        .Join(editorState.FoldSections,
                            fs => new FoldingIndex(fs),
                            fss => new FoldingIndex(fss),
                            (fs, fss) => new Tuple<FoldingSection, FoldSectionState>(fs, fss))
                        .ToList()
                        .ForEach(t => t.Item1.IsFolded = t.Item2.IsFolded);
                }
            }

            if (state != null)
            {
                RestoreEditorState(teEditor, foldingManager, state.EditorState);
                RestoreEditorState(teEditor2, foldingManager2, state.EditorState2);                
            }
        }

        private void InitializeViewModel(TextDocumentViewModel newViewModel)
        {
            Handler = newViewModel.Handler;

            RunOnAllEditors(editor =>
            {
                editor.Document = newViewModel.Document;
                editor.SyntaxHighlighting = newViewModel.Highlighting.Definition;
            });

            newViewModel.EditorAccess = this;
            newViewModel.PropertyChanged += HandleViewModelPropertyChanged;

            // Setting folding strategy
            SetupFolding();
            teEditor.TextChanged += HandleTextChanged;
            teEditor2.TextChanged += HandleTextChanged2;

            // Restoring state from the viewmodel
            var state = newViewModel.LoadState();
            RestoreCurrentState(state);

            // Hooking text editor
            currentEditor = teEditor;
            teEditor.TextArea.SelectionChanged += HandleSelectionChanged;
            teEditor2.TextArea.SelectionChanged += HandleSelectionChanged2;
            UpdateSelectionInfo(teEditor);
        }

        private void DeinitializeViewModel(TextDocumentViewModel viewModel)
        {
            // Unhooking editor
            teEditor.TextArea.SelectionChanged -= HandleSelectionChanged;
            teEditor2.TextArea.SelectionChanged -= HandleSelectionChanged2;

            // Storing current state in the viewmodel
            TextDocumentState state = BuildCurrentState();
            viewModel.SaveState(state);

            // Clearing folding
            ClearFolding();
            teEditor.TextChanged -= HandleTextChanged;
            teEditor2.TextChanged -= HandleTextChanged2;

            viewModel.PropertyChanged -= HandleViewModelPropertyChanged;
            viewModel.EditorAccess = null;

            RunOnAllEditors(editor =>
            {
                editor.SyntaxHighlighting = null;
                editor.Document = null;
            });

            Handler = null;
        }

        private void HandleLoaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => teEditor.Focus()), 
                DispatcherPriority.Normal);            
        }

        private void HandleDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null && !(e.NewValue is TextDocumentViewModel))
                throw new InvalidOperationException("Invalid data context for DocumentView!");

            if (viewModel != null)
            {
                DeinitializeViewModel(viewModel);
                viewModel = null;
            }

            if (e.NewValue != null)
            {
                viewModel = e.NewValue as TextDocumentViewModel;
                InitializeViewModel(viewModel);
            }

            SetupEditor2Panel();
        }

        private void HandleEditorGotFocus(object sender, RoutedEventArgs e)
        {
            SetActiveEditor(teEditor);
        }

        private void HandleEditorGotFocus2(object sender, RoutedEventArgs e)
        {
            SetActiveEditor(teEditor2);
        }

        private void SetupEditor2Panel()
        {
            if (viewModel != null && viewModel.Editor2Visible)
            {
                BindingOperations.SetBinding(rdBottomEditor, RowDefinition.HeightProperty, new Binding
                {
                    Path = new PropertyPath(nameof(TextDocumentViewModel.Editor2Height)),
                    Mode = BindingMode.TwoWay,
                    Converter = new DoubleToStarGridLengthConverter()
                });

                rdBottomEditor.MinHeight = 50.0;
            }
            else
            {
                rdBottomEditor.MinHeight = 0.0;
                BindingOperations.ClearBinding(rdBottomEditor, RowDefinition.HeightProperty);
                rdBottomEditor.Height = new GridLength(0.0, GridUnitType.Auto);
            }
        }

        // Protected methods --------------------------------------------------

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Public methods -----------------------------------------------------

        public TextDocumentView()
        {
            InitializeComponent();

            RunOnAllEditors(editor =>
            {
                editor.Options.AllowScrollBelowDocument = true;
            });
        }

        public void Copy() => GetActiveEditor().Copy();

        public void Cut() => GetActiveEditor().Cut();

        public void Paste() => GetActiveEditor().Paste();

        public (int selStart, int selLength) GetSelection()
        {
            var editor = GetActiveEditor();
            return (editor.SelectionStart, editor.SelectionLength);
        }

        public void SetSelection(int selStart, int selLength)
        {
            var editor = GetActiveEditor();
            editor.Select(selStart, selLength);
        }

        public string GetSelectedText()
        {
            var editor = GetActiveEditor();
            return editor.SelectedText;
        }

        public void RunAsSingleHistoryOperation(Action action)
        {
            var editor = GetActiveEditor();

            try
            {
                editor.BeginChange();
                action();
            }
            finally
            {
                editor.EndChange();
            }
        }

        public void ScrollTo(int line, int column)
        {
            var editor = GetActiveEditor();
            editor.ScrollTo(line, column);
        }

        public void FocusDocument()
        {
            var editor = GetActiveEditor();
            editor.Focus();
        }

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
