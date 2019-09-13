using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BinAnalyzer.AnalyzerDefinition.Definitions
{
    class Definitions
    {
        private readonly List<StructDefinition> structDefinitions;
        
        public Definitions(List<StructDefinition> structDefinitions)
        {
            this.structDefinitions = structDefinitions;
        }

        public IReadOnlyList<StructDefinition> StructDefinitions => structDefinitions;
    }
}
