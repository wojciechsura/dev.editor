﻿using Dev.Editor.BusinessLogic.Models.Dialogs;
using Dev.Editor.BusinessLogic.Properties;
using Dev.Editor.BusinessLogic.Services.Dialogs;
using Dev.Editor.BusinessLogic.Services.Messaging;
using Dev.Editor.BusinessLogic.ViewModels.Base;
using Dev.Editor.BusinessLogic.ViewModels.Document;
using Dev.Editor.BusinessLogic.ViewModels.Search;
using Dev.Editor.Common.Commands;
using Dev.Editor.Common.Conditions;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dev.Editor.BusinessLogic.ViewModels.Main
{
    public partial class MainWindowViewModel : BaseViewModel, ISearchHost
    {
        // Private fields -----------------------------------------------------

        private readonly IMainWindowAccess access;
        private readonly IDialogService dialogService;
        private readonly IMessagingService messagingService;

        private readonly ObservableCollection<DocumentViewModel> documents;
        private DocumentViewModel activeDocument;

        private readonly Condition documentExistsCondition;
        private readonly MutableSourcePropertyWatchCondition<MainWindowViewModel, DocumentViewModel> canUndoCondition;
        private readonly MutableSourcePropertyWatchCondition<MainWindowViewModel, DocumentViewModel> canRedoCondition;
        private readonly MutableSourcePropertyWatchCondition<MainWindowViewModel, DocumentViewModel> selectionAvailableCondition;

        // Private methods ----------------------------------------------------

        private void HandleActiveDocumentChanged()
        {
            documentExistsCondition.Value = activeDocument != null;
        }

        private void RemoveDocument(DocumentViewModel document)
        {
            int index = documents.IndexOf(document);

            documents.Remove(document);
            if (ActiveDocument == document)
            {
                if (index >= documents.Count)
                    index = documents.Count - 1;

                if (index > 0 && index < documents.Count)
                    ActiveDocument = documents[index];
                else
                    ActiveDocument = null;
            }
        }

        // Public methods -----------------------------------------------------

        public MainWindowViewModel(IMainWindowAccess access, IDialogService dialogService, IMessagingService messagingService)
        {
            this.access = access;
            this.dialogService = dialogService;
            this.messagingService = messagingService;

            documents = new ObservableCollection<DocumentViewModel>();

            documentExistsCondition = new Condition(activeDocument != null);

            canUndoCondition = new MutableSourcePropertyWatchCondition<MainWindowViewModel, DocumentViewModel>(this, vm => vm.ActiveDocument, doc => doc.CanUndo, false);
            canRedoCondition = new MutableSourcePropertyWatchCondition<MainWindowViewModel, DocumentViewModel>(this, vm => vm.ActiveDocument, doc => doc.CanRedo, false);
            selectionAvailableCondition = new MutableSourcePropertyWatchCondition<MainWindowViewModel, DocumentViewModel>(this, vm => ActiveDocument, doc => doc.SelectionAvailable, false);

            NewCommand = new AppCommand(obj => DoNew());
            OpenCommand = new AppCommand(obj => DoOpen());
            SaveCommand = new AppCommand(obj => DoSave(), documentExistsCondition);
            SaveAsCommand = new AppCommand(obj => DoSaveAs(), documentExistsCondition);

            UndoCommand = new AppCommand(obj => DoUndo(), canUndoCondition);
            RedoCommand = new AppCommand(obj => DoRedo(), canRedoCondition);
            CopyCommand = new AppCommand(obj => DoCopy(), selectionAvailableCondition);
            CutCommand = new AppCommand(obj => DoCut(), selectionAvailableCondition);
            PasteCommand = new AppCommand(obj => DoPaste(), documentExistsCondition);

            SearchCommand = new AppCommand(obj => DoSearch(), documentExistsCondition);
            ReplaceCommand = new AppCommand(obj => DoReplace(), documentExistsCondition);

            // TODO (if not opened with parameters)

            DoNew();
        }

        public bool CanCloseDocument(DocumentViewModel document)
        {
            if (!document.Changed)
            {
                return true;
            }
            else
            {
                var decision = messagingService.AskYesNoCancel(String.Format(Resources.Message_FileNotSaved, document.FileName));

                if (decision == false)
                {
                    return true;
                }
                else if (decision == true)
                {
                    if (!document.FilenameVirtual)
                    {
                        if (DoSaveDocument(document))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (DoSaveDocumentAs(document))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public void NotifyClosedDocument(DocumentViewModel document)
        {
            RemoveDocument(document);
        }

        // Public properties --------------------------------------------------

        public ObservableCollection<DocumentViewModel> Documents => documents;

        public DocumentViewModel ActiveDocument
        {
            get => activeDocument;
            set => Set(ref activeDocument, () => ActiveDocument, value, HandleActiveDocumentChanged);
        }
    }
}