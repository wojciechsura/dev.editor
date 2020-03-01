using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Types.Main
{
    public interface IReadOnlyObservableCollection<T> : IReadOnlyCollection<T>, INotifyCollectionChanged
    {
        
    }
}
