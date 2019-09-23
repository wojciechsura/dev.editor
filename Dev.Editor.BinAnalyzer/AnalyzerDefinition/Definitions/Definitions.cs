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
        private readonly List<UnsignedEnumDefinition> unsignedEnumDefinitions;
        private readonly List<SignedEnumDefinition> signedEnumDefinitions;
        
        public Definitions(List<StructDefinition> structDefinitions, 
            List<UnsignedEnumDefinition> unsignedEnumDefinitions, 
            List<SignedEnumDefinition> signedEnumDefinitions)
        {
            this.structDefinitions = structDefinitions;
            this.unsignedEnumDefinitions = unsignedEnumDefinitions;
            this.signedEnumDefinitions = signedEnumDefinitions;
        }

        public IReadOnlyList<StructDefinition> StructDefinitions => structDefinitions;

        internal IReadOnlyList<UnsignedEnumDefinition> UnsignedEnumDefinitions => unsignedEnumDefinitions;

        internal IReadOnlyList<SignedEnumDefinition> SignedEnumDefinitions => signedEnumDefinitions;
    }
}
