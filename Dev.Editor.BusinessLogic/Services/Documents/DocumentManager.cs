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

        public DocumentManager()
        {
            documents = new ObservableCollection<DocumentViewModel>();
            documentsWrapper = new ReadOnlyObservableCollection<DocumentViewModel>(documents);
        }

        public void AddNewDocument()
        {
            documents.Add(new DocumentViewModel());
        }

        public ReadOnlyObservableCollection<DocumentViewModel> Documents => documentsWrapper;
    }
}
