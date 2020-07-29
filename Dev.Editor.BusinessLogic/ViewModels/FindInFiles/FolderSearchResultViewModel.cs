using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Dev.Editor.BusinessLogic.ViewModels.FindInFiles
{
    public class FolderSearchResultViewModel : BaseFilesystemSearchResultViewModel
    {
        public FolderSearchResultViewModel(string name, ImageSource icon, List<BaseFilesystemSearchResultViewModel> files)
        {           
            Name = name;
            Icon = icon;
            Files = files;

            Count = files.Sum(f => f.Count);
        }

        public string Name { get; }
        public ImageSource Icon { get; }
        public List<BaseFilesystemSearchResultViewModel> Files { get; }

        public override int Count { get; }

        public string Display => $"{Name} ({Count})";
    }
}
