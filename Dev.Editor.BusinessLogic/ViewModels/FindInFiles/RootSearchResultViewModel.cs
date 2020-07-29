using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Dev.Editor.BusinessLogic.ViewModels.FindInFiles
{
    public class RootSearchResultViewModel : BaseFilesystemSearchResultViewModel
    {
        public RootSearchResultViewModel(string fullPath, string searchPattern, ImageSource icon, List<BaseFilesystemSearchResultViewModel> results)
        {
            FullPath = fullPath;
            SearchPattern = searchPattern;
            Icon = icon;
            Results = results;

            IsExpanded = true;

            Count = results.Sum(r => r.Count);
        }

        public string FullPath { get; }
        public string SearchPattern { get; }
        public ImageSource Icon { get; }
        public List<BaseFilesystemSearchResultViewModel> Results { get; }

        public override int Count { get; }
    }
}
