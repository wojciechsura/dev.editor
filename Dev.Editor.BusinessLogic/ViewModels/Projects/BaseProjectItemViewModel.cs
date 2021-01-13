using Dev.Editor.BusinessLogic.Types.PropertyChange;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Dev.Editor.BusinessLogic.ViewModels.Projects
{
    public class BaseProjectItemViewModel : BasePropertyChangeNotifier
    {
        private string path;
        private string filename;
        private ImageSource icon;

        public BaseProjectItemViewModel(string path, ImageSource icon)
        {
            this.path = path;
            this.icon = icon;
            filename = System.IO.Path.GetFileName(path);
        }

        public string Path => path;
        public string Filename => filename;
        public ImageSource Icon => icon;
    }
}
