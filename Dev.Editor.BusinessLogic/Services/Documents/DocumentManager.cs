using Dev.Editor.BusinessLogic.Properties;
using Dev.Editor.BusinessLogic.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Services.Documents
{
    class DocumentManager : IDocumentManager
    {
        private readonly ObservableCollection<DocumentViewModel> documents; 
        private readonly ReadOnlyObservableCollection<DocumentViewModel> documentsWrapper;

        private static string GenerateBlankFileName(int i)
        {
            return $"{Resources.BlankDocumentName}{i}.txt";
        }

        public DocumentManager()
        {
            documents = new ObservableCollection<DocumentViewModel>();
            documentsWrapper = new ReadOnlyObservableCollection<DocumentViewModel>(documents);
        }

        public DocumentViewModel AddNewDocument()
        {
            int i = 1;
            while (documents.Any(d => d.FileName.Equals(GenerateBlankFileName(i))))
                i++;

            var newDocument = new DocumentViewModel();
            newDocument.Document.FileName = GenerateBlankFileName(i);
            
            documents.Add(newDocument);

            return newDocument;
        }

        public ReadOnlyObservableCollection<DocumentViewModel> Documents => documentsWrapper;
    }
}
