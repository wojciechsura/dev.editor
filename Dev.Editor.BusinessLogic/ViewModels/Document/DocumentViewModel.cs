using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dev.Editor.BusinessLogic.Models.Documents;
using Dev.Editor.BusinessLogic.ViewModels.Base;
using Dev.Editor.Common.Conditions;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;

namespace Dev.Editor.BusinessLogic.ViewModels.Document
{
    public class DocumentViewModel : BaseViewModel
    {
        // Private fields -----------------------------------------------------

        private readonly TextDocument document;
        private bool changed;
        private bool filenameVirtual;
        private bool canUndo;
        private bool canRedo;
        private bool selectionAvailable;
        private DocumentState storedState;

        // Private methods ----------------------------------------------------

        private void HandleFileNameChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(() => FileName);
        }

        private void HandleUndoStackPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(document.UndoStack.CanUndo))
                this.CanUndo = document.UndoStack.CanUndo;
            else if (e.PropertyName == nameof(document.UndoStack.CanRedo))
                this.CanRedo = document.UndoStack.CanRedo;
            else if (e.PropertyName == nameof(document.UndoStack.IsOriginalFile))
                this.Changed = !document.UndoStack.IsOriginalFile;
        }

        // Public methods -----------------------------------------------------

        public DocumentViewModel()
        {
            document = new TextDocument();
            document.FileNameChanged += HandleFileNameChanged;
            document.UndoStack.PropertyChanged += HandleUndoStackPropertyChanged;

            canUndo = document.UndoStack.CanUndo;
            canRedo = document.UndoStack.CanRedo;
            changed = document.UndoStack.IsOriginalFile;
            selectionAvailable = false;

            storedState = null;
            changed = false;
            filenameVirtual = true;
        }

        public void NotifySelectionAvailable(bool selectionAvailable) => SelectionAvailable = selectionAvailable;

        public DocumentState LoadState()
        {
            return storedState;
        }

        public void SaveState(DocumentState state)
        {
            storedState = state;
        }

        // Public properties --------------------------------------------------

        public TextDocument Document => document;

        public string FileName
        {
            get => document.FileName;
            set
            {
                document.FileName = value;
                OnPropertyChanged(() => FileName);
                OnPropertyChanged(() => Title);
            }
        }

        public string Title => Path.GetFileName(document.FileName);
            
        public bool Changed
        {
            get => changed;
            set  
            {
                Set(ref changed, () => Changed, value);
            }
        }

        public bool FilenameVirtual
        {
            get => filenameVirtual;
            set
            {
                Set(ref filenameVirtual, () => FilenameVirtual, value);
            } 
        }

        public bool CanUndo
        {
            get => canUndo;
            set
            {
                Set(ref canUndo, () => CanUndo, value);
            }
        }

        public bool CanRedo
        {
            get => canRedo;
            set
            {
                Set(ref canRedo, () => CanRedo, value);
            }
        }

        public bool SelectionAvailable
        {
            get => selectionAvailable;
            set
            {
                Set(ref selectionAvailable, () => SelectionAvailable, value);
            }
        }
    }
}
