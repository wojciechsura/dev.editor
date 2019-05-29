using Dev.Editor.BusinessLogic.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Services.Documents
{
    public interface IDocumentManager
    {
        DocumentViewModel AddNewDocument();

        DocumentViewModel AddNewDocument(Stream stream, string filename);

        ReadOnlyObservableCollection<DocumentViewModel> Documents { get; }
    }
}
