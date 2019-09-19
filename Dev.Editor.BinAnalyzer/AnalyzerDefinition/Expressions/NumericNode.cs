using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dev.Editor.BinAnalyzer.AnalyzerDefinition.Expressions;
using Dev.Editor.BinAnalyzer.AnalyzerDefinition.Statements;

namespace Dev.Editor.BinAnalyzer.AnalyzerDefinition.Expressions
{
    class NumericNode : BaseExpressionNode
    {
        private readonly dynamic value;

        public NumericNode(int line, int column, dynamic value)
            : base(line, column)
        {
            this.value = value;
        }

        public override dynamic Eval(Scope scope)
        {
            return value;
        }

        public dynamic Value => value;
    }
}
