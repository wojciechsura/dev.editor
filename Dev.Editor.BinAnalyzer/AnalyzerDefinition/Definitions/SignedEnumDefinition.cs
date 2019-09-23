using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BinAnalyzer.AnalyzerDefinition.Definitions
{
    class SignedEnumDefinition : BaseDefinition
    {
        private readonly string type;
        private readonly List<SignedEnumItem> items;

        public SignedEnumDefinition(string name, string type, List<SignedEnumItem> items) 
            : base(name)
        {
            this.type = type;
            this.items = items;
        }

        public List<SignedEnumItem> Items => items;

        public string Type => type;
    }
}
