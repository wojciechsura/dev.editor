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
    class AssignmentStatement : BaseStatement
    {
        private readonly string identifier;
        private readonly Expression expression;

        internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
        {
            try
            {
                dynamic value = expression.Eval(scope);
                var data = DataFactory.DataFromDynamic(identifier, value);

                scope.Contents.Add(identifier, data);
            }
            catch
            {
                throw new InvalidOperationException("Cannot perform assignment!");
            }
        }

        public AssignmentStatement(string identifier, Expression expression)
        {
            this.identifier = identifier;
            this.expression = expression;
        }

        public string Identifier => identifier;
        public Expression Expression => expression;
    }
}
