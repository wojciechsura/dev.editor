using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dev.Editor.BinAnalyzer.AnalyzerDefinition.Expressions;
using Dev.Editor.BinAnalyzer.Data;
using Dev.Editor.BinAnalyzer.AnalyzerDefinition.Statements;
using Dev.Editor.BinAnalyzer.Exceptions;
using Dev.Editor.Resources;

namespace Dev.Editor.BinAnalyzer.AnalyzerDefinition.Expressions
{
    class QualifiedIdentifierNode : BaseExpressionNode
    {
        private readonly List<string> identifier;

        public QualifiedIdentifierNode(int line, int column, List<string> identifier)
            : base(line, column)
        {
            if (identifier == null || identifier.Count == 0)
                throw new ArgumentException(nameof(identifier));

            this.identifier = identifier;
        }

        public override dynamic Eval(Scope scope)
        {
            BaseData data = null;

            while (scope != null)
            {
                bool result;

                (result, data) = scope.TryGetContent(identifier[0]);

                if (result)
                    break;
                else
                    scope = scope.ParentScope;
            }

            if (scope == null)
                throw new EvalException(Line, Column, "Cannot find identifier!", string.Format(Strings.Message_EvalError_CannotFindIdentifier, String.Join(".", identifier)));
            
            int i = 1;
            while (i < identifier.Count)
            {
                if (!(data is StructData structData))
                    throw new EvalException(Line, Column, "Cannot access member!", string.Format(Strings.Message_EvalError_CannotAccessMember, identifier[i - 1], identifier[i]));

                var child = structData.Children.FirstOrDefault(x => x.Name.Equals(identifier[i]));
                data = child ?? throw new EvalException(Line, Column, "Cannot access member!", string.Format(Strings.Message_EvalError_CannotAccessMember, identifier[i - 1], identifier[i]));
            }

            if (!(data is BaseValueData valueData))
                throw new EvalException(Line, Column, "Accessed field is not value-type!", string.Format(Strings.Message_EvalError_MemberIsNotValue, String.Join(".", identifier)));

            return valueData.GetValue();
        }

        public IReadOnlyList<string> Identifier => identifier;
    }
}
