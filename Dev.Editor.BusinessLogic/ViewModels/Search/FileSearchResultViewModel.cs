using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Dev.Editor.BusinessLogic.ViewModels.Search
{
    public class FileSearchResultViewModel
    {
        public FileSearchResultViewModel(FormattedText display, string fullPath, List<SearchResultViewModel> results)
        {
            if (string.IsNullOrEmpty(fullPath))
                throw new ArgumentException("Invalid path!", nameof(fullPath));

            Display = display;
            FullPath = fullPath;
            Results = results;
        }

        public FormattedText Display { get; }
        public string FullPath { get; }
        public List<SearchResultViewModel> Results { get; }
    }
}
