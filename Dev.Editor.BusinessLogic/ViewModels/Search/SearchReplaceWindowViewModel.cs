using Dev.Editor.BusinessLogic.ViewModels.Search.SearchOperations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.ViewModels.Search
{
    public class SearchReplaceWindowViewModel : ISearchOperationHost
    {
        private readonly List<BaseSearchOperationViewModel> searchOperations = new List<BaseSearchOperationViewModel>();
        private readonly ISearchHost searchHost;
        private readonly ISearchReplaceWindowAccess access;

        private void InitializeSearchOperations()
        {
            searchOperations.Add(new SearchOperationViewModel(this));
            searchOperations.Add(new ReplaceOperationViewModel(this));
        }

        public SearchReplaceWindowViewModel(ISearchHost searchHost, ISearchReplaceWindowAccess access)
        {
            this.searchHost = searchHost;
            this.access = access;

            InitializeSearchOperations();
        }

        public void ShowSearch()
        {

            access.ShowAndFocus();
        }

        public void ShowReplace()
        {

            access.ShowAndFocus();
        }

        public IReadOnlyList<BaseSearchOperationViewModel> SearchOperations => searchOperations;
    }
}
