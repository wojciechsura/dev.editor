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
        LanguageInfoModel BuildLanguageInfoModel(string[] lines, string alphabet, Func<bool> checkCancellation, Action<int> reportProgress);
        LanguageInfoModel LoadLanguageInfoModel(string filename);
        string Process(Dictionary<char, char> inputKey, string data, bool forward, bool useUnrecognizedCharsDirectly, Func<bool> checkCancellation = null);
        void SaveLanguageInfoModel(string filename, LanguageInfoModel model);
        Dictionary<char, char> TrySolve(string cipher, Dictionary<char, char> initialKey, LanguageInfoModel languageInfo, Func<bool> checkCancellation = null, Action<int> reportProgress = null);
        string ExtractAlphabet(string[] lines);
    }
}
