using Dev.Editor.BusinessLogic.Models.Documents;
using Dev.Editor.BusinessLogic.Models.Highlighting;
using HexEditor.Infrastructure;
using HexEditor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Dev.Editor.BusinessLogic.ViewModels.Document
{
    public class HexDocumentViewModel : BaseDocumentViewModel
    {
        // Private fields -----------------------------------------------------

        private readonly HexByteContainer document;

        private HexDocumentState storedState;
        private IHexEditorAccess editorAccess;

        // TODO: SearchModel lastSearch

        // Private methods ----------------------------------------------------

        private void HandleDocumentChanged(object sender, DataChangeEventArgs args)
        {
            Changed = true;
        }

        private void HandleDocumentUndoParamsChanged(object sender, EventArgs e)
        {
            CanUndo = document.CanUndo;
            CanRedo = document.CanRedo;
        }

        private void ValidateEditorAccess()
        {
            if (editorAccess == null)
                throw new InvalidOperationException("No editor attached!");
        }

        // Public methods -----------------------------------------------------

        public HexDocumentViewModel(IDocumentHandler handler, Guid guid)
            : base(handler, guid)
        {
            document = new HexByteContainer();
            document.Changed += HandleDocumentChanged;
            document.UndoParamsChanged += HandleDocumentUndoParamsChanged;

            editorAccess = null;

            canUndo = document.CanUndo;
            canRedo = document.CanRedo;
            canSave = true;
            changed = false;

            selectionAvailable = false;
            regularSelectionAvailable = false;

            storedState = null;
            filenameVirtual = true;

            // lastSearch = null;
        }

        public void NotifySelectionAvailable(bool selectionAvailable) => SelectionAvailable = selectionAvailable;

        public void NotifyRegularSelectionAvailable(bool regularSelectionAvailable) => RegularSelectionAvailable = regularSelectionAvailable;
        
        public HexDocumentState LoadState()
        {
            return storedState;
        }
        
        public void SaveState(HexDocumentState state)
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
            document.Undo();
        }

        public override void Redo()
        {
            document.Redo();
        }

        public override void FocusDocument()
        {
            editorAccess.FocusDocument();
        }

        public override string ToString()
        {
            return $"HexDocument, {base.ToString()}";
        }

        // Public properties --------------------------------------------------

        public HexByteContainer Document => document;

        // Hex document doesn't use highlighting, property is in place
        // to provide proper behavior for UI (gallery item selection)
        public override HighlightingInfo Highlighting { get; set; } = null;

        public IHexEditorAccess EditorAccess
        {
            get => editorAccess;
            set => editorAccess = value;
        }
    }
}
