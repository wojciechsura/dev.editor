using Dev.Editor.BinAnalyzer.AnalyzerDefinition.Expressions;
using Dev.Editor.BinAnalyzer.AnalyzerDefinition.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BinAnalyzer.AnalyzerDefinition.Expressions
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
            return root.Eval(scope);
        }

        public BaseExpressionNode Root => root;
    }
}
