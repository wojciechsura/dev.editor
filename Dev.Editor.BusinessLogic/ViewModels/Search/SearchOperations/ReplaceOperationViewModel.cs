using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.ViewModels.Search.SearchOperations
{
    public class ReplaceOperationViewModel : BaseSearchOperationViewModel
    {
        public ReplaceOperationViewModel(ISearchOperationHost searchOperationHost) 
            : base(searchOperationHost)
        {

        }

        public override string Title => Properties.Resources.Search_ReplaceTitle;
    }
}
