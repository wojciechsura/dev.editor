using Dev.Editor.BinAnalyzer.ProgramItems.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BinAnalyzer.ProgramItems.Statements
{
    class AssignmentStatement : BaseStatement
    {
        private readonly string identifier;
        private readonly Expression expression;

        public AssignmentStatement(string identifier, Expression expression)
        {
            this.identifier = identifier;
            this.expression = expression;
        }

        public string Identifier => identifier;
        public Expression Expression => expression;
    }
}
