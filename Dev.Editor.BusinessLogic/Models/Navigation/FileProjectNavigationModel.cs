using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Dev.Editor.BusinessLogic.Models.Navigation
{
    public class FileProjectNavigationModel : BaseNavigationModel
    {
        public FileProjectNavigationModel(string path, string group, string title, ImageSource icon, bool enabled) : base(title, group, icon, enabled)
        {
            Path = path;
        }

        public string Path { get; }
    }
}
