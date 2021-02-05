using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Dev.Editor.BusinessLogic.Models.Navigation
{
    public class BaseNavigationModel
    {
        public BaseNavigationModel(string title, string group, ImageSource icon, bool enabled)
        {
            Title = title;
            Group = group;
            Icon = icon;
            Enabled = enabled;
        }

        public string Title { get; }
        public string Group { get; }
        public ImageSource Icon { get; }
        public bool Enabled { get; }
    }
}
