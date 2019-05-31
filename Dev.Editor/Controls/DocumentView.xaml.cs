using Dev.Editor.BusinessLogic.Models.Documents;
using Dev.Editor.BusinessLogic.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Dev.Editor.Controls
{
    /// <summary>
    /// Logika interakcji dla klasy DocumentView.xaml
    /// </summary>
    public partial class DocumentView : UserControl
    {
        public DocumentView()
        {
            InitializeComponent();
        }

        private void HandleLoaded(object sender, RoutedEventArgs e)
        {
            var state = ((DocumentViewModel)DataContext).LoadState();

            if (state != null)
            {
                teEditor.CaretOffset = state.CaretOffset;
                teEditor.SelectionStart = state.SelectionStart;
                teEditor.SelectionLength = state.SelectionLength;
                teEditor.ScrollToVerticalOffset(state.VerticalOffset);
                teEditor.ScrollToHorizontalOffset(state.HorizontalOffset);
            }

            teEditor.Focus();
        }

        private void HandleUnloaded(object sender, RoutedEventArgs e)
        {
            var state = new DocumentState(teEditor.CaretOffset, 
                teEditor.SelectionStart, 
                teEditor.SelectionLength, 
                teEditor.HorizontalOffset, 
                teEditor.VerticalOffset);
            ((DocumentViewModel)DataContext).SaveState(state);
        }
    }
}
