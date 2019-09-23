using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BinAnalyzer.AnalyzerDefinition.Definitions
{
    class UnsignedEnumDefinition : BaseDefinition
    {
        private readonly string type;
        private readonly List<UnsignedEnumItem> items;

        public UnsignedEnumDefinition(string name, string type, List<UnsignedEnumItem> items)
            : base(name)
        {
            this.type = type;
            this.items = items;
        }

        public List<UnsignedEnumItem> Items => items;

        public string Type => type;
    }
}
