using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BinAnalyzer.Data
{
    public abstract class BaseData
    {
        public BaseData(string name, long offset, string typeName)
        {
            this.Name = name;
            this.Offset = offset;
            this.TypeName = typeName;
        }

        public string Name { get; }
        public long Offset { get; }
        public string TypeName { get; }

        public string OffsetDisplay
        {
            get
            {
                if (Offset < 0)
                    return string.Empty;
                else
                    return $"{Offset:X8}";
            }
        }

        public abstract IList<BaseData> Children { get; }

        public abstract DataType DataType { get; }

        public bool IsExpanded { get; set; } = false;

        public bool IsSelected { get; set; } = false;
    }    
}
