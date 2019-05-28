using Dev.Editor.BusinessLogic.Services.Documents;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.ViewModels
{
    public class MainWindowViewModel
    {
        private readonly IDocumentManager documentManager;

        public MainWindowViewModel(IDocumentManager documentManager)
        {
            this.documentManager = documentManager;

            documentManager.AddNewDocument();
        }

        public ReadOnlyObservableCollection<DocumentViewModel> Documents => documentManager.Documents;
    }
}
