using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.Common.Conditions.Utils
{
    /// <summary>
    /// Extracts all member accesses, eg. given a => a.A + a.B, it will return list {&quot;A&quot;, &quot;B&quot;}.
    /// </summary>
    /// <remarks>
    /// Nested accesses are forbidden and will yield an exception.
    /// </remarks>
    internal class MemberAccessExtractor : ExpressionVisitor
    {
        private static Lazy<MemberAccessExtractor> instance = new Lazy<MemberAccessExtractor>(() => new MemberAccessExtractor());

        private bool processing = false;
        private string parameterName;
        private readonly HashSet<string> accessedMembers = new HashSet<string>();

        protected MemberAccessExtractor()
        {

        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression is MemberExpression)
                throw new InvalidOperationException("Nested member accesses are not allowed!");

            if (node.Expression is ParameterExpression parameterExpression)
            {
                if (parameterExpression.Name == parameterName)
                    accessedMembers.Add(node.Member.Name);
            }

            return base.VisitMember(node);
        }

        public override Expression Visit(Expression node)
        {
            if (!processing)
                throw new InvalidOperationException("Use ExtractMembers<T1, T2> instead.");

            return base.Visit(node);
        }

        public List<string> ExtractMembers<T1, T2>(Expression<Func<T1, T2>> expression)
        {
            try
            {
                processing = true;

                parameterName = expression.Parameters[0].Name;
                accessedMembers.Clear();

                Visit(expression);
            }
            finally
            {
                processing = false;
            }

            return accessedMembers.ToList();
        }

        public static MemberAccessExtractor Instance => instance.Value;
    }
}
