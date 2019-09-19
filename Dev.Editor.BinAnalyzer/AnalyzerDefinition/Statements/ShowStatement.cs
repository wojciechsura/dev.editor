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
    class ShowStatement : BaseStatement
    {
        private readonly Expression expression;
        private readonly string alias;

        internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
        {
            try
            {
                dynamic value = expression.Eval(scope);
                var data = DataFactory.DataFromDynamic(alias, value);

                result.Add(data);
            }
            catch (BaseLocalizedException e)
            {
                throw new AnalysisException(Line, Column, "Cannot perform show operation!", string.Format(Strings.Message_AnalysisError_CannotShow, alias, e.LocalizedErrorMessage));
            }
            catch (Exception e)
            {
                throw new AnalysisException(Line, Column, "Cannot perform show operation!", string.Format(Strings.Message_AnalysisError_CannotShow, alias, e.Message));
            }
        }

        public ShowStatement(int line, int column, Expression expression, string alias)
            : base(line, column)
        {
            this.expression = expression;
            this.alias = alias;
        }

        public Expression Expression => expression;
        public string Alias => alias;        
    }
}
