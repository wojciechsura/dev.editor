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
        private List<(Expression condition, List<BaseStatement> statements)> conditions;
        private List<BaseStatement> elseStatements;

        public IfStatement(int line, int column, List<(Expression condition, List<BaseStatement> statements)> conditions, List<BaseStatement> elseStatements)
            : base(line, column)
        {
            this.conditions = conditions;
            this.elseStatements = elseStatements;
        }

        internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
        {
            for (int i = 0; i < conditions.Count; i++)
            {
                dynamic value = conditions[i].condition.Eval(scope);

                if (value is bool boolValue)
                {
                    if (boolValue)
                    {
                        for (int j = 0; j < conditions[i].statements.Count; j++)
                            conditions[i].statements[j].Read(reader, result, scope);

                        return;
                    }
                }
                else
                {
                    throw new AnalysisException(Line, Column, "If condition does not evaluate to bool!", Strings.Message_AnalysisError_InvalidIfCondition);
                }
            }

            if (elseStatements != null)
            {
                for (int j = 0; j < elseStatements.Count; j++)
                    elseStatements[j].Read(reader, result, scope);
            }
        }
    }
}
