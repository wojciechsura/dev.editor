using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BinAnalyzer.Data
{
    public abstract class BaseData
    {
        public BaseData(string name)
        {
            this.Name = name;
        }

        public string Name { get; }

        public abstract IList<BaseData> Children { get; }
    }    
}
