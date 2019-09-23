using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BinAnalyzer.AnalyzerDefinition.Definitions
{
    class SignedEnumItem
    {
        public SignedEnumItem(string name, long value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public long Value { get; }
    }
}
