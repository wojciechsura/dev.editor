using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.SubstitutionCipher
{
    public class SequenceInfoModel
    {
        public SequenceInfoModel(double frequency, double fitness)
        {
            Frequency = frequency;
            Fitness = fitness;
        }

        public double Frequency { get; }
        public double Fitness { get; }
    }
}
