﻿using Dev.Editor.BusinessLogic.Models.Documents.Text;
using Dev.Editor.BusinessLogic.Models.Highlighting;
using Dev.Editor.BusinessLogic.Models.Search;
using Dev.Editor.BusinessLogic.Types.Document;
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
        // Private fields -----------------------------------------------------

        private bool isActive;
        private bool isSelected;
        private TabColor tabColor;
        private bool isPinned;
        private DiffInfo diffResult;

        // Private methods ----------------------------------------------------

        private void DoClose()
        {
            handler.RequestClose(this);
        }

        private void DoCloseOthersCommand()
        {
            handler.RequestCloseOthers(this);
        }

        private void DoCloseAllButPinned()
        {
            handler.RequestCloseAllButPinned();
        }

        private void DoCloseAll()
        {
            handler.RequestCloseAll();
        }

        private void HandleIsActiveChanged()
        {
            handler.ChildActivated(this);
        }

        private void DoMoveToOtherViewCommand()
        {
            handler.MoveToOtherView(this);
        }

        // Protected fields ---------------------------------------------------

        protected readonly IDocumentHandler handler;

        protected bool changed;
        protected string filename;
        protected bool filenameVirtual;
        protected DateTime lastModificationDate;
        protected bool canUndo;
        protected bool canRedo;
        protected bool canSave;
        protected bool selectionAvailable;
        protected bool regularSelectionAvailable;
        protected SearchReplaceModel lastSearch;

        protected ImageSource icon;

        // Public methods -----------------------------------------------------

        public BaseDocumentViewModel(IDocumentHandler handler)
        {
            this.handler = handler;
            this.isActive = false;
            this.tabColor = TabColor.Default;
            this.isPinned = false;

            SetTabColorCommand = new AppCommand(obj => DoSetTabColor((TabColor)obj));
            CloseCommand = new AppCommand(obj => DoClose());
            CloseAllCommand = new AppCommand(obj => DoCloseAll());
            CloseAllButPinnedCommand = new AppCommand(obj => DoCloseAllButPinned());
            CloseOthersCommand = new AppCommand(obj => DoCloseOthersCommand());
            MoveToOtherViewCommand = new AppCommand(obj => DoMoveToOtherViewCommand());
        }

        private void DoSetTabColor(TabColor color)
        {
            TabColor = color;
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
        public ICommand CloseAllCommand { get; }
        public ICommand CloseAllButPinnedCommand { get; }
        public ICommand CloseOthersCommand { get; }
        public ICommand MoveToOtherViewCommand { get; }
        public ICommand SetTabColorCommand { get; }

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

        public DateTime LastModificationDate
        {
            get => lastModificationDate;
            set => Set(ref lastModificationDate, () => LastModificationDate, value);
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

        public SearchReplaceModel LastSearch
        {
            get => lastSearch;
            set => Set(ref lastSearch, () => LastSearch, value);
        }

        public abstract HighlightingInfo Highlighting { get; set; }
        
        public bool IsActive 
        {
            get => isActive; 
            set => Set(ref isActive, () => IsActive, value, HandleIsActiveChanged);
        }

        public bool IsSelected 
        {
            get => isSelected;
            set => Set(ref isSelected, () => IsSelected, value); 
        }

        public TabColor TabColor
        {
            get => tabColor;
            set => Set(ref tabColor, () => TabColor, value);
        }

        public bool IsPinned
        {
            get => isPinned;
            set => Set(ref isPinned, () => IsPinned, value);
        }

        public DiffInfo DiffResult
        {
            get => diffResult;
            set => Set(ref diffResult, () => DiffResult, value);
        }

    }
}
