using Dev.Editor.BusinessLogic.ViewModels;
using Dev.Editor.BusinessLogic.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
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
    public partial class DocumentView : UserControl, ITextEditorAccess
    {
        private void HandleDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is DocumentViewModel oldViewModel)
            {
                oldViewModel.TextEditorAccess = null;
            }

            if (e.NewValue is DocumentViewModel newViewModel)
            {
                newViewModel.TextEditorAccess = this;
            }
        }

        public DocumentView()
        {
            InitializeComponent();
            DataContextChanged += HandleDataContextChanged;
        }        
    }
}
