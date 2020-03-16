using Dev.Editor.BusinessLogic.Models.Documents;
using Dev.Editor.BusinessLogic.Types.Folding;
using Dev.Editor.BusinessLogic.ViewModels.Document;
using Dev.Editor.Converters;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Dev.Editor.Folding.Strategies;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Rendering;
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

            public int EndOffset { get; }

            public int StartOffset { get; }
        }

        private class SegmentColorizingTransformer : DocumentColorizingTransformer
        {
            private readonly Brush brush;
            private readonly AnchorSegment segment;

            private void SetBackgroundColor(VisualLineElement element)
            {
                element.BackgroundBrush = brush;
            }

            protected override void ColorizeLine(DocumentLine line)
            {
                if (segment.Offset <= line.EndOffset && segment.EndOffset >= line.Offset)
                {
                    int startOffset = Math.Max(line.Offset, segment.Offset);
                    int endOffset = Math.Min(line.EndOffset, segment.EndOffset);

                    ChangeLinePart(startOffset, endOffset, SetBackgroundColor);
                }
            }

            public SegmentColorizingTransformer(AnchorSegment segment, Brush brush)
            {
                this.segment = segment ?? throw new ArgumentNullException();
                this.brush = brush ?? throw new ArgumentNullException();
            }
        }

        // Private fields -----------------------------------------------------

        private readonly Brush findReplaceSegmentBackground = new SolidColorBrush(Color.FromArgb(255, 230, 230, 230));

        private TextEditor currentEditor;
        private SegmentColorizingTransformer findReplaceSegmentTransformer;
        private FoldingManager foldingManager;
        private FoldingManager foldingManager2;
        private BaseFoldingStrategy foldingStrategy;
        private DispatcherTimer foldingTimer;
        private DispatcherTimer foldingTimer2;
        private IDocumentHandler handler;
        private TextDocumentViewModel viewModel;

        // Private methods ----------------------------------------------------

        // TODO Limit extent of duplicated code in * and *2 methods

        private TextDocumentState BuildCurrentState()
        {
            TextEditorState state = BuildEditorState(teEditor, foldingManager);
            TextEditorState state2 = null;
            if (viewModel.Editor2Visible)
                state2 = BuildEditorState(teEditor2, foldingManager2);

            return new TextDocumentState(state, state2);
        }

        private TextEditorState BuildEditorState(TextEditor editor, FoldingManager manager)
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

        private void ClearFindReplaceSegment()
        {
            if (findReplaceSegmentTransformer == null)
                throw new InvalidOperationException("Cannot clear FindReplaceSegment - it is already empty!");

            findReplaceSegmentTransformer = null;
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

        private void ClearFolding2()
        {
            if (foldingManager2 != null)
            {
                FoldingManager.Uninstall(foldingManager2);
                foldingManager2 = null;
            }
        }

        private void CreateFindReplaceSegment(TextDocumentViewModel viewModel)
        {
            if (findReplaceSegmentTransformer != null)
                throw new InvalidOperationException("FindReplaceSegment is already initialized!");
            if (viewModel.FindReplaceSegment == null)
                throw new InvalidOperationException("Viewmodel does not provide FindReplaceSegment!");

            findReplaceSegmentTransformer = new SegmentColorizingTransformer(viewModel.FindReplaceSegment, findReplaceSegmentBackground);
        }

        private void DeinitializeEditor(TextDocumentViewModel viewModel)
        {
            rdTopEditor.MinHeight = 0.0;
            BindingOperations.ClearBinding(rdTopEditor, RowDefinition.HeightProperty);
            rdTopEditor.Height = new GridLength(1.0, GridUnitType.Star);

            if (findReplaceSegmentTransformer != null)
                UninstallFindReplaceSegment(teEditor);

            ClearFolding();

            teEditor.TextArea.SelectionChanged -= HandleSelectionChanged;
            teEditor.TextChanged -= HandleTextChanged;

            teEditor.SyntaxHighlighting = null;
            teEditor.Document = null;
        }

        private void DeinitializeEditor2(TextDocumentViewModel viewModel)
        {
            rdBottomEditor.MinHeight = 0.0;
            BindingOperations.ClearBinding(rdBottomEditor, RowDefinition.HeightProperty);
            rdBottomEditor.Height = new GridLength(0.0, GridUnitType.Auto);

            if (findReplaceSegmentTransformer != null)
                UninstallFindReplaceSegment(teEditor2);

            ClearFolding2();

            teEditor2.TextArea.SelectionChanged -= HandleSelectionChanged2;
            teEditor2.TextChanged -= HandleTextChanged2;

            teEditor2.SyntaxHighlighting = null;
            teEditor2.Document = null;
        }

        private void DeinitializeViewModel(TextDocumentViewModel viewModel)
        {
            // Storing current state in the viewmodel
            TextDocumentState state = BuildCurrentState();
            viewModel.SaveState(state);

            // Deinitializing editors
            if (viewModel.Editor2Visible)
                DeinitializeEditor2(viewModel);

            DeinitializeEditor(viewModel);

            if (findReplaceSegmentTransformer != null)
                ClearFindReplaceSegment();

            viewModel.PropertyChanged -= HandleViewModelPropertyChanged;
            viewModel.EditorAccess = null;

            Handler = null;
        }

        private TextEditor GetActiveEditor()
        {
            return currentEditor;
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

                if (viewModel.IsActive)
                {
                    // Focus fix. Sometimes editor is not focused. Now if active
                    // document is switched, TextDocumentView control will ensure,
                    // that text editor receives focus.

                    if (viewModel.ActiveEditor == BusinessLogic.Types.Document.Text.ActiveEditor.Primary)
                        teEditor.Focus();
                    else if (viewModel.ActiveEditor == BusinessLogic.Types.Document.Text.ActiveEditor.Secondary)
                        teEditor2.Focus();
                    else
                        throw new InvalidOperationException("Unsupported active editor!");
                }
            }
        }

        private void HandleEditorGotFocus(object sender, RoutedEventArgs e)
        {
            if (viewModel.ActiveEditor != BusinessLogic.Types.Document.Text.ActiveEditor.Primary)
                viewModel.ActiveEditor = BusinessLogic.Types.Document.Text.ActiveEditor.Primary;
        }

        private void HandleEditorGotFocus2(object sender, RoutedEventArgs e)
        {
            if (viewModel.ActiveEditor != BusinessLogic.Types.Document.Text.ActiveEditor.Secondary)
                viewModel.ActiveEditor = BusinessLogic.Types.Document.Text.ActiveEditor.Secondary;
        }

        private void HandleSelectionChanged(object sender, EventArgs e) => UpdateSelectionInfo(teEditor);

        private void HandleSelectionChanged2(object sender, EventArgs e) => UpdateSelectionInfo(teEditor2);

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

        private void HandleViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(viewModel.Highlighting))
            {
                teEditor.SyntaxHighlighting = viewModel.Highlighting.Definition;
                SetupFolding();

                if (viewModel.Editor2Visible)
                {
                    teEditor2.SyntaxHighlighting = viewModel.Highlighting.Definition;
                    SetupFolding2();
                }
            }
            else if (e.PropertyName == nameof(viewModel.Editor2Visible))
            {
                if (viewModel.Editor2Visible)
                {
                    // Displaying editor 2
                    InitializeEditor2(viewModel);

                    // Copy state from editor 1
                    var state = BuildEditorState(teEditor, foldingManager);
                    RestoreEditorState(teEditor2, foldingManager2, state);
                }
                else
                {
                    // Hiding editor 2
                    viewModel.ActiveEditor = BusinessLogic.Types.Document.Text.ActiveEditor.Primary;

                    DeinitializeEditor2(viewModel);
                }
            }
            else if (e.PropertyName == nameof(viewModel.ActiveEditor))
            {
                UpdateActiveEditor();
            }
            else if (e.PropertyName == nameof(viewModel.FindReplaceSegment))
            {
                // Clear previous segment
                if (findReplaceSegmentTransformer != null)
                {
                    UninstallFindReplaceSegment(teEditor);
                    if (viewModel.Editor2Visible)
                        UninstallFindReplaceSegment(teEditor2);
                    ClearFindReplaceSegment();
                }

                // Install new segment
                if (viewModel.FindReplaceSegment != null)
                {
                    CreateFindReplaceSegment(viewModel);
                    InstallFindReplaceSegment(teEditor);
                    if (viewModel.Editor2Visible)
                        InstallFindReplaceSegment(teEditor2);
                }
            }
        }

        private void InitializeEditor(TextDocumentViewModel newViewModel)
        {
            teEditor.Document = newViewModel.Document;
            teEditor.SyntaxHighlighting = newViewModel.Highlighting.Definition;

            SetupFolding();
            teEditor.TextChanged += HandleTextChanged;
            teEditor.TextArea.SelectionChanged += HandleSelectionChanged;

            if (findReplaceSegmentTransformer != null)
                InstallFindReplaceSegment(teEditor);

            BindingOperations.SetBinding(rdTopEditor, RowDefinition.HeightProperty, new Binding
            {
                Path = new PropertyPath(nameof(TextDocumentViewModel.Editor1Height)),
                Mode = BindingMode.TwoWay,
                Converter = new DoubleToStarGridLengthConverter()
            });

            rdTopEditor.MinHeight = 50.0;
        }

        private void InitializeEditor2(TextDocumentViewModel newViewModel)
        {
            teEditor2.Document = newViewModel.Document;
            teEditor2.SyntaxHighlighting = newViewModel.Highlighting.Definition;

            SetupFolding2();
            teEditor2.TextChanged += HandleTextChanged2;
            teEditor2.TextArea.SelectionChanged += HandleSelectionChanged2;

            if (findReplaceSegmentTransformer != null)
                InstallFindReplaceSegment(teEditor2);

            BindingOperations.SetBinding(rdBottomEditor, RowDefinition.HeightProperty, new Binding
            {
                Path = new PropertyPath(nameof(TextDocumentViewModel.Editor2Height)),
                Mode = BindingMode.TwoWay,
                Converter = new DoubleToStarGridLengthConverter()
            });

            rdBottomEditor.MinHeight = 50.0;
        }

        private void InitializeViewModel(TextDocumentViewModel newViewModel)
        {
            Handler = newViewModel.Handler;

            newViewModel.EditorAccess = this;
            newViewModel.PropertyChanged += HandleViewModelPropertyChanged;

            var state = newViewModel.LoadState();

            if (newViewModel.FindReplaceSegment != null)
                CreateFindReplaceSegment(newViewModel);

            // Hooking text editor
            InitializeEditor(newViewModel);
            if (state != null)
                RestoreEditorState(teEditor, foldingManager, state.EditorState);

            if (newViewModel.Editor2Visible)
            {
                InitializeEditor2(newViewModel);
                if (state != null)
                    RestoreEditorState(teEditor2, foldingManager, state.EditorState2);
            }

            UpdateActiveEditor();
        }

        private void InstallFindReplaceSegment(TextEditor editor)
        {
            editor.TextArea.TextView.LineTransformers.Add(findReplaceSegmentTransformer);
        }

        private void RestoreEditorState(TextEditor editor, FoldingManager manager, TextEditorState editorState)
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

        private void RunOnAllEditors(Action<TextEditor> action)
        {
            action(teEditor);
            action(teEditor2);
        }

        private void SetActiveEditor(TextEditor editor)
        {
            currentEditor = editor;

            UpdateSelectionInfo(currentEditor);
        }

        private void SetupFolding()
        {
            if (foldingTimer != null)
            {
                foldingTimer.Stop();
                foldingTimer = null;
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
                }

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

        private void SetupFolding2()
        {
            if (foldingTimer2 != null)
            {
                foldingTimer2.Stop();
                foldingTimer2 = null;
            }

            // Second editor is secondary - it relies on folding strategy chosen for the first one

            if (foldingStrategy != null)
            {
                // Install folding manager with chosen strategy

                if (foldingManager2 == null)
                {
                    foldingManager2 = FoldingManager.Install(teEditor2.TextArea);
                }

                foldingStrategy.UpdateFoldings(foldingManager2, viewModel.Document);
                foldingTimer2 = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Background, UpdateFolding2, this.Dispatcher);
            }
            else
            {
                // Uninstall folding manager

                if (foldingManager2 != null)
                {
                    FoldingManager.Uninstall(foldingManager2);
                    foldingManager2 = null;
                }
            }
        }

        private void UninstallFindReplaceSegment(TextEditor editor)
        {
            editor.TextArea.TextView.LineTransformers.Remove(findReplaceSegmentTransformer);
        }

        private void UpdateActiveEditor()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (viewModel != null && viewModel.IsActive)
                {
                    switch (viewModel.ActiveEditor)
                    {
                        case BusinessLogic.Types.Document.Text.ActiveEditor.Primary:
                            {
                                SetActiveEditor(teEditor);

                                if (teEditor2.IsFocused)
                                    teEditor.Focus();

                                break;
                            }
                        case BusinessLogic.Types.Document.Text.ActiveEditor.Secondary:
                            {
                                SetActiveEditor(teEditor2);

                                if (teEditor.IsFocused)
                                    teEditor2.Focus();

                                break;
                            }
                        default:
                            throw new InvalidOperationException("Unsupported editor!");
                    }
                }
            }), DispatcherPriority.ContextIdle);
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

        private void UpdateSelectionInfo(TextEditor editor)
        {
            viewModel.NotifySelectionAvailable(editor.SelectionLength > 0);
            viewModel.NotifyRegularSelectionAvailable(editor.SelectionLength > 0 && editor.TextArea.Selection.Segments.Count() == 1);
        }

        private void HandlePreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (viewModel != null && viewModel.FindReplaceSegment != null)
                {
                    // Clear find/replace highlight on escape press

                    viewModel.FindReplaceSegment = null;
                }
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

        public void FocusDocument()
        {
            var editor = GetActiveEditor();
            editor.Focus();
        }

        public string GetSelectedText()
        {
            var editor = GetActiveEditor();
            return editor.SelectedText;
        }

        public (int selStart, int selLength) GetSelection()
        {
            var editor = GetActiveEditor();
            return (editor.SelectionStart, editor.SelectionLength);
        }

        public void Paste() => GetActiveEditor().Paste();

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

        public void SetSelection(int selStart, int selLength)
        {
            var editor = GetActiveEditor();
            editor.Select(selStart, selLength);
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
