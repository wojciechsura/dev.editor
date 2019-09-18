using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BinAnalyzer.Data
{
    public abstract class BaseData
    {
        public BaseData(string name, string typeName)
        {
            this.Name = name;
            this.TypeName = typeName;
        }

        public string Name { get; }

        public string TypeName { get; }

        public abstract IList<BaseData> Children { get; }
    }    
}
