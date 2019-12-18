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
        private readonly List<BaseEnumDefinition> enumDefinitions;
        
        public Definitions(List<StructDefinition> structDefinitions, 
            List<BaseEnumDefinition> enumDefinitions)
        {
            this.structDefinitions = structDefinitions;
            this.enumDefinitions = enumDefinitions;
        }

        public IReadOnlyList<StructDefinition> StructDefinitions => structDefinitions;

        internal IReadOnlyList<BaseEnumDefinition> EnumDefinitions => enumDefinitions;
    }
}
