using Dev.Editor.BusinessLogic.Services.Config;
using Dev.Editor.BusinessLogic.Services.Dialogs;
using Dev.Editor.BusinessLogic.Services.Messaging;
using Dev.Editor.BusinessLogic.Services.Paths;
using Dev.Editor.BusinessLogic.Services.Registry;
using Dev.Editor.BusinessLogic.ViewModels.Base;
using Spooksoft.VisualStateManager.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dev.Editor.BusinessLogic.ViewModels.Configuration
{
    public class ConfigurationWindowViewModel : BaseViewModel
    {
        // Private fields -----------------------------------------------------

        private readonly IConfigurationService configurationService;
        private readonly IRegistryService registryService;
        private readonly IPathService pathService;
        private readonly IMessagingService messagingService;

        private readonly IConfigurationWindowAccess access;

        private readonly List<BaseConfigurationViewModel> pages;
        private BaseConfigurationViewModel activePage;

        // Private methods ----------------------------------------------------

        private void DoCancel()
        {
            access.CloseWindow();
        }

        private void DoOk()
        {
            // Validate
            foreach (BaseConfigurationViewModel page in pages)
            {
                List<string> messages = page.Validate()?.ToList();
                if (messages != null && messages.Any())
                {
                    messagingService.Warn(messages.First(), Resources.Strings.Configuration_Title);                    
                    return;
                }
            }

            foreach (BaseConfigurationViewModel viewModel in pages)
            {
                viewModel.Save();
            }

            configurationService.Save();            
            access.CloseWindow();
        }

        // Public methods -----------------------------------------------------

        public ConfigurationWindowViewModel(IConfigurationService configurationService, IMessagingService messagingService, IRegistryService registryService, IPathService pathService, IConfigurationWindowAccess access)
        {
            this.configurationService = configurationService;
            this.messagingService = messagingService;
            this.registryService = registryService;
            this.pathService = pathService;
            this.access = access;

            pages = new List<BaseConfigurationViewModel>
            {
                new BehaviorConfigurationViewModel(configurationService),
                new SystemConfigurationViewModel(registryService, pathService)
            };
            activePage = pages.First();

            OkCommand = new AppCommand(obj => DoOk());
            CancelCommand = new AppCommand(obj => DoCancel());
        }

        // Public properties --------------------------------------------------

        public ICommand OkCommand { get; }

        public ICommand CancelCommand { get; }

        public IReadOnlyList<BaseConfigurationViewModel> Pages => pages;

        public BaseConfigurationViewModel ActivePage
        {
            get => activePage;
            set => Set(ref activePage, () => ActivePage, value);
        }
    }
}
