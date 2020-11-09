using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Dev.Editor.BusinessLogic.ViewModels.FindInFiles
{
    public class ReplaceResultsViewModel : SearchResultsViewModel
    {
        public ReplaceResultsViewModel(string fullPath, string searchPattern, ImageSource icon, List<BaseFilesystemSearchResultViewModel> results) 
            : base(fullPath, searchPattern, icon, results)
        {

        }
    }
}
