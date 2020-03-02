using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Types.Main
{
    public class TabDocumentCollection<T> : ObservableCollection<T>, ITabDocumentCollection<T>
    {
        public TabDocumentCollection(DocumentTabKind documentTabKind)
        {
            DocumentTabKind = documentTabKind;
        }

        public DocumentTabKind DocumentTabKind { get; }
    }
}
