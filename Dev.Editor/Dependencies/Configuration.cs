using Autofac;
using Dev.Editor.BusinessLogic.Services.AppVersion;
using Dev.Editor.BusinessLogic.Services.Dialogs;
using Dev.Editor.BusinessLogic.Services.ImageResources;
using Dev.Editor.BusinessLogic.Services.Platform;
using Dev.Editor.BusinessLogic.Services.Registry;
using Dev.Editor.Services.AppVersion;
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

namespace Dev.Editor.Dependencies
{
    public static class Configuration
    {
        public static void Configure(ContainerBuilder builder)
        {
            builder.RegisterType<DialogService>().As<IDialogService>().SingleInstance();
            builder.RegisterType<ImageResources>().As<IImageResources>().SingleInstance();
            builder.RegisterType<PlatformService>().As<IPlatformService>().SingleInstance();
            builder.RegisterType<SingleInstanceService>().SingleInstance();
            builder.RegisterType<WinAPIService>().SingleInstance();
            builder.RegisterType<RegistryService>().As<IRegistryService>().SingleInstance();
            builder.RegisterType<AppVersionService>().As<IAppVersionService>().SingleInstance();

            Dev.Editor.BusinessLogic.Dependencies.Configuration.Configure(builder);
        }
    }
}
