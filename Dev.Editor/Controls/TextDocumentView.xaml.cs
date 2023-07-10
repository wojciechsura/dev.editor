using Dev.Editor.BusinessLogic.Models.Documents;
using Dev.Editor.BusinessLogic.Models.Documents.Text;
using Dev.Editor.BusinessLogic.Models.TextComparison;
using Dev.Editor.BusinessLogic.Types.Document.Text;
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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        private struct SimpleSegment : IEquatable<SimpleSegment>, ISegment
        {
            public static readonly SimpleSegment Invalid = new SimpleSegment(-1, -1);

            public readonly int Offset, Length;

            int ISegment.Offset
            {
                get { return Offset; }
            }

            int ISegment.Length
            {
                get { return Length; }
            }

            public int EndOffset
            {
                get
                {
                    return Offset + Length;
                }
            }

            public SimpleSegment(int offset, int length)
            {
                this.Offset = offset;
                this.Length = length;
            }

            public SimpleSegment(ISegment segment)
            {
                Debug.Assert(segment != null);
                this.Offset = segment.Offset;
                this.Length = segment.Length;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return Offset + 10301 * Length;
                }
            }

            public override bool Equals(object obj)
            {
                return (obj is SimpleSegment) && Equals((SimpleSegment)obj);
            }

            public bool Equals(SimpleSegment other)
            {
                return this.Offset == other.Offset && this.Length == other.Length;
            }

            public static bool operator ==(SimpleSegment left, SimpleSegment right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(SimpleSegment left, SimpleSegment right)
            {
                return !left.Equals(right);
            }

            /// <inheritdoc/>
            public override string ToString()
            {
                return "[Offset=" + Offset.ToString(CultureInfo.InvariantCulture) + ", Length=" + Length.ToString(CultureInfo.InvariantCulture) + "]";
            }
        }


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

        private class ContextualBackgroundColorizer : DocumentColorizingTransformer
        {
            private readonly Brush searchSelectionBrush = new SolidColorBrush(Color.FromArgb(255, 230, 230, 230));
            private readonly Brush highlightedIdentifierBrush = new SolidColorBrush(Color.FromArgb(255, 150, 255, 210));

            private void SetSearchSelectionBackground(VisualLineElement element)
            {
                element.BackgroundBrush = searchSelectionBrush;
            }

            private void SetHighlightedIdentifierBackground(VisualLineElement element)
            {
                element.BackgroundBrush = highlightedIdentifierBrush;
            }

            protected override void ColorizeLine(DocumentLine line)
            {
                if (Segment != null && Segment.Offset <= line.EndOffset && Segment.EndOffset >= line.Offset)
                {
                    int startOffset = Math.Max(line.Offset, Segment.Offset);
                    int endOffset = Math.Min(line.EndOffset, Segment.EndOffset);

                    ChangeLinePart(startOffset, endOffset, SetSearchSelectionBackground);
                }

                if (HighlightedIdentifier != null)
                {
                    string text = CurrentContext.Document.GetText(line);
                    int start = 0;
                    int index;
                    while ((index = text.IndexOf(HighlightedIdentifier, start)) >= 0)
                    {
                        if ((index == 0 || !IsIdentifierChar(text[index - 1])) &&
                            (index + HighlightedIdentifier.Length == text.Length || !IsIdentifierChar(text[index + HighlightedIdentifier.Length])))
                        {
                            ChangeLinePart(line.Offset + index,
                                line.Offset + index + HighlightedIdentifier.Length,
                                SetHighlightedIdentifierBackground);
                        }

                        start = index + 1;
                    }
                }
            }

            public AnchorSegment Segment { get; set; }
            public string HighlightedIdentifier { get; set; }
        }

        private class DiffBackgroundRenderer : IBackgroundRenderer
        {
            private readonly bool[] changes;
            private readonly DiffDisplayMode mode;

            private readonly Brush deleteBrush;
            private readonly Brush insertBrush;

            public DiffBackgroundRenderer(bool[] changes, DiffDisplayMode mode, Brush deleteBrush, Brush insertBrush)
            {
                this.changes = changes;
                this.mode = mode;

                this.deleteBrush = deleteBrush;
                this.insertBrush = insertBrush;
            }

            public void Draw(TextView textView, DrawingContext drawingContext)
            {
                Brush brush;

                switch (mode)
                {
                    case DiffDisplayMode.Delete:
                        brush = deleteBrush;
                        break;
                    case DiffDisplayMode.Insert:
                        brush = insertBrush;
                        break;
                    default:
                        throw new InvalidOperationException("Unsupported diff mode!");
                }

                textView.EnsureVisualLines();

                foreach (var line in textView.VisualLines)
                {
                    var docLine = line.FirstDocumentLine;
                    do
                    {
                        var lineNumber = docLine.LineNumber - 1;

                        if (lineNumber < changes.Length && changes[lineNumber])
                        {
                            foreach (var rect in BackgroundGeometryBuilder.GetRectsForSegment(textView, docLine, true))
                            {
                                var fullRect = new Rect(rect.X, rect.Y, textView.ActualWidth - rect.X, rect.Height);
                                drawingContext.DrawRectangle(brush, null, fullRect);
                            }
                        }

                        docLine = docLine.NextLine;
                    }
                    while (docLine != null && docLine.PreviousLine != line.LastDocumentLine);
                }
            }

            public KnownLayer Layer
            {
                get { return KnownLayer.Background; }
            }
        }

        private class LineDiffBackgroundRenderer : IBackgroundRenderer
        {
            private readonly List<List<LineChangeInstance>> changes;
            private readonly DiffDisplayMode mode;

            private readonly Brush deleteBrush;
            private readonly Brush deleteTextBrush;
            private readonly Brush insertBrush;
            private readonly Brush insertTextBrush;

            public LineDiffBackgroundRenderer(List<List<LineChangeInstance>> changes, 
                DiffDisplayMode mode, 
                Brush deleteBrush, 
                Brush deleteTextBrush,
                Brush insertBrush,
                Brush insertTextBrush)
            {
                this.changes = changes;
                this.mode = mode;

                this.deleteBrush = deleteBrush;
                this.deleteTextBrush = deleteTextBrush;
                this.insertBrush = insertBrush;
                this.insertTextBrush = insertTextBrush;
            }

            public void Draw(TextView textView, DrawingContext drawingContext)
            {
                Brush brush, detailsBrush;

                switch (mode)
                {
                    case DiffDisplayMode.Delete:
                        brush = deleteBrush;
                        detailsBrush = deleteTextBrush;
                        break;
                    case DiffDisplayMode.Insert:
                        brush = insertBrush;
                        detailsBrush = insertTextBrush;
                        break;
                    default:
                        throw new InvalidOperationException("Unsupported diff mode!");
                }

                textView.EnsureVisualLines();

                foreach (var line in textView.VisualLines)
                {
                    var docLine = line.FirstDocumentLine;
                    do
                    {
                        var lineNumber = docLine.LineNumber - 1;

                        if (lineNumber < changes.Count && changes[lineNumber] != null && changes[lineNumber].Any())
                        {
                            // General background
                            foreach (var rect in BackgroundGeometryBuilder.GetRectsForSegment(textView, docLine, true))
                            {
                                var fullRect = new Rect(rect.X, rect.Y, textView.ActualWidth - rect.X, rect.Height);
                                drawingContext.DrawRectangle(brush, null, fullRect);
                            }

                            // Detailed changes
                            foreach (var change in changes[lineNumber])
                            {
                                var segment = new SimpleSegment(docLine.Offset + change.Start, change.End - change.Start + 1);
                                foreach (var rect in BackgroundGeometryBuilder.GetRectsForSegment(textView, segment, true))
                                {
                                    drawingContext.DrawRectangle(detailsBrush, null, rect);
                                }
                            }
                        }

                        docLine = docLine.NextLine;
                    }
                    while (docLine != null && docLine.PreviousLine != line.LastDocumentLine);
                }
            }

            public KnownLayer Layer
            {
                get { return KnownLayer.Background; }
            }
        }

        // Private fields -----------------------------------------------------

        private readonly Regex identifierRegex = new Regex("^[a-zA-Z_][a-zA-Z0-9_]+$");

        private readonly Brush deleteBrush = new SolidColorBrush(Color.FromArgb(255, 255, 210, 210));
        private readonly Brush deleteTextBrush = new SolidColorBrush(Color.FromArgb(255, 255, 160, 160));
        private readonly Brush insertBrush = new SolidColorBrush(Color.FromArgb(255, 210, 255, 230));
        private readonly Brush insertTextBrush = new SolidColorBrush(Color.FromArgb(255, 160, 255, 190));

        private readonly ContextualBackgroundColorizer contextualBackgroundColorizer;
        private readonly ContextualBackgroundColorizer contextualBackgroundColorizer2;

        private TextEditor currentEditor;
        private IBackgroundRenderer diffRenderer;
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

        private void ClearDiff()
        {
            if (diffRenderer == null)
                throw new InvalidOperationException("Cannot clear diff - it is already empty!");

            diffRenderer = null;
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

        private void CreateDiff(TextDocumentViewModel viewModel)
        {
            if (diffRenderer != null)
                throw new InvalidOperationException("Diff is already initialized!");
            if (viewModel.DiffResult == null)
                throw new InvalidOperationException("Viewmodel does not provide DiffResult!");

            switch (viewModel.DiffResult)
            {
                case DocumentDiffInfo diffInfo:
                    diffRenderer = new DiffBackgroundRenderer(diffInfo.Changes, diffInfo.Mode, deleteBrush, insertBrush);
                    break;
                case DocumentLineDiffInfo lineDiffInfo:
                    diffRenderer = new LineDiffBackgroundRenderer(lineDiffInfo.Changes, 
                        lineDiffInfo.Mode, 
                        deleteBrush, 
                        deleteTextBrush, 
                        insertBrush, 
                        insertTextBrush);
                    break;
                default:
                    throw new InvalidOperationException("Unsupported diff result!");
            }
        }

        private void DeinitializeEditor(TextDocumentViewModel viewModel)
        {
            rdTopEditor.MinHeight = 0.0;
            BindingOperations.ClearBinding(rdTopEditor, RowDefinition.HeightProperty);
            rdTopEditor.Height = new GridLength(1.0, GridUnitType.Star);

            ClearFolding();

            teEditor.TextArea.SelectionChanged -= HandleSelectionChanged;
            teEditor.TextChanged -= HandleTextChanged;

            teEditor.SyntaxHighlighting = null;
            teEditor.Document = null;

            if (diffRenderer != null)
                UninstallDiff(teEditor);

            contextualBackgroundColorizer.Segment = null;
            contextualBackgroundColorizer.HighlightedIdentifier = null;
        }

        private void DeinitializeEditor2(TextDocumentViewModel viewModel)
        {
            rdBottomEditor.MinHeight = 0.0;
            BindingOperations.ClearBinding(rdBottomEditor, RowDefinition.HeightProperty);
            rdBottomEditor.Height = new GridLength(0.0, GridUnitType.Auto);

            ClearFolding2();

            teEditor2.TextArea.SelectionChanged -= HandleSelectionChanged2;
            teEditor2.TextChanged -= HandleTextChanged2;

            teEditor2.SyntaxHighlighting = null;
            teEditor2.Document = null;

            if (diffRenderer != null)
                UninstallDiff(teEditor2);

            contextualBackgroundColorizer2.Segment = null;
            contextualBackgroundColorizer2.HighlightedIdentifier = null;
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

            if (diffRenderer != null)
                ClearDiff();

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
                        Dispatcher.BeginInvoke(new Action(() => teEditor.Focus()), DispatcherPriority.ContextIdle);
                    else if (viewModel.ActiveEditor == BusinessLogic.Types.Document.Text.ActiveEditor.Secondary)
                        Dispatcher.BeginInvoke(new Action(() => teEditor2.Focus()), DispatcherPriority.ContextIdle);
                    else
                        throw new InvalidOperationException("Unsupported active editor!");
                }
            }
        }

        private void SetViewModelActiveEditor(ActiveEditor editor)
        {
            if (viewModel.ActiveEditor != editor)
                viewModel.ActiveEditor = editor;
        }

        private void HandleEditorGotFocus(object sender, RoutedEventArgs e)
        {   
            if (viewModel != null)
                SetViewModelActiveEditor(ActiveEditor.Primary);
        }

        private void HandleEditorGotFocus2(object sender, RoutedEventArgs e)
        {
            if (viewModel != null)
            SetViewModelActiveEditor(ActiveEditor.Secondary);
        }

        private void HandlePreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (viewModel != null)
                {
                    // If QuickSearch is shown, close it.
                    if (viewModel.QuickSearchVisible)
                    {
                        viewModel.CloseQuickSearch();
                        return;
                    }    

                    // Clear find/replace highlight on escape press
                    if (viewModel.FindReplaceSegment != null)
                        viewModel.FindReplaceSegment = null;

                    viewModel.RequestClearAllDiffs();
                }
            }
        }

        private void HandleSelectionChanged(object sender, EventArgs e)
        {
            UpdateSelectionInfo(teEditor);

            TryHighlightSelectedIdentifier(teEditor, contextualBackgroundColorizer);
        }

        private void HandleSelectionChanged2(object sender, EventArgs e)
        {
            UpdateSelectionInfo(teEditor2);

            TryHighlightSelectedIdentifier(teEditor2, contextualBackgroundColorizer2);
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
                    // Copy state from editor 1
                    var state = BuildEditorState(teEditor, foldingManager);

                    // Displaying editor 2
                    InitializeEditor2(viewModel, state);

                    RestoreEditorState(teEditor2, foldingManager2, state);

                    TryHighlightSelectedIdentifier(teEditor2, contextualBackgroundColorizer2);
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
                UpdateActiveEditorFromViewModel();
            }
            else if (e.PropertyName == nameof(viewModel.FindReplaceSegment))
            {                
                contextualBackgroundColorizer.Segment = viewModel.FindReplaceSegment;
                contextualBackgroundColorizer2.Segment = viewModel.FindReplaceSegment;

                RedrawEditors(viewModel);
            }
            else if (e.PropertyName == nameof(viewModel.DiffResult))
            {
                // Uninstall previous diff
                if (diffRenderer != null)
                {
                    UninstallDiff(teEditor);
                    if (viewModel.Editor2Visible)
                        UninstallDiff(teEditor2);

                    ClearDiff();
                }

                // Install new diff
                if (viewModel.DiffResult != null)
                {
                    CreateDiff(viewModel);
                    InstallDiff(teEditor);
                    if (viewModel.Editor2Visible)
                        InstallDiff(teEditor2);
                }
            }
        }

        private void InitializeEditor(TextDocumentViewModel newViewModel, TextEditorState state)
        {
            teEditor.Document = newViewModel.Document;
            teEditor.SyntaxHighlighting = newViewModel.Highlighting.Definition;

            SetupFolding();
            teEditor.TextChanged += HandleTextChanged;
            teEditor.TextArea.SelectionChanged += HandleSelectionChanged;

            if (diffRenderer != null)
                InstallDiff(teEditor);

            BindingOperations.SetBinding(rdTopEditor, RowDefinition.HeightProperty, new Binding
            {
                Path = new PropertyPath(nameof(TextDocumentViewModel.Editor1Height)),
                Mode = BindingMode.TwoWay,
                Converter = new DoubleToStarGridLengthConverter()
            });

            rdTopEditor.MinHeight = 50.0;

            if (state != null)
                RestoreEditorState(teEditor, foldingManager, state);

            TryHighlightSelectedIdentifier(teEditor, contextualBackgroundColorizer);
        }

        private void InitializeEditor2(TextDocumentViewModel newViewModel, TextEditorState state)
        {
            teEditor2.Document = newViewModel.Document;
            teEditor2.SyntaxHighlighting = newViewModel.Highlighting.Definition;

            SetupFolding2();
            teEditor2.TextChanged += HandleTextChanged2;
            teEditor2.TextArea.SelectionChanged += HandleSelectionChanged2;

            if (diffRenderer != null)
                InstallDiff(teEditor2);

            BindingOperations.SetBinding(rdBottomEditor, RowDefinition.HeightProperty, new Binding
            {
                Path = new PropertyPath(nameof(TextDocumentViewModel.Editor2Height)),
                Mode = BindingMode.TwoWay,
                Converter = new DoubleToStarGridLengthConverter()
            });

            rdBottomEditor.MinHeight = 50.0;

            if (state != null)
                RestoreEditorState(teEditor2, foldingManager, state);

            TryHighlightSelectedIdentifier(teEditor2, contextualBackgroundColorizer2);
        }

        private void InitializeViewModel(TextDocumentViewModel newViewModel)
        {
            System.Diagnostics.Debug.WriteLine($"Initializing viewmodel, newViewModel: {newViewModel?.Document?.FileName ?? "(none)"}");

            Handler = newViewModel.Handler;

            newViewModel.EditorAccess = this;
            newViewModel.PropertyChanged += HandleViewModelPropertyChanged;

            var state = newViewModel.LoadState();

            if (newViewModel.DiffResult != null)
                CreateDiff(newViewModel);

            // Hooking text editor
            InitializeEditor(newViewModel, state?.EditorState);

            if (newViewModel.Editor2Visible)
                InitializeEditor2(newViewModel, state?.EditorState2);

            contextualBackgroundColorizer.Segment = newViewModel.FindReplaceSegment;
            contextualBackgroundColorizer2.Segment = newViewModel.FindReplaceSegment;

            RedrawEditors(newViewModel);

            UpdateActiveEditorFromViewModel();
        }

        private void InstallDiff(TextEditor editor)
        {
            editor.TextArea.TextView.BackgroundRenderers.Add(diffRenderer);
        }

        private static bool IsIdentifierChar(char c)
        {
            return (c >= 'a' && c <= 'z') || 
                (c >= 'A' && c <= 'Z') || 
                (c >= '0' && c <= '9') || 
                c == '_';
        }       

        private void TryHighlightSelectedIdentifier(TextEditor editor, ContextualBackgroundColorizer colorizingTransformer)
        {
            if (editor.SelectionLength == 0)
            {
                SetIdentifierHighlightedWord(editor, colorizingTransformer, null);

                return;
            }

            if (editor.SelectionStart > 0 && 
                IsIdentifierChar(editor.Document.GetCharAt(editor.SelectionStart - 1)))
            {
                SetIdentifierHighlightedWord(editor, colorizingTransformer, null);

                return;
            }

            if (editor.SelectionStart + editor.SelectionLength - 1 < editor.Document.TextLength - 1 && 
                IsIdentifierChar(editor.Document.GetCharAt(editor.SelectionStart + editor.SelectionLength)))
            {
                SetIdentifierHighlightedWord(editor, colorizingTransformer, null);

                return;
            }

            var selectedText = editor.Document.GetText(editor.SelectionStart, editor.SelectionLength);
            if (!identifierRegex.IsMatch(selectedText))
            {
                SetIdentifierHighlightedWord(editor, colorizingTransformer, null);

                return;
            }

            SetIdentifierHighlightedWord(editor, colorizingTransformer, selectedText);
        }

        private static void SetIdentifierHighlightedWord(TextEditor editor, ContextualBackgroundColorizer colorizingTransformer, string highlightedWord)
        {
            if (colorizingTransformer.HighlightedIdentifier != highlightedWord)
            {
                colorizingTransformer.HighlightedIdentifier = highlightedWord;
                editor.TextArea.TextView.Redraw();
            }
        }

        private void RedrawEditors(TextDocumentViewModel viewModel)
        {
            teEditor.TextArea.TextView.Redraw();
            if (viewModel.Editor2Visible)
                teEditor2.TextArea.TextView.Redraw();
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

        private void UninstallDiff(TextEditor editor)
        {
            editor.TextArea.TextView.BackgroundRenderers.Remove(diffRenderer);
        }

        private void UpdateActiveEditorFromViewModel()
        {
            System.Diagnostics.Debug.WriteLine($"Calling updateActiveEditorFromViewModel, viewModel: {viewModel?.FileName ?? "(none)"}, selected: {viewModel?.IsSelected.ToString() ?? "(none)"}");

            Dispatcher.BeginInvoke(new Action(() =>
            {
                System.Diagnostics.Debug.WriteLine($"Executing updateActiveEditorFromViewMode, viewModel: {viewModel?.FileName ?? "(none)"}, selected: {viewModel?.IsSelected.ToString() ?? "(none)"}");

                if (viewModel != null && viewModel.IsSelected)
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

        private bool GetQuickSearchFocused() => tbQuickSearch.IsFocused;

        // Protected methods --------------------------------------------------

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Public methods -----------------------------------------------------

        public TextDocumentView()
        {
            InitializeComponent();

            contextualBackgroundColorizer = new ContextualBackgroundColorizer();
            teEditor.TextArea.TextView.LineTransformers.Add(contextualBackgroundColorizer);

            contextualBackgroundColorizer2 = new ContextualBackgroundColorizer();
            teEditor2.TextArea.TextView.LineTransformers.Add(contextualBackgroundColorizer2);

            RunOnAllEditors(editor =>
            {
                editor.Options.AllowScrollBelowDocument = true;
                editor.Options.AllowToggleOverstrikeMode = true;
                editor.Options.ConvertTabsToSpaces = true;
                editor.Options.ColumnRulerPosition = 80;
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

        public void FocusQuickSearch()
        {
            tbQuickSearch.Focus();
            tbQuickSearch.SelectAll();
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

        private void HandleQuickSearchKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                viewModel.NotifyQuickSearchEnterPressed();
            }
        }

        public bool QuickSearchFocused
        {
            get
            {
                return GetQuickSearchFocused();
            }
        }
    }
}