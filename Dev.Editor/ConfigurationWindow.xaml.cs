using Autofac;
using Dev.Editor.BusinessLogic.ViewModels.Configuration;
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
using System.Windows.Shapes;

namespace Dev.Editor
{
    /// <summary>
    /// Logika interakcji dla klasy ConfigWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : Window, IConfigurationWindowAccess
    {
        // Private fields -----------------------------------------------------

        private ConfigurationWindowViewModel viewModel;

        // Public methods -----------------------------------------------------

        public ConfigurationWindow()
        {
            InitializeComponent();

            viewModel = Dependencies.Container.Instance.Resolve<ConfigurationWindowViewModel>(new NamedParameter("access", this));
            DataContext = viewModel;
        }

        public void CloseWindow()
        {
            Close();
        }
    }
}
