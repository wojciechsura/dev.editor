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

        public CharData(string name, long offset, char value)
            : base(name, offset, "char")
        {
            this.value = value;
        }

        public override dynamic GetValue()
        {
            return value;
        }
    }
}
