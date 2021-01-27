using Dev.Editor.BusinessLogic.ViewModels.Projects;
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
    /// Interaction logic for ProjectTool.xaml
    /// </summary>
    public partial class ProjectTool : UserControl
    {
        public ProjectTool()
        {
            InitializeComponent();
        }

        private void HandleFileItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var fileVm = (sender as FrameworkElement).DataContext as ProjectFileViewModel;
            fileVm.NotifyDoubleClicked();
        }
    }
}
