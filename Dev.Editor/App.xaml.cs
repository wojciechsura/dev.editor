using Dev.Editor.BusinessLogic.Models.Events;
using Dev.Editor.BusinessLogic.Services.EventBus;
using Dev.Editor.BusinessLogic.Services.StartupInfo;
using Dev.Editor.Dependencies;
using Dev.Editor.Models;
using Dev.Editor.Services.SingleInstance;
using Dev.Editor.Services.WinAPI;
using Newtonsoft.Json;
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
        private readonly SingleInstanceService singleInstanceService;
        private readonly WinAPIService winAPIService;

        public App()
        {
            Configuration.Configure(Container.Instance);

            winAPIService = Dependencies.Container.Instance.Resolve<WinAPIService>();
            singleInstanceService = Dependencies.Container.Instance.Resolve<SingleInstanceService>();
            eventBus = Dependencies.Container.Instance.Resolve<IEventBus>();
        }

        private void HandleAppActivated(object sender, EventArgs e)
        {
            eventBus.Send(this, new ApplicationActivatedEvent());
        }

        private void HandleApplicationStartup(object sender, StartupEventArgs e)
        {
            if (!singleInstanceService.IsMainInstance)
            {
                var argInfo = new ArgumentInfo
                {
                    Args = e.Args.ToList()
                };
                var serializedArgs = JsonConvert.SerializeObject(argInfo);

                var mainInstanceWindowHandle = singleInstanceService.ReadMainwWindowHandle();

                winAPIService.SendCopyData(mainInstanceWindowHandle, serializedArgs);

                this.Shutdown(0);
            }

            var startupService = Container.Instance.Resolve<IStartupInfoService>();
            startupService.Parameters = e.Args;
        }
    }
}
