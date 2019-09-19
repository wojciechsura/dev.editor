using Dev.Editor.BinAnalyzer.AnalyzerDefinition.Expressions;
using Dev.Editor.BinAnalyzer.AnalyzerDefinition.Statements;
using Dev.Editor.BinAnalyzer.Exceptions;
using Dev.Editor.Resources;
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

        public Expression(int line, int column, BaseExpressionNode root)
        {
            this.Line = line;
            this.Column = column;
            this.root = root;
        }

        public dynamic Eval(Scope scope)
        {
            try
            {
                return root.Eval(scope);
            }
            catch (BaseLocalizedException e)
            {
                throw new EvalException(Line, Column, "Failed to eval expression", String.Format(Strings.Message_EvalError_FailedToEvalExpression, e.LocalizedErrorMessage));
            }
            catch (Exception e)
            {
                throw new EvalException(Line, Column, "Failed to eval expression", String.Format(Strings.Message_EvalError_FailedToEvalExpression, e.Message));
            }
        }

        public BaseExpressionNode Root => root;

        public int Line { get; }
        public int Column { get; }
    }
}
