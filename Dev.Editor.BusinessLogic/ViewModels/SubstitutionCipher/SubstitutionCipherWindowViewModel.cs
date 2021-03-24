using Dev.Editor.BusinessLogic.Services.Dialogs;
using Dev.Editor.BusinessLogic.Types.SubstitutionCipher;
using Dev.Editor.BusinessLogic.ViewModels.Base;
using ICSharpCode.AvalonEdit.Document;
using Spooksoft.VisualStateManager.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dev.Editor.BusinessLogic.ViewModels.SubstitutionCipher
{
    public class SubstitutionCipherWindowViewModel : BaseViewModel
    {
        // Private fields -----------------------------------------------------

        private readonly ISubstitutionCipherHost host;
        private readonly IDialogService dialogService;
        
        private readonly TextDocument plaintextDoc;
        private readonly TextDocument cipherDoc;
        private SubstitutionCipherMode mode;

        private readonly ObservableCollection<AlphabetEntryViewModel> alphabet = new ObservableCollection<AlphabetEntryViewModel>();

        // Private methods ----------------------------------------------------

        private void DoEnterAlphabet()
        {
            string previousAlphabet = null;
            if (alphabet != null)
                previousAlphabet = String.Join("", alphabet
                    .Select(e => e.Plaintext)
                    .OrderBy(x => x));

            (bool result, string newAlphabet) = dialogService.ShowAlphabetDialog(previousAlphabet);
            if (result)
            {
                // Move entries from old to new alphabet
                var previous = alphabet.ToList();
                alphabet.Clear();

                if (!string.IsNullOrEmpty(newAlphabet))
                {
                    foreach (char ch in newAlphabet.Distinct().OrderBy(x => x))
                    {
                        var entry = new AlphabetEntryViewModel(ch);
                        var oldEntry = previous.FirstOrDefault(e => e.Plaintext.Equals(entry.Plaintext));
                        if (oldEntry != null)
                            entry.Cipher = oldEntry.Cipher;

                        alphabet.Add(entry);
                    }
                }
            }
        }

        // Public methods -----------------------------------------------------

        public SubstitutionCipherWindowViewModel(IDialogService dialogService, ISubstitutionCipherHost host)
        {
            this.dialogService = dialogService;
            this.host = host;

            mode = SubstitutionCipherMode.Cipher;
            plaintextDoc = new TextDocument();
            cipherDoc = new TextDocument();

            EnterAlphabetCommand = new AppCommand(obj => DoEnterAlphabet());
        }

        // Public properties --------------------------------------------------

        public TextDocument PlaintextDoc => plaintextDoc;

        public TextDocument CipherDoc => cipherDoc;

        public SubstitutionCipherMode Mode
        {
            get => mode;
            set => Set(ref mode, () => Mode, value);
        }

        public ObservableCollection<AlphabetEntryViewModel> Alphabet => alphabet;

        public ICommand EnterAlphabetCommand { get; }
    }
}
