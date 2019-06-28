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
        public BaseNavigationModel(string title, ImageSource icon)
        {
            Title = title;
            Icon = icon;
        }

        public string Title { get; }
        public ImageSource Icon { get; }
    }
}
