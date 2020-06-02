using Dev.Editor.BusinessLogic.Services.Commands;
using Dev.Editor.BusinessLogic.Services.Config;
using Dev.Editor.BusinessLogic.Services.Dialogs;
using Dev.Editor.BusinessLogic.Services.EventBus;
using Dev.Editor.BusinessLogic.Services.FileIcons;
using Dev.Editor.BusinessLogic.Services.Highlighting;
using Dev.Editor.BusinessLogic.Services.Messaging;
using Dev.Editor.BusinessLogic.Services.Paths;
using Dev.Editor.BusinessLogic.Services.Registry;
using Dev.Editor.BusinessLogic.Services.SearchEncoder;
using Dev.Editor.BusinessLogic.Services.StartupInfo;
using Dev.Editor.BusinessLogic.Services.TextComparison;
using Dev.Editor.BusinessLogic.Services.TextTransform;
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
        private static bool isConfigured = false;

        public static void Configure(IUnityContainer container)
        {
            if (isConfigured)
                return;
            isConfigured = true;

            container.RegisterType<IMessagingService, MessagingService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IConfigurationService, ConfigrationService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IPathService, PathService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IStartupInfoService, StartupInfoService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IHighlightingProvider, HighlightingProvider>(new ContainerControlledLifetimeManager());
            container.RegisterType<ICommandRepositoryService, CommandRepositoryService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IFileIconProvider, FileIconProvider>(new ContainerControlledLifetimeManager());
            container.RegisterType<IEventBus, EventBus>(new ContainerControlledLifetimeManager());
            container.RegisterType<ISearchEncoderService, SearchEncoderService>(new ContainerControlledLifetimeManager());
            container.RegisterType<ITextComparisonService, TextComparisonService>(new ContainerControlledLifetimeManager());
            container.RegisterType<ITextTransformService, TextTransformService>(new ContainerControlledLifetimeManager());
        }
    }
}
