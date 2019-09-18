using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BinAnalyzer.Data
{
    public class CharData : BaseValueData
    {
        private readonly char value;

        public CharData(string name, char value)
            : base(name, "char")
        {
            this.value = value;
        }

        public override dynamic GetValue()
        {
            return value;
        }
    }
}
