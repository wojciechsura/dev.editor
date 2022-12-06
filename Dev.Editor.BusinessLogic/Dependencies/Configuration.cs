using Autofac;
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
using Dev.Editor.BusinessLogic.Services.SubstitutionCipher;
using Dev.Editor.BusinessLogic.Services.TextComparison;
using Dev.Editor.BusinessLogic.Services.TextTransform;
using Dev.Editor.BusinessLogic.ViewModels.Dialogs.DuplicatedLines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Dependencies
{
    public static class Configuration
    {
        private static bool isConfigured = false;

        public static void Configure(ContainerBuilder builder)
        {
            if (isConfigured)
                return;
            isConfigured = true;

            builder.RegisterType<MessagingService>().As<IMessagingService>().SingleInstance();
            builder.RegisterType<ConfigurationService>().As<IConfigurationService>().SingleInstance();
            builder.RegisterType<PathService>().As<IPathService>().SingleInstance();
            builder.RegisterType<StartupInfoService>().As<IStartupInfoService>().SingleInstance();
            builder.RegisterType<HighlightingProvider>().As<IHighlightingProvider>().SingleInstance();
            builder.RegisterType<CommandRepositoryService>().As<ICommandRepositoryService>().SingleInstance();
            builder.RegisterType<FileIconProvider>().As<IFileIconProvider>().SingleInstance();
            builder.RegisterType<EventBus>().As<IEventBus>().SingleInstance();
            builder.RegisterType<SearchEncoderService>().As<ISearchEncoderService>().SingleInstance();
            builder.RegisterType<TextComparisonService>().As<ITextComparisonService>().SingleInstance();
            builder.RegisterType<TextTransformService>().As<ITextTransformService>().SingleInstance();
            builder.RegisterType<SubstitutionCipherService>().As<ISubstitutionCipherService>().SingleInstance();

            builder.RegisterType<DuplicatedLinesFinderConfigDialogViewModel>().As<DuplicatedLinesFinderConfigDialogViewModel>().SingleInstance();
        }
    }
}
