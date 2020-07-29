using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Dev.Editor.BusinessLogic.ViewModels.FindInFiles
{
    public class FolderSearchResultViewModel : BaseSearchResultViewModel
    {
        public FolderSearchResultViewModel(string name, ImageSource icon, List<BaseSearchResultViewModel> files)
        {           
            Name = name;
            Icon = icon;
            Files = files;
        }

        public string Name { get; }
        public ImageSource Icon { get; }
        public List<BaseSearchResultViewModel> Files { get; }
    }
}
