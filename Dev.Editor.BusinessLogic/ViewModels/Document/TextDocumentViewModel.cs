﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using Dev.Editor.BusinessLogic.Models.Documents;
using Dev.Editor.BusinessLogic.Models.Documents.Text;
using Dev.Editor.BusinessLogic.Models.Highlighting;
using Dev.Editor.BusinessLogic.Models.Search;
using Dev.Editor.BusinessLogic.Types.Document.Text;
using Dev.Editor.BusinessLogic.ViewModels.Base;
using Spooksoft.VisualStateManager.Commands;
using Spooksoft.VisualStateManager.Conditions;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;

namespace Dev.Editor.BusinessLogic.ViewModels.Document
{
    public class TextDocumentViewModel : BaseDocumentViewModel
    {
        // Private fields -----------------------------------------------------

        private readonly TextDocument document;

        private TextDocumentState storedState;

        private ITextEditorAccess editorAccess;
        private HighlightingInfo highlighting;

        private double editor1Height;
        private double editor2Height;
        private bool editor2Visible;
        private ActiveEditor activeEditor;
        private AnchorSegment findReplaceSegment;
        private BaseDocumentDiffInfo diffResult;

        private string quickSearchText;
        private bool quickSearchVisible;
        private bool quickSearchFound;
        private bool quickSearchCaseSensitive;
        private bool quickSearchWholeWord;
        private bool quickSearchRegex;

        // Private methods ----------------------------------------------------

