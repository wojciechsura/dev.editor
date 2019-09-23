using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BinAnalyzer.AnalyzerDefinition.Definitions
{
    class UnsignedEnumItem
    {
        public UnsignedEnumItem(string name, ulong value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public ulong Value { get; }
    }
}
