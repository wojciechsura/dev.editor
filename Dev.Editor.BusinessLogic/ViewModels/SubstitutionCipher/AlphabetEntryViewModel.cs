﻿using Dev.Editor.BusinessLogic.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.ViewModels.SubstitutionCipher
{
    public class AlphabetEntryViewModel : BaseViewModel
    {
        private readonly IAlphabetEntryHandler handler;
        private readonly string plaintext;
        private string previousCipher;
        private string cipher;
        private bool isDoubled;

        private void HandleCipherChanged()
        {
            handler.NotifyChanged(this, previousCipher);
            previousCipher = cipher;
        }

        public AlphabetEntryViewModel(char plaintext, IAlphabetEntryHandler handler)
        {
            this.plaintext = plaintext.ToString();
            this.handler = handler;
            cipher = null;
            isDoubled = false;
        }

        public void SetCipherSilently(string newCipher)
        {
            cipher = newCipher;
            previousCipher = newCipher;
            OnPropertyChanged(() => Cipher);
        }

        public string Plaintext => plaintext.ToString();

        public string Cipher
        {
            get => cipher;
            set => Set(ref cipher, () => Cipher, value?.ToLowerInvariant(), HandleCipherChanged);
        }

        public bool IsDoubled
        {
            get => isDoubled;
            set => Set(ref isDoubled, () => IsDoubled, value);
        }
    }
}
