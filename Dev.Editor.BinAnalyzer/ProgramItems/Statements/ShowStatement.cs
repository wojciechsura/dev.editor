using Dev.Editor.BinAnalyzer.ProgramItems.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BinAnalyzer.ProgramItems.Statements
{
    class ShowStatement : BaseStatement
    {
        private readonly Expression expression;
        private readonly string alias;

        public ShowStatement(Expression expression, string alias)
        {
            this.expression = expression;
            this.alias = alias;
        }

        public Expression Expression => expression;
        public string Alias => alias;
    }
}
