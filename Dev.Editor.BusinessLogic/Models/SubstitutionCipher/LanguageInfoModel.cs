using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.SubstitutionCipher
{
    public class LanguageInfoModel
    {
        public LanguageInfoModel(Dictionary<char, int> alphabet, 
            Dictionary<char, double> letterFrequencies, 
            Dictionary<int, SequenceInfoModel[]> sequenceFrequencies)
        {
            Alphabet = alphabet;
            LetterFrequencies = letterFrequencies;
            SequenceFrequencies = sequenceFrequencies;
        }

        public Dictionary<char, int> Alphabet { get; }
        public Dictionary<char, double> LetterFrequencies { get; }
        public Dictionary<int, SequenceInfoModel[]> SequenceFrequencies { get; }
    }
}
