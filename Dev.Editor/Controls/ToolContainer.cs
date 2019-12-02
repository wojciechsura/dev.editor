using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace Dev.Editor.Controls
{
    [ContentProperty(nameof(ToolContainer.Content))]
    public class ToolContainer : Control
    {
        #region Header dependency property

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =  
            DependencyProperty.Register("Header", typeof(string), typeof(ToolContainer), new PropertyMetadata(""));

        #endregion

        #region Content dependency property

        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(ToolContainer), new PropertyMetadata(null));

        #endregion

        #region CloseCommand dependency property

        public ICommand CloseCommand
        {
            get { return (ICommand)GetValue(CloseCommandProperty); }
            set { SetValue(CloseCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CloseCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CloseCommandProperty =
            DependencyProperty.Register("CloseCommand", typeof(ICommand), typeof(ToolContainer), new PropertyMetadata(null));

        #endregion
    }
}
