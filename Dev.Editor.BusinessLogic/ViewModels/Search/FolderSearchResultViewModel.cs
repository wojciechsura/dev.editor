using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Dev.Editor.BusinessLogic.ViewModels.Search
{
    public class FolderSearchResultViewModel : BaseFilesystemSearchResultViewModel
    {
        public FolderSearchResultViewModel(string path, List<BaseFilesystemSearchResultViewModel> files)
        {           
            Path = path;
            Files = files;
        }

        public string Path { get; }
        public List<BaseFilesystemSearchResultViewModel> Files { get; }
    }
}
