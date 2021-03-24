using Dev.Editor.BusinessLogic.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.ViewModels.SubstitutionCipher
{
    public class AlphabetEntryViewModel : BaseViewModel
    {
        private readonly char plaintext;
        private char? cipher;
        private bool isDoubled;

        public AlphabetEntryViewModel(char plaintext)
        {
            this.plaintext = plaintext;
            cipher = null;
            isDoubled = false;
        }

        public string Plaintext => plaintext.ToString();

        public string Cipher
        {
            get => cipher.ToString();
            set
            {
                if (string.IsNullOrEmpty(value))
                    cipher = null;
                else
                    cipher = value[0];

                OnPropertyChanged(() => Cipher);
            }
        }

        public bool IsDoubled
        {
            get => isDoubled;
            set => Set(ref isDoubled, () => isDoubled, value);
        }
    }
}
