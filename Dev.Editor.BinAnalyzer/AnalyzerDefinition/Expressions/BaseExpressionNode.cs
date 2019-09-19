using Dev.Editor.BinAnalyzer.AnalyzerDefinition;
using Dev.Editor.BinAnalyzer.AnalyzerDefinition.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BinAnalyzer.AnalyzerDefinition.Expressions
{
    abstract class BaseExpressionNode
    {
        protected BaseExpressionNode(int line, int column)
        {
            Line = line;
            Column = column;
        }

        public abstract dynamic Eval(Scope scope);

        public int Line { get; }
        public int Column { get; }
    }
}
