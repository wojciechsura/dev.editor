using Dev.Editor.BusinessLogic.ViewModels.BottomTools.Base;
using Dev.Editor.BusinessLogic.ViewModels.FindInFiles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.ViewModels.BottomTools.SearchResults
{
    public interface ISearchResultsHandler : IBottomToolHandler, INotifyPropertyChanged
    {
        void OpenFileSearchResult(string fullPath, int line, int column, int length);
        void PerformReplaceInFiles(ReplaceResultsViewModel replaceResults);
        void CreateNewDocument(List<string> strings);
    }
}
