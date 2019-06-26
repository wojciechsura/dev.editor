using Dev.Editor.BusinessLogic.Services.Commands;
using Dev.Editor.BusinessLogic.Services.Config;
using Dev.Editor.BusinessLogic.Services.Dialogs;
using Dev.Editor.BusinessLogic.Services.Highlighting;
using Dev.Editor.BusinessLogic.Services.Messaging;
using Dev.Editor.BusinessLogic.Services.Paths;
using Dev.Editor.BusinessLogic.Services.StartupInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Lifetime;

namespace Dev.Editor.BusinessLogic.Dependencies
{
    public static class Configuration
    {
        public static void Configure(IUnityContainer container)
        {
            container.RegisterType<IMessagingService, MessagingService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IConfigurationService, ConfigrationService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IPathService, PathService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IStartupInfoService, StartupInfoService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IHighlightingProvider, HighlightingProvider>(new ContainerControlledLifetimeManager());
            container.RegisterType<ICommandRepositoryService, CommandRepositoryService>(new ContainerControlledLifetimeManager());
        }
    }
}
