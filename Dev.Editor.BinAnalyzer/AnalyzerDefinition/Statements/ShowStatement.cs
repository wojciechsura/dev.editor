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
using Dev.Editor.BinAnalyzer.AnalyzerDefinition.Show;

namespace Dev.Editor.BinAnalyzer.AnalyzerDefinition.Statements
{
    class ShowStatement : BaseStatement
    {
        private readonly BaseShowValue showValue;
        private readonly string alias;

        internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
        {
            try
            {
                var data = showValue.Eval(alias, scope); 
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

        public ShowStatement(int line, int column, BaseShowValue showValue, string alias)
            : base(line, column)
        {
            this.showValue = showValue;
            this.alias = alias;
        }

        public BaseShowValue ShowValue => showValue;
        public string Alias => alias;        
    }
}
