using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dev.Editor.BinAnalyzer.AnalyzerDefinition.Expressions;
using Dev.Editor.BinAnalyzer.Data;
using Dev.Editor.BinAnalyzer.Exceptions;
using Dev.Editor.Resources;

namespace Dev.Editor.BinAnalyzer.AnalyzerDefinition.Statements
{
    class IfStatement : BaseStatement
    {
        private readonly Expression condition;
        private readonly List<BaseStatement> statements;

        public IfStatement(int line, int column, Expression condition, List<BaseStatement> statements)
            : base(line, column)
        {
            this.condition = condition;
            this.statements = statements;
        }

        internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
        {
            dynamic value = condition.Eval(scope);

            if (value is bool boolValue)
            {
                if (boolValue)
                {
                    for (int i = 0; i < statements.Count; i++)
                        statements[i].Read(reader, result, scope);
                }
            }
            else
            {
                throw new AnalysisException(Line, Column, "If condition does not evaluate to bool!", Strings.Message_AnalysisError_InvalidIfCondition);
            }
        }
    }
}
