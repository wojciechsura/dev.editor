using Dev.Editor.BusinessLogic.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Dev.Editor.Controls
{
    public class TextEditorUserControl : UserControl
    {
        public TextEditorUserControl()
        {
        }

        public ITextEditorAccess TextEditorAccess
        {
            get { return (ITextEditorAccess)GetValue(TextEditorAccessProperty); }
            set { SetValue(TextEditorAccessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextEditorAccess.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextEditorAccessProperty =
            DependencyProperty.Register("TextEditorAccess", typeof(ITextEditorAccess), typeof(DocumentView), new PropertyMetadata(null));
    }
}
