using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.SubstitutionCipher
{
    public class LanguageInfoModel
    {
        public LanguageInfoModel(Dictionary<char, double> letterFrequencies, Dictionary<int, Dictionary<string, SequenceInfoModel>> sequenceFrequencies)
        {
            LetterFrequencies = letterFrequencies;
            SequenceFrequencies = sequenceFrequencies;
        }

        public Dictionary<char, double> LetterFrequencies { get; }
        public Dictionary<int, Dictionary<string, SequenceInfoModel>> SequenceFrequencies { get; }
    }
}
