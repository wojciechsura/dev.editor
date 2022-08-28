using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BinAnalyzer.Data
{
    public abstract class BaseValueData : BaseData
    {
        public BaseValueData(string name, long offset, string type) 
            : base(name, offset, type)
        {

        }

        public abstract dynamic GetValue();

        public virtual string Value => GetValue().ToString();

        public override IList<BaseData> Children => null;

        public override DataType DataType => DataType.Field;
    }
}
