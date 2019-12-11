﻿using Dev.Editor.BusinessLogic.Models.Highlighting;
using Dev.Editor.BusinessLogic.Models.Search;
using Dev.Editor.BusinessLogic.ViewModels.Base;
using Dev.Editor.Common.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace Dev.Editor.BusinessLogic.ViewModels.Document
{
    public abstract class BaseDocumentViewModel : BaseViewModel
    {
        // Private methods ----------------------------------------------------

        private void DoClose()
        {
            handler.RequestClose(this);
        }

        // Protected fields ---------------------------------------------------

        protected readonly IDocumentHandler handler;

        protected bool changed;
        protected string filename;
        protected bool filenameVirtual;
        protected bool canUndo;
        protected bool canRedo;
        protected bool canSave;
        protected bool selectionAvailable;
        protected bool regularSelectionAvailable;
        protected SearchModel lastSearch;

        protected ImageSource icon;

        // Public methods -----------------------------------------------------

        public BaseDocumentViewModel(IDocumentHandler handler)
        {
            this.handler = handler;

            CloseCommand = new AppCommand(obj => DoClose());
        }

        public virtual void SetFilename(string filename, ImageSource icon)
        {
            this.filename = filename;
            this.icon = icon;

            OnPropertyChanged(() => FileName);
            OnPropertyChanged(() => Title);
            OnPropertyChanged(() => Icon);
        }

        public abstract void FocusDocument();

        public abstract void Paste();

        public abstract void Cut();

        public abstract void Copy();

        public abstract void Undo();

        public abstract void Redo();        

        // Public properties --------------------------------------------------

        public IDocumentHandler Handler => handler;

        public ICommand CloseCommand { get; }

        public string FileName
        {
            get => filename;
        }

        public ImageSource Icon
        {
            get => icon;
        }

        public string Title => Path.GetFileName(filename);

        public bool Changed
        {
            get => changed;
            set => Set(ref changed, () => Changed, value);
        }

        public bool FilenameVirtual
        {
            get => filenameVirtual;
            set => Set(ref filenameVirtual, () => FilenameVirtual, value);
        }

        public bool CanUndo
        {
            get => canUndo;
            set => Set(ref canUndo, () => CanUndo, value);
        }

        public bool CanRedo
        {
            get => canRedo;
            set => Set(ref canRedo, () => CanRedo, value);
        }

        public bool CanSave
        {
            get => canSave;
            set => Set(ref canSave, () => CanSave, value);
        }

        public bool SelectionAvailable
        {
            get => selectionAvailable;
            set => Set(ref selectionAvailable, () => SelectionAvailable, value);
        }

        public bool RegularSelectionAvailable
        {
            get => regularSelectionAvailable;
            set => Set(ref regularSelectionAvailable, () => RegularSelectionAvailable, value);
        }

        public SearchModel LastSearch
        {
            get => lastSearch;
            set => Set(ref lastSearch, () => LastSearch, value);
        }

        public abstract HighlightingInfo Highlighting { get; set; }
    }
}