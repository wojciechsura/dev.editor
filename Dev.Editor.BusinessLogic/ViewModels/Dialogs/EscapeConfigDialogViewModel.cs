using Dev.Editor.BusinessLogic.Models.Configuration.StoredDefaults;
using Dev.Editor.BusinessLogic.Models.Dialogs;
using Dev.Editor.BusinessLogic.Services.Config;
using Dev.Editor.BusinessLogic.ViewModels.Base;
using Spooksoft.VisualStateManager.Commands;
using Spooksoft.VisualStateManager.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dev.Editor.BusinessLogic.ViewModels.Dialogs
{
    public class EscapeConfigDialogViewModel : BaseViewModel
    {
        private readonly IConfigurationService configurationService;

        private readonly SimpleCondition escapeCharacterEnteredCondition;
        private readonly IEscapeConfigDialogAccess access;

        private readonly bool forward;

        private string escapeChar;
        private bool includeDoubleQuotes;
        private bool includeSingleQuotes;
        private bool includeSpecialCharacters;

        private void DoOk()
        {
            EscapeDefaults escapeDefaults = configurationService.Configuration.StoredDefaults.EscapeDefaults;

            escapeDefaults.EscapeCharacter.Value = escapeChar;
            escapeDefaults.IncludeDoubleQuotes.Value = includeDoubleQuotes;
            escapeDefaults.IncludeSingleQuotes.Value = includeSingleQuotes;
            escapeDefaults.IncludeSpecialCharacters.Value = includeSpecialCharacters;

            configurationService.Save();

            access.Close(new EscapeConfigResult(escapeChar, includeSingleQuotes, includeDoubleQuotes, includeSpecialCharacters), true);
        }

        private void DoCancel()
        {
            access.Close(null, false);
        }

        private void EscapeCharChanged()
        {
            escapeCharacterEnteredCondition.Value = (escapeChar?.Length ?? 0) == 1;
        }

        public EscapeConfigDialogViewModel(IConfigurationService configurationService, IEscapeConfigDialogAccess access, EscapeConfigModel model)
        {
            this.configurationService = configurationService;
            this.access = access;
            this.forward = model.Forward;

            EscapeDefaults escapeDefaults = configurationService.Configuration.StoredDefaults.EscapeDefaults;
            escapeChar = escapeDefaults.EscapeCharacter.Value;
            includeDoubleQuotes = escapeDefaults.IncludeDoubleQuotes.Value;
            includeSingleQuotes = escapeDefaults.IncludeSingleQuotes.Value;
            includeSpecialCharacters = escapeDefaults.IncludeSpecialCharacters.Value;

            escapeCharacterEnteredCondition = new SimpleCondition((escapeChar?.Length ?? 0) == 1);

            OkCommand = new AppCommand(obj => DoOk(), escapeCharacterEnteredCondition);
            CancelCommand = new AppCommand(obj => DoCancel());
        }

        public string EscapeChar
        {
            get => escapeChar;
            set => Set(ref escapeChar, () => EscapeChar, value, EscapeCharChanged, false);           
        }

        public bool IncludeDoubleQuotes
        {
            get => includeDoubleQuotes;
            set => Set(ref includeDoubleQuotes, () => IncludeDoubleQuotes, value);
        }

        public bool IncludeSingleQuotes
        {
            get => includeSingleQuotes;
            set => Set(ref includeSingleQuotes, () => IncludeSingleQuotes, value);
        }

        public bool IncludeSpecialCharacters
        {
            get => includeSpecialCharacters;
            set => Set(ref includeSpecialCharacters, () => IncludeSpecialCharacters, value);
        }

        public string Title
        {
            get
            {
                if (forward)
                    return Resources.Strings.ConfigureEscapeDialog_ForwardTitle;
                else
                    return Resources.Strings.ConfigureEscapeDialog_BackwardTitle;
            }
        }

        public bool Forward => forward;

        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }
    }
}
