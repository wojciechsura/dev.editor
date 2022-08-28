using Dev.Editor.BinAnalyzer.Data;
using Dev.Editor.BinAnalyzer.AnalyzerDefinition.Expressions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dev.Editor.BinAnalyzer.Exceptions;
using Dev.Editor.Resources;

namespace Dev.Editor.BinAnalyzer.AnalyzerDefinition.Statements
{
    class AssignmentStatement : BaseStatement
    {
        private readonly string identifier;
        private readonly Expression expression;

        internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
        {
            BaseData data;
            try
            {
                dynamic value = expression.Eval(scope);
                data = DataFactory.DataFromDynamic(identifier, -1, value);
            }
            catch (BaseLocalizedException e)
            {
                throw new AnalysisException(Line, Column, "Cannot perform assignment!", string.Format(Strings.Message_AnalysisError_CannotAssign, identifier, e.LocalizedErrorMessage));
            }
            catch (Exception e)
            {
                throw new AnalysisException(Line, Column, "Cannot perform assignment!", string.Format(Strings.Message_AnalysisError_CannotAssign, identifier, e.Message));
            }

            scope.AddContent(identifier, data);
        }

        public AssignmentStatement(int line, int column, string identifier, Expression expression)
            : base(line, column)
        {
            this.identifier = identifier;
            this.expression = expression;
        }

        public string Identifier => identifier;
        public Expression Expression => expression;
    }
}
