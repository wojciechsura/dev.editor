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
using System.Windows.Threading;
using Unity;
using Unity.Resolution;

namespace Dev.Editor
{
    /// <summary>
    /// Logika interakcji dla klasy SubstitutionCipherWindow.xaml
    /// </summary>
    public partial class SubstitutionCipherWindow : RibbonWindow, ISubstitutionCipherWindowAccess
    {
        private readonly SubstitutionCipherWindowViewModel viewModel;
        private readonly DispatcherTimer timer;

        private void HandleTimerTick(object sender, EventArgs e)
        {
            timer.Stop();
            viewModel.NotifyActionTimerElapsed();
        }

        public void RestartActionTimer()
        {
            timer.Stop();
            timer.Start();
        }

        public SubstitutionCipherWindow(ISubstitutionCipherHost host)
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += HandleTimerTick;

            viewModel = Dependencies.Container.Instance.Resolve<SubstitutionCipherWindowViewModel>(new ParameterOverride("host", host),
                new ParameterOverride("access", this));
            this.DataContext = viewModel;
        }
    }
}
