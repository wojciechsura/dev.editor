using Dev.Editor.BusinessLogic.Types.SubstitutionCipher;
using Dev.Editor.BusinessLogic.ViewModels.Base;
using ICSharpCode.AvalonEdit.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.ViewModels.SubstitutionCipher
{
    public class SubstitutionCipherWindowViewModel : BaseViewModel
    {
        // Private fields -----------------------------------------------------

        private readonly ISubstitutionCipherHost host;
        private readonly TextDocument plaintextDoc;
        private readonly TextDocument cipherDoc;
        private SubstitutionCipherMode mode;

        // Public methods -----------------------------------------------------

        public SubstitutionCipherWindowViewModel(ISubstitutionCipherHost host)
        {
            this.host = host;

            mode = SubstitutionCipherMode.Cipher;
            plaintextDoc = new TextDocument();
            cipherDoc = new TextDocument();
        }

        // Public properties --------------------------------------------------

        public TextDocument PlaintextDoc => plaintextDoc;

        public TextDocument CipherDoc => cipherDoc;

        public SubstitutionCipherMode Mode
        {
            get => mode;
            set => Set(ref mode, () => Mode, value);
        }
    }
}
