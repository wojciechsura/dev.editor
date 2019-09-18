using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BinAnalyzer.Data
{
    public class CharArrayData : BaseValueData
    {
        private string value;

        public CharArrayData(string name, string data)
            : base(name, $"char[{data.Length}]")
        {
            value = data;
        }

        public override dynamic GetValue()
        {
            return value;
        }
    }
}
