using Dev.Editor.BusinessLogic.ViewModels.BottomTools.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.ViewModels.BottomTools.SearchResults
{
    public interface ISearchResultsHandler : IBottomToolHandler
    {
        void OpenFileSearchResult(string fullPath, int line, int column);
    }
}
