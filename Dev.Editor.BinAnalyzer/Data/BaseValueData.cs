using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BinAnalyzer.Data
{
    public abstract class BaseValueData : BaseData
    {
        public BaseValueData(string name, string type) : base(name, type)
        {
        }

        public abstract dynamic GetValue();

        public virtual string Value => GetValue().ToString();

        public override IList<BaseData> Children => null;

    }
}
