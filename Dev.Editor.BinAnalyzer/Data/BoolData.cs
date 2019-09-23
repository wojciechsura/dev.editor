using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BinAnalyzer.Data
{
    public class BoolData : BaseValueData
    {
        private readonly bool value;

        public BoolData(string name, bool value)
            : base(name, "bool")
        {
            this.value = value;
        }

        public override dynamic GetValue()
        {
            return value;
        }
    }
}
