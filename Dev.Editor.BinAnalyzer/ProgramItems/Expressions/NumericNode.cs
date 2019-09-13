using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dev.Editor.BinAnalyzer.ProgramItems.Statements;

namespace Dev.Editor.BinAnalyzer.ProgramItems.Expressions
{
    class NumericNode : BaseExpressionNode
    {
        private readonly dynamic value;

        public NumericNode(dynamic value)
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
