using Dev.Editor.BinAnalyzer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BinAnalyzer.AnalyzerDefinition.Definitions
{
    class BaseEnumItem
    {
        private string name;

        public BaseEnumItem(string name)
        {
            this.name = name;
        }

        public string Name => name;
    }
}
