using Dev.Editor.BusinessLogic.Models.SubstitutionCipher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Services.SubstitutionCipher
{
    public interface ISubstitutionCipherService
    {
        LanguageInfoModel BuildLanguageInfoModel(string[] lines, Func<bool> checkCancellation, Action<int> reportProgress);
        LanguageInfoModel LoadLanguageInfoModel(string filename);
        string Process(Dictionary<char, char> inputKey, string data, bool forward, Func<bool> checkCancellation = null);
        void SaveLanguageInfoModel(string filename, LanguageInfoModel model);
    }
}
