using Dev.Editor.BinAnalyzer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BinAnalyzer.AnalyzerDefinition.Definitions
{
    abstract class BaseEnumDefinition : BaseDefinition
    {
        private readonly string type;

        public BaseEnumDefinition(string name, string type)
            : base(name)
        {
            this.type = type;
        }

        public abstract BaseData GenerateEnumData(string field, long offset, string memberName);

        public string Type => type;
    }
}
