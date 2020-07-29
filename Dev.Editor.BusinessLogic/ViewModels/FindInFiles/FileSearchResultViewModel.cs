using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Dev.Editor.BusinessLogic.ViewModels.FindInFiles
{
    public class FileSearchResultViewModel : BaseSearchResultViewModel
    {
        public FileSearchResultViewModel(string name, ImageSource icon, List<SearchResultViewModel> results)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Invalid path!", nameof(name));

            Name = name;
            Icon = icon;
            Results = results;
        }

        public string Name { get; }
        public ImageSource Icon { get; }
        public List<SearchResultViewModel> Results { get; }
    }
}
