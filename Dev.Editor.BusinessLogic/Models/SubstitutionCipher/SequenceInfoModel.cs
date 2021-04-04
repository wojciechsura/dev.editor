using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.SubstitutionCipher
{
    public class SequenceInfoModel
    {
        public SequenceInfoModel(int count, double frequency, double fitness)
        {
            Count = count;
            Frequency = frequency;
            Fitness = fitness;
        }

        public int Count { get; }
        public double Frequency { get; }
        public double Fitness { get; }
    }
}
