using Dev.Editor.BusinessLogic.Models.Dialogs;
using Dev.Editor.BusinessLogic.Properties;
using Dev.Editor.BusinessLogic.Services.Dialogs;
using Dev.Editor.BusinessLogic.Services.Documents;
using Dev.Editor.BusinessLogic.Services.FileService;
using Dev.Editor.Common.Commands;
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

namespace Dev.Editor.BusinessLogic.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        // Private fields -----------------------------------------------------

        private readonly IDocumentManager documentManager;
        private readonly IFileService fileService;
        private readonly IDialogService dialogService;
        private DocumentViewModel activeDocument;

        // Private methods ----------------------------------------------------

        private void DoPaste()
        {
            throw new NotImplementedException();
        }

        private void DoCut()
        {
            throw new NotImplementedException();
        }

        private void DoCopy()
        {
            throw new NotImplementedException();
        }

        private void DoRedo()
        {
            throw new NotImplementedException();
        }

        private void DoUndo()
        {
            throw new NotImplementedException();
        }

        private void DoSaveAs()
        {
            throw new NotImplementedException();
        }

        private void DoSave()
        {
            throw new NotImplementedException();
        }

        private void DoOpen()
        {
            var dialogResult = dialogService.OpenDialog();
            if (dialogResult.Result)
            {
                try
                {
                    using (FileStream fs = new FileStream(dialogResult.FileName, FileMode.Open, FileAccess.Read))
                    {
                        var ms = new MemoryStream();
                        fs.CopyTo(ms);
                        ms.Seek(0, SeekOrigin.Begin);

                        var newDoc = fileService.OpenDocument(ms, dialogResult.FileName);
                        InternalSet(ref activeDocument, () => ActiveDocument, newDoc);
                    }
                }
                catch (Exception e)
                {
                    dialogService.ShowError(string.Format(Resources.Message_CannotOpenFile, dialogResult.FileName, e.Message));
                }                
            }
        }

        private void DoNew()
        {
            var newDoc = fileService.NewDocument();
            InternalSet(ref activeDocument, () => ActiveDocument, newDoc);
        }

        // Public methods -----------------------------------------------------

        public MainWindowViewModel(IDocumentManager documentManager,
            IFileService fileService,
            IDialogService dialogService)
        {
            this.documentManager = documentManager;
            this.fileService = fileService;
            this.dialogService = dialogService;

            NewCommand = new AppCommand(obj => DoNew());
            OpenCommand = new AppCommand(obj => DoOpen());
            SaveCommand = new AppCommand(obj => DoSave());
            SaveAsCommand = new AppCommand(obj => DoSaveAs());
            UndoCommand = new AppCommand(obj => DoUndo());
            RedoCommand = new AppCommand(obj => DoRedo());
            CopyCommand = new AppCommand(obj => DoCopy());
            CutCommand = new AppCommand(obj => DoCut());
            PasteCommand = new AppCommand(obj => DoPaste());

            // TODO (if not opened with parameters)
            documentManager.AddNewDocument();
        }        

        // Public properties --------------------------------------------------

        public ReadOnlyObservableCollection<DocumentViewModel> Documents => documentManager.Documents;
        public DocumentViewModel ActiveDocument
        {
            get => activeDocument;
            set => Set(ref activeDocument, () => ActiveDocument, value);
        }

        public ICommand NewCommand { get; }
        public ICommand OpenCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand SaveAsCommand { get; }        
        public ICommand UndoCommand { get; }
        public ICommand RedoCommand { get; }
        public ICommand CopyCommand { get; }
        public ICommand CutCommand { get; }
        public ICommand PasteCommand { get; }
    }
}
