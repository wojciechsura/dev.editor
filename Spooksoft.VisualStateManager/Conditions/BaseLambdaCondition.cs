using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Spooksoft.VisualStateManager.Conditions
{
    public abstract class BaseLambdaCondition : BaseCondition
    {
        // Protected methods --------------------------------------------------

        /// <summary>
        /// Processes lambda in to a list of chained member-access nodes
        /// </summary>
        /// <param name="lambda">A lambda expression to be processed</param>
        /// <param name="notificationRegistry">A NotificationRegistry, which will be passed
        /// into chained nodes</param>
        /// <param name="visitor">An ExpressionVisitor, which is expected to extract all ParameterExpressions and
        /// MemberAccessExpressions from the expression (and possibly do some validation as well)</param>
        /// <returns></returns>
        internal static List<MemberAccessChainNode> ProcessLambda<TSource, TOutput>(Expression<Func<TSource, TOutput>> lambda,
            NotificationRegistry notificationRegistry,
            BaseMemberAccessVisitor visitor)
        {
            visitor.Visit(lambda);

            List<List<MemberExpression>> linkedExpressions = new List<List<MemberExpression>>();

            // Chain member access expressions originating at parameter expression

            var allExpressions = new List<Expression>(visitor.Expressions);

            var root = allExpressions.OfType<ParameterExpression>().FirstOrDefault();
            while (root != null)
            {
                allExpressions.Remove(root);

                List<MemberExpression> expressions = new List<MemberExpression>();
                Expression previous = root;
                var next = allExpressions.OfType<MemberExpression>().FirstOrDefault(e => e.Expression == previous);
                while (next != null)
                {
                    allExpressions.Remove(next);

                    expressions.Add(next);
                    previous = next;
                    next = allExpressions.OfType<MemberExpression>().FirstOrDefault(e => e.Expression == previous);
                }

                if (expressions.Count > 0)
                    linkedExpressions.Add(expressions);

                root = allExpressions.OfType<ParameterExpression>().FirstOrDefault();
            }

            var result = new List<MemberAccessChainNode>();

            foreach (var expressions in linkedExpressions)
            {
                MemberAccessChainNode next = null;
                MemberAccessChainNode current = null;

                for (int i = expressions.Count - 1; i >= 0; i--)
                {
                    current = new MemberAccessChainNode(next, notificationRegistry, expressions[i]);
                    next = current;
                }

                if (current != null)
                    result.Add(current);
            }

            return result;
        }
    }
}
