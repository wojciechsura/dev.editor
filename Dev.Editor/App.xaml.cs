using Dev.Editor.BusinessLogic.Services.StartupInfo;
using Dev.Editor.Dependencies;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Unity;

namespace Dev.Editor
{
    /// <summary>
    /// Logika interakcji dla klasy App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Configuration.Configure(Container.Instance);            
        }

        private void HandleApplicationStartup(object sender, StartupEventArgs e)
        {
            var startupService = Container.Instance.Resolve<IStartupInfoService>();
            startupService.Parameters = e.Args;
        }
    }
}
