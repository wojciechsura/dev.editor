using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Dev.Editor.Controls
{
    public class TabControlExt : TabControl
    {
        #region HeaderDoubleclickCommand

        public ICommand HeaderDoubleClickCommand
        {
            get { return (ICommand)GetValue(HeaderDoubleClickCommandProperty); }
            set { SetValue(HeaderDoubleClickCommandProperty, value); }
        }

        public static readonly DependencyProperty HeaderDoubleClickCommandProperty =
            DependencyProperty.Register("HeaderDoubleClickCommand", typeof(ICommand), typeof(TabControlExt), new PropertyMetadata(null));

        #endregion
    }
}
