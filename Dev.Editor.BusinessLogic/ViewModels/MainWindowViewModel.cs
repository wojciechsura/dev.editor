using Dev.Editor.BusinessLogic.Services.Documents;
using Dev.Editor.BusinessLogic.Services.FileService;
using Dev.Editor.BusinessLogic.ViewModels.Interfaces;
using Dev.Editor.Common.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        private DocumentViewModel activeDocument;

        // Private methods ----------------------------------------------------

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
            throw new NotImplementedException();
        }

        private void DoNew()
        {
            var newDoc = fileService.NewDocument();
            InternalSet(ref activeDocument, () => ActiveDocument, newDoc);
        }

        // Public methods -----------------------------------------------------

        public MainWindowViewModel(IDocumentManager documentManager,
            IFileService fileService)
        {
            this.documentManager = documentManager;
            this.fileService = fileService;

            NewCommand = new AppCommand(obj => DoNew());
            OpenCommand = new AppCommand(obj => DoOpen());
            SaveCommand = new AppCommand(obj => DoSave());
            SaveAsCommand = new AppCommand(obj => DoSaveAs());

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
    }
}
