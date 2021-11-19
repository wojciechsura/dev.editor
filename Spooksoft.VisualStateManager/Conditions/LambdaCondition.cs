using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Spooksoft.VisualStateManager.Conditions
{
    public class LambdaCondition<TSource> : BaseLambdaCondition
        where TSource : class, INotifyPropertyChanged
    {
        // Private types ------------------------------------------------------

        private class Visitor : BaseMemberAccessVisitor
        {
            private readonly ExpressionType[] availableExpressions = new[] { ExpressionType.Add, ExpressionType.AddChecked, ExpressionType.And,
                ExpressionType.AndAlso, ExpressionType.ArrayLength, ExpressionType.ArrayIndex, ExpressionType.Constant, ExpressionType.Convert,
                ExpressionType.Divide, ExpressionType.Equal, ExpressionType.ExclusiveOr, ExpressionType.GreaterThan, ExpressionType.GreaterThanOrEqual, 
                ExpressionType.Lambda, ExpressionType.LeftShift, ExpressionType.LessThan, ExpressionType.LessThanOrEqual, ExpressionType.MemberAccess, 
                ExpressionType.Modulo, ExpressionType.Multiply, ExpressionType.MultiplyChecked, ExpressionType.Negate, ExpressionType.UnaryPlus, 
                ExpressionType.NegateChecked, ExpressionType.Not, ExpressionType.NotEqual, ExpressionType.Or, ExpressionType.OrElse, 
                ExpressionType.Parameter, ExpressionType.Power, ExpressionType.Quote, ExpressionType.RightShift, ExpressionType.Subtract, 
                ExpressionType.SubtractChecked, ExpressionType.TypeIs, ExpressionType.Default, ExpressionType.Unbox, ExpressionType.TypeEqual, 
                ExpressionType.OnesComplement, ExpressionType.IsTrue, ExpressionType.IsFalse 
            };

            private readonly List<Expression> expressions = new List<Expression>();

            public override Expression Visit(Expression node)
            {
                if (!availableExpressions.Contains(node.NodeType))
                    throw new ArgumentException($"Expression contains unsupported operation: {node.NodeType}");

                return base.Visit(node);
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                expressions.Add(node);
                return base.VisitParameter(node);
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                expressions.Add(node);
                return base.VisitMember(node);
            }

            public override IReadOnlyList<Expression> Expressions => expressions;
        }

        // Private fields -----------------------------------------------------

        private readonly List<MemberAccessChainNode> chainNodes;
        private readonly NotificationRegistry notificationRegistry;
        private readonly Func<TSource, bool> func;
        private readonly TSource source;

        private bool defaultValue;
        private bool cachedValue;

        // Private methods ----------------------------------------------------

        private void HandleIntermediateValueChanged(object sender, EventArgs e)
        {
            UpdateValue();
        }

        private void UpdateValue()
        {
            bool newValue;

            // Check if any of intermediate sources is not null
            if (chainNodes.All(n => n.SourceInChainIsValid()))
                newValue = func(source);
            else
                newValue = defaultValue;

            if (cachedValue != newValue)
            {
                cachedValue = newValue;
                OnValueChanged(cachedValue);
            }
        }

        // Public methods -----------------------------------------------------

        public LambdaCondition(TSource source, Expression<Func<TSource, bool>> lambda, bool defaultValue)
        {
            this.defaultValue = defaultValue;
            this.cachedValue = defaultValue;
            this.source = source;

            notificationRegistry = new NotificationRegistry();
            notificationRegistry.ValueChanged += HandleIntermediateValueChanged;

            chainNodes = ProcessLambda(lambda, notificationRegistry, new Visitor());

            func = lambda.Compile();

            foreach (var chainNode in chainNodes)
                chainNode.NotifySourceChanged(source);

            UpdateValue();
        }

        public override bool GetValue()
        {
            return cachedValue;
        }
    }
}
