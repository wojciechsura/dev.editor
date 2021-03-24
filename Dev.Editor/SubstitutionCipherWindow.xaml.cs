using Dev.Editor.BusinessLogic.ViewModels.SubstitutionCipher;
using Fluent;
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
using Unity;
using Unity.Resolution;

namespace Dev.Editor
{
    /// <summary>
    /// Logika interakcji dla klasy SubstitutionCipherWindow.xaml
    /// </summary>
    public partial class SubstitutionCipherWindow : RibbonWindow
    {
        private readonly SubstitutionCipherWindowViewModel viewModel;

        public SubstitutionCipherWindow(ISubstitutionCipherHost host)
        {
            InitializeComponent();

            viewModel = Dependencies.Container.Instance.Resolve<SubstitutionCipherWindowViewModel>(new ParameterOverride("host", host));
            this.DataContext = viewModel;
        }
    }
}
