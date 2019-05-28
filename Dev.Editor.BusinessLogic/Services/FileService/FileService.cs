using Dev.Editor.BusinessLogic.Services.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Services.FileService
{
    class FileService : IFileService
    {
        private IDocumentManager documentManager;

        public FileService(IDocumentManager documentManager)
        {
            this.documentManager = documentManager;
        }

        public void NewDocument()
        {
            documentManager.AddNewDocument();
        }
    }
}
