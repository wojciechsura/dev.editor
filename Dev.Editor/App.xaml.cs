using Dev.Editor.BusinessLogic.Models.Events;
using Dev.Editor.BusinessLogic.Services.EventBus;
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
        private readonly IEventBus eventBus;

        public App()
        {
            Configuration.Configure(Container.Instance);

            eventBus = Dependencies.Container.Instance.Resolve<IEventBus>();

            Activated += HandleAppActivated;
        }

        private void HandleAppActivated(object sender, EventArgs e)
        {
            eventBus.Send(this, new ApplicationActivatedEvent());
        }

        private void HandleApplicationStartup(object sender, StartupEventArgs e)
        {
            var startupService = Container.Instance.Resolve<IStartupInfoService>();
            startupService.Parameters = e.Args;
        }
    }
}
