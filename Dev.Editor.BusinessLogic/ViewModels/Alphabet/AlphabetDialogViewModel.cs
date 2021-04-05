using Dev.Editor.BusinessLogic.ViewModels.Base;
using Spooksoft.VisualStateManager.Commands;
using Spooksoft.VisualStateManager.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dev.Editor.BusinessLogic.ViewModels.Alphabet
{
    public class AlphabetDialogViewModel : BaseViewModel
    {
        private LambdaCondition<AlphabetDialogViewModel> alphabetNonEmptyCondition;
        private readonly IAlphabetDialogWindowAccess access;
        private string alphabet;

        private void DoCancel()
        {
            access.Close(false);            
        }

        private void DoOk()
        {
            access.Close(true);
        }

        public string Alphabet
        {
            get => alphabet;
            set => Set(ref alphabet, () => Alphabet, value);
        }

        public AlphabetDialogViewModel(IAlphabetDialogWindowAccess access, string message, string previousAlphabet = null)
        {
            if (string.IsNullOrEmpty(previousAlphabet))
                alphabet = "abcdefghijklmnopqrstuvwxyz";
            else
                alphabet = previousAlphabet;

            Message = message;

            alphabetNonEmptyCondition = new LambdaCondition<AlphabetDialogViewModel>(this, vm => vm.Alphabet != null, false);

            OkCommand = new AppCommand(obj => DoOk(), alphabetNonEmptyCondition);
            CancelCommand = new AppCommand(obj => DoCancel());
            this.access = access;
        }

        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        public string Message { get; }
    }
}
