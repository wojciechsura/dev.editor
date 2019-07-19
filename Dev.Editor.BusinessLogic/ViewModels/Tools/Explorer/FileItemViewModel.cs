using Dev.Editor.BusinessLogic.Types.Tools.Explorer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Dev.Editor.BusinessLogic.ViewModels.Tools.Explorer
{
    public class FileItemViewModel
    {
        public FileItemViewModel(FolderItemViewModel parent, string path, string display, ImageSource icon, FileItemType itemType)
        {
            Parent = parent;
            Path = path;
            Display = display;
            Icon = icon;
            ItemType = itemType;
        }

        public string GetFullPath()
        {
            string result;

            if (Parent != null)
                result = System.IO.Path.Combine(Parent.GetFullPath(), Path);
            else
                result = Path;

            return result;
        }

        public FolderItemViewModel Parent { get; }
        public string Path { get; set; }        
        public string Display { get; set; }
        public ImageSource Icon { get; set; }
        public FileItemType ItemType { get; }
    }
}
