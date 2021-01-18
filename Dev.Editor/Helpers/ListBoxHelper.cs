using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Dev.Editor.Helpers
{
    public static class ListBoxHelper
    {
        public static ListBoxItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is ListBoxItem))
            {
                if (source is Visual)
                    source = VisualTreeHelper.GetParent(source);
                else if (source is FrameworkContentElement element)
                {
                    source = element.Parent;
                }
            }

            return source as ListBoxItem;
        }
    }
}
