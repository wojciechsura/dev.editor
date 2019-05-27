using Dev.Editor.BusinessLogic.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Services.Documents
{
    public interface IDocumentManager
    {
        ReadOnlyObservableCollection<DocumentViewModel> Documents { get; }
    }
}
