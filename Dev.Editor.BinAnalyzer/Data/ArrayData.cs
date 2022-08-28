using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BinAnalyzer.Data
{
    public class ArrayData<T> : BaseData
        where T : BaseData
    {       
        public ArrayData(string name, long offset, string childType, T[] children) 
            : base(name, offset, $"{childType}[{children.Length}]")
        {
            Children = children;
        }

        public string ChildType { get; }

        public override IList<BaseData> Children { get; }

        public override DataType DataType => DataType.Field;
    }
}
