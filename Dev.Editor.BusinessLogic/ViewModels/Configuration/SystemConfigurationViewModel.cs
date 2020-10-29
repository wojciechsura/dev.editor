using Dev.Editor.BusinessLogic.Services.Paths;
using Dev.Editor.BusinessLogic.Services.Registry;
using Spooksoft.VisualStateManager.Commands;
using Spooksoft.VisualStateManager.Conditions;
using Dev.Editor.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dev.Editor.BusinessLogic.ViewModels.Configuration
{
    public class SystemConfigurationViewModel : BaseConfigurationViewModel
    {
        private const string FILE_TYPE = "*";
        private const string REG_KEY_NAME = "Dev.Editor";
        private readonly IRegistryService registryService;
        private readonly IPathService pathService;
        private readonly BaseCondition integrationDisabledCondition;
        private readonly Condition integrationEnabledCondition;
        private bool systemIntegrationState;

        private void CheckCurrentIntegrationState()
        {
            SystemIntegrationState = registryService.IsFileContextMenuEntryRegistered(FILE_TYPE, REG_KEY_NAME);
        }

        private void DoDisableIntegration()
        {
            registryService.UnregisterFileContextMenuEntry(FILE_TYPE, REG_KEY_NAME);
            CheckCurrentIntegrationState();
        }

        private void DoEnableIntegration()
        {
            var path = $"\"{pathService.AppExecutablePath}\" \"%1\"";
            var iconPath = $"{pathService.AppExecutablePath},0";

            registryService.RegisterFileContextMenuEntry(FILE_TYPE, REG_KEY_NAME, Strings.SystemIntegration_OpenInDevEditor, path, iconPath);
            CheckCurrentIntegrationState();
        }

        private void HandleSystemIntegrationStateChanged()
        {
            integrationEnabledCondition.Value = systemIntegrationState;
        }

        public SystemConfigurationViewModel(IRegistryService registryService, IPathService pathService)
        {
            this.registryService = registryService;
            this.pathService = pathService;

            integrationEnabledCondition = new Condition(systemIntegrationState);
            integrationDisabledCondition = !integrationEnabledCondition;

            CheckCurrentIntegrationState();

            EnableIntegrationCommand = new AppCommand(obj => DoEnableIntegration(), integrationDisabledCondition);
            DisableIntegrationCommand = new AppCommand(obj => DoDisableIntegration(), integrationEnabledCondition);
        }
        public override void Save()
        {
            
        }

        public override IEnumerable<string> Validate()
        {
            return null;
        }

        public ICommand DisableIntegrationCommand { get; }

        public override string DisplayName => Strings.Configuration_System;

        public ICommand EnableIntegrationCommand { get; }

        public bool SystemIntegrationState
        {
            get => systemIntegrationState;
            set => Set(ref systemIntegrationState, () => SystemIntegrationState, value, HandleSystemIntegrationStateChanged);
        }
    }
}
