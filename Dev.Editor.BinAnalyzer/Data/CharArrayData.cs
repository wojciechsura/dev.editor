using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BinAnalyzer.Data
{
    public class CharArrayData : BaseValueData
    {
        private char[] value;

        public CharArrayData(string name, long offset, char[] data)
            : base(name, offset, $"char[{data.Length}]")
        {
            value = data;
        }

        public override dynamic GetValue()
        {
            return new string(value);
        }
    }
}
