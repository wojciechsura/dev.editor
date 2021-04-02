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
        void AddLineToLanguageStatisticsModel(LanguageStatisticsModel model, string line);
        LanguageInfoModel BuildLanguageInfoModel(LanguageStatisticsModel model);
        LanguageStatisticsModel InitializeLanguageStatisticsModel();
    }
}
