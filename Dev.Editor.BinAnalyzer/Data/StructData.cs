using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BinAnalyzer.Data
{
    public class StructData : BaseData
    {
        public StructData(string name, long offset, string type, List<BaseData> members)
            : base(name, offset, $"struct {type}")
        {
            Type = type;
            Children = members;
        }

        public string Type { get; }

        public string Value => String.Empty;

        public override IList<BaseData> Children { get; }

        public override DataType DataType => DataType.Struct;
    }
}
