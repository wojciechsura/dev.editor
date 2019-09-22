using Dev.Editor.BusinessLogic.Models.Messages;
using Dev.Editor.BusinessLogic.ViewModels.BottomTools.Messages;
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
    /// Interaction logic for MessagesBottomTool.xaml
    /// </summary>
    public partial class MessagesBottomTool : UserControl
    {
        private MessagesBottomToolViewModel viewModel;

        private void HandleGridRowMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var msg = ((sender as DataGridRow)?.Item as MessageModel);
            if (msg != null)
                viewModel.NotifyMessageChosen(msg);            
        }

        private void HandleDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            viewModel = e.NewValue as MessagesBottomToolViewModel;
        }

        public MessagesBottomTool()
        {
            InitializeComponent();
        }
    }
}
