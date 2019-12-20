using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Dev.Editor.Controls
{
    public class TreeViewEx : TreeView
    {
        private ScrollViewer scrollViewer;

        public override void OnApplyTemplate()
        {
            scrollViewer = this.Template.FindName("PART_Scroll", this) as ScrollViewer;

            base.OnApplyTemplate();
        }

        public double VerticalScrollOffset
        {
            get
            {
                return scrollViewer?.VerticalOffset ?? 0.0;
            }
            set
            {
                if (scrollViewer != null)
                    scrollViewer.ScrollToVerticalOffset(value);
            }
        }

        public double HorizontalScrollOffset
        {
            get
            {
                return scrollViewer?.HorizontalOffset ?? 0.0;
            }
            set
            {
                if (scrollViewer != null)
                    scrollViewer.ScrollToHorizontalOffset(value);
            }
        }
    }
}
