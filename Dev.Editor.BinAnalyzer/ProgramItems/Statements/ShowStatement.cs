using Dev.Editor.BinAnalyzer.Data;
using Dev.Editor.BinAnalyzer.ProgramItems.Expressions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BinAnalyzer.ProgramItems.Statements
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
            catch
            {
                throw new InvalidOperationException("Cannot perform assignment!");
            }
        }

        public ShowStatement(Expression expression, string alias)
        {
            this.expression = expression;
            this.alias = alias;
        }

        public Expression Expression => expression;
        public string Alias => alias;        
    }
}
