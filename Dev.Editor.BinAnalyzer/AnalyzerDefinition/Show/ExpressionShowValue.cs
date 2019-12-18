using Dev.Editor.BinAnalyzer.AnalyzerDefinition.Expressions;
using Dev.Editor.BinAnalyzer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BinAnalyzer.AnalyzerDefinition.Show
{
    class ExpressionShowValue : BaseShowValue
    {
        private readonly Expression expression;

        public ExpressionShowValue(int line, int column, Expression expression)
        {
            this.expression = expression;
        }

        public override BaseData Eval(string alias, Scope scope)
        {
            dynamic value = expression.Eval(scope);
            return DataFactory.DataFromDynamic(alias, value);
        }
    }
}
