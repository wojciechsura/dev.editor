using Dev.Editor.BusinessLogic.Services.Config;
using Dev.Editor.BusinessLogic.Types.Behavior;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.ViewModels.Configuration
{
    public class BehaviorConfigurationViewModel : BaseConfigurationViewModel
    {
        // Private fields -----------------------------------------------------

        private readonly IConfigurationService configurationService;

        private CloseBehavior closeBehavior;

        // Public methods -----------------------------------------------------

        public BehaviorConfigurationViewModel(IConfigurationService configurationService)
        {
            this.configurationService = configurationService;

            closeBehavior = configurationService.Configuration.Behavior.CloseBehavior.Value;
        }

        public override void Save()
        {
            configurationService.Configuration.Behavior.CloseBehavior.Value = closeBehavior;
        }

        public override IEnumerable<string> Validate()
        {
            return new List<string>();
        }

        // Public properties -------------------------------------------------

        public override string DisplayName => Resources.Strings.Configuration_Behavior;

        public CloseBehavior CloseBehavior
        {
            get => closeBehavior;
            set => Set(ref closeBehavior, () => CloseBehavior, value);
        }
    }
}
