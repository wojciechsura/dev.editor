using Spooksoft.VisualStateManager.Conditions.LambdaConditions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Spooksoft.VisualStateManager.Conditions
{

    public abstract class BaseChainedLambdaCondition<TFinalSource> : BaseLambdaCondition, IBaseChainedLambdaStep<TFinalSource>
        where TFinalSource : class, INotifyPropertyChanged
    {
        // Private types ------------------------------------------------------

        private class Visitor : BaseMemberAccessVisitor
        {
            private readonly ExpressionType[] availableExpressions = new[] { ExpressionType.Add, ExpressionType.AddChecked, ExpressionType.And,
                ExpressionType.AndAlso, ExpressionType.ArrayLength, ExpressionType.ArrayIndex, ExpressionType.Call, ExpressionType.Constant, 
                ExpressionType.Convert, ExpressionType.Divide, ExpressionType.Equal, ExpressionType.ExclusiveOr, ExpressionType.GreaterThan, 
                ExpressionType.GreaterThanOrEqual, ExpressionType.Lambda, ExpressionType.LeftShift, ExpressionType.LessThan, ExpressionType.LessThanOrEqual, 
                ExpressionType.MemberAccess, ExpressionType.Modulo, ExpressionType.Multiply, ExpressionType.MultiplyChecked, ExpressionType.Negate, 
                ExpressionType.UnaryPlus, ExpressionType.NegateChecked, ExpressionType.Not, ExpressionType.NotEqual, ExpressionType.Or, 
                ExpressionType.OrElse, ExpressionType.Parameter, ExpressionType.Power, ExpressionType.Quote, ExpressionType.RightShift, 
                ExpressionType.Subtract, ExpressionType.SubtractChecked, ExpressionType.TypeIs, ExpressionType.TypeAs, ExpressionType.Default, 
                ExpressionType.Unbox, ExpressionType.TypeEqual, ExpressionType.OnesComplement, ExpressionType.IsTrue, ExpressionType.IsFalse
            };

            private readonly List<Expression> expressions = new List<Expression>();

            public override Expression Visit(Expression node)
            {
                if (node != null && !availableExpressions.Contains(node.NodeType))
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
                if (node.Expression is MemberExpression)
                    throw new ArgumentException("ChainedLambdaCondition does not allow chained member access operations (like a.B.C). Use LambdaCondition instead.");

                expressions.Add(node);
                return base.VisitMember(node);
            }

            public override IReadOnlyList<Expression> Expressions => expressions;
        }

        // Private fields -----------------------------------------------------

        private readonly bool defaultValue;
        private readonly Expression<Func<TFinalSource, bool>> expression;
        private readonly Func<TFinalSource, bool> func;
        private readonly NotificationRegistry notificationRegistry;
        private readonly List<MemberAccessChainNode> nodes;

        private TFinalSource source;
        private bool cachedValue;

        // Private methods ----------------------------------------------------

        private void Update()
        {
            bool newValue = source != null ? func(source) : defaultValue;
            if (cachedValue != newValue)
            {
                cachedValue = newValue;
                OnValueChanged(newValue);
            }
        }

        private void HandleValueChanged(object sender, EventArgs e)
        {
            Update();
        }

        // IBaseChainedLambdaStep<TFinalSource> implementation ----------------

        void IBaseChainedLambdaStep<TFinalSource>.UpdateSource(TFinalSource newSource, bool force)
        {
            if (source == newSource && !force)
                return;

            source = newSource;

            foreach (var node in nodes)
                node.NotifySourceChanged(newSource);

            Update();
        }

        // Protected types ----------------------------------------------------

        internal class ChainStep<TInput, TOutput> : IBaseChainedLambdaStep<TInput>
            where TInput : class, INotifyPropertyChanged
            where TOutput : class, INotifyPropertyChanged
        {
            private readonly Expression<Func<TInput, TOutput>> expression;
            private readonly Func<TInput, TOutput> func;
            private readonly List<MemberAccessChainNode> nodes;
            private readonly IBaseChainedLambdaStep<TOutput> nextStep;
            private readonly NotificationRegistry notificationRegistry;
            private TInput source;
            
            private void Update(bool force)
            {
                if (source != null)
                {
                    TOutput output = func(source);
                    nextStep.UpdateSource(output, force);
                }
                else
                {
                    nextStep.UpdateSource(null, force);
                }
            }

            private void HandleValueChanged(object sender, EventArgs e)
            {
                Update(false);
            }

            internal ChainStep(Expression<Func<TInput, TOutput>> lambda, 
                IBaseChainedLambdaStep<TOutput> nextStep)
            {
                notificationRegistry = new NotificationRegistry();
                notificationRegistry.ValueChanged += HandleValueChanged;

                nodes = ProcessLambda(lambda, notificationRegistry, new Visitor());
                expression = lambda;
                func = lambda.Compile();
                this.nextStep = nextStep;
            }

            void IBaseChainedLambdaStep<TInput>.UpdateSource(TInput newSource, bool force)
            {
                if (source == newSource && !force)
                    return;

                source = newSource;

                foreach (var node in nodes)
                    node.NotifySourceChanged(source);

                Update(force);
            }
        }

        // Public methods -----------------------------------------------------

        public BaseChainedLambdaCondition(Expression<Func<TFinalSource, bool>> expression,
            bool defaultValue = false)
        {
            this.notificationRegistry = new NotificationRegistry();
            notificationRegistry.ValueChanged += HandleValueChanged;

            this.defaultValue = defaultValue;
            this.nodes = ProcessLambda(expression, notificationRegistry, new Visitor());
            this.expression = expression;
            this.func = expression.Compile();
            this.source = null;
        }

        public override bool GetValue()
        {
            return cachedValue;
        }
    }
}
