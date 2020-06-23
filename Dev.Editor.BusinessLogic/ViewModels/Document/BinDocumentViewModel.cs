using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dev.Editor.BinAnalyzer.Data;
using Dev.Editor.BusinessLogic.Models.Configuration.BinDefinitions;
using Dev.Editor.BusinessLogic.Models.Documents;
using Dev.Editor.BusinessLogic.Models.Highlighting;

namespace Dev.Editor.BusinessLogic.ViewModels.Document
{
    public class BinDocumentViewModel : BaseDocumentViewModel
    {
        // Private fields -----------------------------------------------------

        private List<BaseData> document;
        private BinDocumentState storedState;

        // Public methods -----------------------------------------------------

        public BinDocumentViewModel(IDocumentHandler handler) : base(handler)
        {
            canUndo = false;
            canRedo = false;
            canSave = false;
            selectionAvailable = false;
        }

        public override void FocusDocument()
        {
            throw new NotImplementedException();
        }

        public override void Copy()
        {
            
        }

        public override void Cut()
        {
            
        }

        public override void Paste()
        {
            
        }

        public override void Redo()
        {
            
        }

        public override void Undo()
        {

        }

        public BinDocumentState LoadState()
        {
            return storedState;
        }

        public void SaveState(BinDocumentState state)
        {
            storedState = state;
        }

        public override string ToString()
        {
            return $"BinDocument, {base.ToString()}";
        }

        // Public properties --------------------------------------------------

        public override HighlightingInfo Highlighting { get; set; } = null;

        public List<BaseData> Document
        {
            get => document;
            set => Set(ref document, () => Document, value);
        }

        public BinDefinition Definition { get; set; }

        public IBinEditorAccess EditorAccess { get; set; }
    }
}
