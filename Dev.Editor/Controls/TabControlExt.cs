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

        #region IsActiveDocumentTab

        public bool IsActiveDocumentTab
        {
            get { return (bool)GetValue(IsActiveDocumentTabProperty); }
            set { SetValue(IsActiveDocumentTabProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsActiveDocumentTab.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsActiveDocumentTabProperty =
            DependencyProperty.Register("IsActiveDocumentTab", typeof(bool), typeof(TabControlExt), new PropertyMetadata(false));

        #endregion
    }
}
