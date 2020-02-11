using Dev.Editor.BusinessLogic.Models.Configuration.BinDefinitions;
using Dev.Editor.BusinessLogic.Models.Dialogs;
using Dev.Editor.BusinessLogic.ViewModels.Base;
using Dev.Editor.Common.Commands;
using Dev.Editor.Common.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dev.Editor.BusinessLogic.ViewModels.Dialogs
{
    public class NameDialogViewModel : BaseViewModel
    {
        private string name;
        private INameDialogAccess access;

        private Condition NameNotEmptyCondition;

        private void HandleNameChanged()
        {
            NameNotEmptyCondition.Value = !string.IsNullOrWhiteSpace(Name);
        }

        private void InitializeCommands()
        {
            NameNotEmptyCondition = new Condition(!string.IsNullOrWhiteSpace(Name));

            OkCommand = new AppCommand(obj => DoOk(), NameNotEmptyCondition);
            CancelCommand = new AppCommand(obj => DoCancel());
        }

        private void DoCancel()
        {
            access.CloseDialog(null, false);
        }

        private void DoOk()
        {
            access.CloseDialog(new NameDialogResult(name), true);
        }

        public NameDialogViewModel(INameDialogAccess access, NameDialogModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            this.access = access;

            Name = model.CurrentName;
            WindowTitle = model.WindowTitle;
            GroupboxTitle = model.GroupboxTitle;

            InitializeCommands();
        }

        public ICommand OkCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public string Name
        {
            get => name;
            set
            {
                Set(ref name, () => Name, value, HandleNameChanged);
            }
        }

        public string WindowTitle { get; }
        public string GroupboxTitle { get; }
    }
}
