using Dev.Editor.BinAnalyzer.ProgramItems.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BinAnalyzer.ProgramItems.Statements
{
    class StructFieldStatement : BaseFieldStatement
    {
        private readonly StructDefinition structDef;

        public StructFieldStatement(string name, StructDefinition structDef) : base(name)
        {
            this.structDef = structDef;
        }

        public StructDefinition StructDef => structDef;
    }
}