        private void HandleUndoStackPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(document.UndoStack.CanUndo))
                this.CanUndo = document.UndoStack.CanUndo;
            else if (e.PropertyName == nameof(document.UndoStack.CanRedo))
                this.CanRedo = document.UndoStack.CanRedo;
            else if (e.PropertyName == nameof(document.UndoStack.IsOriginalFile))
                this.Changed = !document.UndoStack.IsOriginalFile;
        }

        private void ValidateEditorAccess()
        {
            if (editorAccess == null)
                throw new InvalidOperationException("No editor attached!");
        }

        private void HandleQuickSearchTextChanged()
        {
            QuickSearchFound = handler.PerformQuickSearch(quickSearchText, false, quickSearchCaseSensitive, quickSearchWholeWord, quickSearchRegex);
        }

        private void HandleHighlightingChanged()
        {
            OnPropertyChanged(() => HighlightingToolset);
        }

        // Public methods -----------------------------------------------------

        public TextDocumentViewModel(IDocumentHandler handler, Guid guid)
            : base(handler, guid)
        {
            document = new TextDocument();
            document.UndoStack.PropertyChanged += HandleUndoStackPropertyChanged;

            editorAccess = null;

            canUndo = document.UndoStack.CanUndo;
            canRedo = document.UndoStack.CanRedo;
            canSave = true;
            changed = false;

            selectionAvailable = false;
            regularSelectionAvailable = false;

            storedState = null;
            filenameVirtual = true;

            lastSearch = null;

            editor1Height = 1.0;
            editor2Height = 1.0;
            editor2Visible = false;
            activeEditor = ActiveEditor.Primary;
            findReplaceSegment = null;

            quickSearchText = "";
            quickSearchVisible = false;

            CloseQuickSearchCommand = new AppCommand(obj => CloseQuickSearch());
        }

        public void RunAsSingleHistoryEntry(Action action)
        {
            editorAccess.RunAsSingleHistoryOperation(action);            
        }

        public T RunAsSingleHistoryEntry<T>(Func<T> func)
        {
            T result = default(T);

            editorAccess.RunAsSingleHistoryOperation(() => result = func());

            return result;
        }

        public void NotifySelectionAvailable(bool selectionAvailable) => SelectionAvailable = selectionAvailable;

        public void NotifyRegularSelectionAvailable(bool regularSelectionAvailable) => RegularSelectionAvailable = regularSelectionAvailable;        

        public TextDocumentState LoadState()
        {
            return storedState;
        }

        public void SaveState(TextDocumentState state)
        {
            storedState = state;
        }

        public override void Copy()
        {
            ValidateEditorAccess();
            editorAccess.Copy();
        }

        public override void Cut()
        {
            ValidateEditorAccess();
            editorAccess.Cut();
        }

        public override void Paste()
        {
            ValidateEditorAccess();
            editorAccess.Paste();
        }

        public override void Undo()
        {
            Document.UndoStack.Undo();
        }

        public override void Redo()
        {
            Document.UndoStack.Redo();
        }

        public (int selStart, int selLength) GetSelection()
        {
            return editorAccess.GetSelection();
        }

        public void SetSelection(int selStart, int selLength, bool scrollTo = true)
        {
            editorAccess.SetSelection(selStart, selLength);

            if (scrollTo)
            {
                var location = document.GetLocation(selStart);
                editorAccess.ScrollTo(location.Line, location.Column);
            }
        }

        public string GetSelectedText()
        {
            return editorAccess.GetSelectedText();
        }

        public override void FocusDocument()
        {
            editorAccess.FocusDocument();
        }

        public void RequestClearAllDiffs()
        {
            handler.RequestClearAllDiffs();
        }

        public override string ToString()
        {
            return $"TextDocument, {base.ToString()}";
        }

        public void ShowQuickSearch(string quickSearchText = "")
        {
            QuickSearchText = quickSearchText;
            QuickSearchVisible = true;

            editorAccess.FocusQuickSearch();
        }

        public void CloseQuickSearch()
        {
            QuickSearchVisible = false;
            editorAccess.FocusDocument();
        }

        public void NotifyQuickSearchEnterPressed()
        {
            handler.PerformQuickSearch(quickSearchText, true, quickSearchCaseSensitive, quickSearchWholeWord, quickSearchRegex);
        }

        // Public properties --------------------------------------------------

        public TextDocument Document => document;

        public override HighlightingInfo Highlighting
        {
            get => highlighting;
            set => Set(ref highlighting, () => Highlighting, value, HandleHighlightingChanged);
        }

        public ITextEditorAccess EditorAccess
        {
            get => editorAccess;
            set => editorAccess = value;
        }

        public ICommand CopyCommand => handler.CopyCommand;

        public ICommand CutCommand => handler.CutCommand;

        public ICommand PasteCommand => handler.PasteCommand;

        public ICommand CloseQuickSearchCommand { get; }

        public double Editor1Height
        {
            get => editor1Height;
            set => Set(ref editor1Height, () => Editor1Height, value);
        }

        public double Editor2Height
        {
            get => editor2Height;
            set => Set(ref editor2Height, () => Editor2Height, value);
        }

        public bool Editor2Visible
        {
            get => editor2Visible;
            set => Set(ref editor2Visible, () => Editor2Visible, value);
        }

        public ActiveEditor ActiveEditor
        {
            get => activeEditor;
            set => Set(ref activeEditor, () => ActiveEditor, value);
        }

        public AnchorSegment FindReplaceSegment
        {
            get => findReplaceSegment;
            set => Set(ref findReplaceSegment, () => FindReplaceSegment, value);
        }

        public BaseDocumentDiffInfo DiffResult
        {
            get => diffResult;
            set => Set(ref diffResult, () => DiffResult, value);
        }

        public string QuickSearchText
        {
            get => quickSearchText;
            set => Set(ref quickSearchText, () => QuickSearchText, value, HandleQuickSearchTextChanged);
        }

        public bool QuickSearchCaseSensitive
        {
            get => quickSearchCaseSensitive;
            set => Set(ref quickSearchCaseSensitive, () => QuickSearchCaseSensitive, value);
        }

        public bool QuickSearchWholeWord
        {
            get => quickSearchWholeWord;
            set => Set(ref quickSearchWholeWord, () => QuickSearchWholeWord, value);
        }

        public bool QuickSearchRegex
        {
            get => quickSearchRegex;
            set => Set(ref quickSearchRegex, () => QuickSearchRegex, value);
        }

        public bool QuickSearchVisible
        {
            get => quickSearchVisible;
            set => Set(ref quickSearchVisible, () => QuickSearchVisible, value);
        }

        public bool QuickSearchFocused => editorAccess.QuickSearchFocused;

        public bool QuickSearchFound
        {
            get => quickSearchFound;
            set => Set(ref quickSearchFound, () => QuickSearchFound, value);
        }
    }
}
