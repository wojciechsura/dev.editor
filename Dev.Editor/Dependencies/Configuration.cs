using Dev.Editor.BusinessLogic.Services.Dialogs;
using Dev.Editor.BusinessLogic.Services.ImageResources;
using Dev.Editor.BusinessLogic.Services.Platform;
using Dev.Editor.BusinessLogic.Services.Registry;
using Dev.Editor.Services.Dialogs;
using Dev.Editor.Services.ImageResources;
using Dev.Editor.Services.Platform;
using Dev.Editor.Services.Registry;
using Dev.Editor.Services.SingleInstance;
using Dev.Editor.Services.WinAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Lifetime;

namespace Dev.Editor.Dependencies
{
    public static class Configuration
    {
        private static bool configured = false;

        public static void Configure(IUnityContainer container)
        {
            if (configured)
                return;

            container.RegisterType<IDialogService, DialogService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IImageResources, ImageResources>(new ContainerControlledLifetimeManager());
            container.RegisterType<IPlatformService, PlatformService>(new ContainerControlledLifetimeManager());
            container.RegisterType<SingleInstanceService>(new ContainerControlledLifetimeManager());
            container.RegisterType<WinAPIService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IRegistryService, RegistryService>(new ContainerControlledLifetimeManager());

            Dev.Editor.BusinessLogic.Dependencies.Configuration.Configure(container);

            configured = true;
        }
    }
}
