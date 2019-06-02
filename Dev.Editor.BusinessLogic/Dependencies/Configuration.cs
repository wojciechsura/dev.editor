using Dev.Editor.BusinessLogic.Services.Dialogs;
using Dev.Editor.BusinessLogic.Services.Messaging;
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
            container.RegisterType<IDialogService, DialogService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IMessagingService, MessagingService>(new ContainerControlledLifetimeManager());
        }
    }
}
