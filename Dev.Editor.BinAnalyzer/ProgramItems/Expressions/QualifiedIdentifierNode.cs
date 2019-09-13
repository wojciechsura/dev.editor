using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dev.Editor.BinAnalyzer.Data;
using Dev.Editor.BinAnalyzer.ProgramItems.Statements;

namespace Dev.Editor.BinAnalyzer.ProgramItems.Expressions
{
    class QualifiedIdentifierNode : BaseExpressionNode
    {
        private readonly List<string> identifier;

        public QualifiedIdentifierNode(List<string> identifier)
        {
            if (identifier == null || identifier.Count == 0)
                throw new ArgumentException(nameof(identifier));

            this.identifier = identifier;
        }

        public override dynamic Eval(Scope scope)
        {
            while (scope != null && !scope.Contents.ContainsKey(identifier[0]))
                scope = scope.ParentScope;

            if (scope == null)
                throw new InvalidOperationException("Cannot find identifier in scope!");

            var data = scope.Contents[identifier[0]];
            int i = 1;
            while (i < identifier.Count)
            {
                var structData = data as StructData;

                if (structData == null)
                    throw new InvalidOperationException("Invalid qualified identifier");

                var child = structData.Children.FirstOrDefault(x => x.Name.Equals(identifier[i]));
                data = child ?? throw new InvalidOperationException("Invalid qualified identifier!");
            }

            var valueData = data as BaseValueData;

            if (valueData == null)
                throw new InvalidOperationException("Data is not valued-data!");

            return valueData.GetValue();
        }

        public IReadOnlyList<string> Identifier => identifier;
    }
}
