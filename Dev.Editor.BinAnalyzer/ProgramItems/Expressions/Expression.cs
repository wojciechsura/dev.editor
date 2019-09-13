using Dev.Editor.BinAnalyzer.ProgramItems.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BinAnalyzer.ProgramItems.Expressions
{
    class Expression
    {
        private readonly BaseExpressionNode root;

        public Expression(BaseExpressionNode root)
        {
            this.root = root;
        }

        public dynamic Eval(Scope scope)
        {
            root.Eval(scope);
        }

        public BaseExpressionNode Root => root;
    }
}
