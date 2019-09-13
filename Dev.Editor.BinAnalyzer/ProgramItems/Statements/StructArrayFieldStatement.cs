using Dev.Editor.BinAnalyzer.ProgramItems.Definitions;
using Dev.Editor.BinAnalyzer.ProgramItems.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BinAnalyzer.ProgramItems.Statements
{
    class StructArrayFieldStatement : BaseFieldStatement
    {
        private readonly StructDefinition structDef;
        private readonly Expression length;

        public StructArrayFieldStatement(string name, StructDefinition structDef, Expression length) 
            : base(name)
        {
            this.structDef = structDef;
            this.length = length;
        }

        public StructDefinition StructDef => structDef;
        public Expression Length => length;
    }
}
