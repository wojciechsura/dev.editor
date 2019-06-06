using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.ViewModels.Search.SearchOperations
{
    public abstract class BaseSearchOperationViewModel
    {
        protected readonly ISearchOperationHost searchOperationHost;

        public BaseSearchOperationViewModel(ISearchOperationHost searchOperationHost)
        {
            this.searchOperationHost = searchOperationHost;
        }

        public abstract string Title { get; }
    }
}
