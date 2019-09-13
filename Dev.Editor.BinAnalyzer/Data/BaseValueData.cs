using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BinAnalyzer.Data
{
    public abstract class BaseValueData : BaseData
    {
        public BaseValueData(string name) : base(name)
        {
        }

        public abstract dynamic GetValue();

        public override IList<BaseData> Children => null;
    }
}
