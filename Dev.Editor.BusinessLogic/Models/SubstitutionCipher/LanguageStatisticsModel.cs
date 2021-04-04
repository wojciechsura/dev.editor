using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.SubstitutionCipher
{
    public class LanguageStatisticsModel
    {
        public LanguageStatisticsModel()
        {
            Sequences = new Dictionary<int, int[]>();
            LetterStats = new Dictionary<char, int>();
        }

        public Dictionary<int, int[]> Sequences { get; }
        public Dictionary<char, int> LetterStats { get; }
    }
}
