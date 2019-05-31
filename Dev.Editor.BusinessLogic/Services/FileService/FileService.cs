using Dev.Editor.BusinessLogic.Properties;
using Dev.Editor.BusinessLogic.Services.Dialogs;
using Dev.Editor.BusinessLogic.Services.Documents;
using Dev.Editor.BusinessLogic.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Services.FileService
{
    class FileService : IFileService
    {
        private readonly IDocumentManager documentManager;
        private readonly IDialogService dialogService;

        private bool DoSaveDocument(DocumentViewModel document, string filename)
        {
            using (var fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (var writer = new StreamWriter(fs))
                {
                    document.Document.WriteTextTo(writer);
                }
            }

            document.Changed = false;
            return true;
        }

        public FileService(IDocumentManager documentManager, IDialogService dialogService)
        {
            this.documentManager = documentManager;
            this.dialogService = dialogService;
        }

        public DocumentViewModel NewDocument()
        {
            return documentManager.AddNewDocument();
        }

        public DocumentViewModel OpenDocument(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                return documentManager.AddDocumentFromFile(fs, filename);
            }
        }

        public void SaveDocument(DocumentViewModel document)
        {
            if (document.FilenameVirtual)
                throw new InvalidOperationException("Cannot save without physical file location");

            DoSaveDocument(document, document.FileName);
        }

        public void SaveDocumentAs(DocumentViewModel document, string filename)
        {
            if (DoSaveDocument(document, filename))
            {
                document.FileName = filename;
            }
        }
    }
}
