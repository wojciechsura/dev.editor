using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.ViewModels.SubstitutionCipher
{
    public interface IAlphabetEntryHandler
    {
        void NotifyChanged(AlphabetEntryViewModel alphabetEntryViewModel);
    }
}
