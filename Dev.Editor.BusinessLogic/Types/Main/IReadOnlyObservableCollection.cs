using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Types.Main
{
    public interface ITabDocumentCollection<T> : IReadOnlyList<T>, INotifyCollectionChanged
    {
        int IndexOf(T item);

        DocumentTabKind DocumentTabKind { get; }
    }
}
