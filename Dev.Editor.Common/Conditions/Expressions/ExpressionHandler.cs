using Dev.Editor.Common.Conditions.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Dev.Editor.Common.Conditions.Expressions
{
    internal class ExpressionHandler<TInput, TOutput> : BaseExpressionHandler, IChainedExpressionHandler<TInput>
        where TInput : class, INotifyPropertyChanged
        where TOutput : class, INotifyPropertyChanged
    {
        private readonly Expression<Func<TInput, TOutput>> expression;
        private readonly IChainedExpressionHandler<TOutput> nextExpressionHandler;
        private readonly Func<TInput, TOutput> getOutputFunc;
        private readonly List<string> watchedProperties;
        private TInput source;

        private void Update()
        {
            if (source != null)
            {
                TOutput output = getOutputFunc(source);
                nextExpressionHandler?.NotifySourceChanged(output);
            }
            else
            {
                nextExpressionHandler?.NotifySourceChanged(null);
            }
        }

        private void HandleSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Update();
        }

        public ExpressionHandler(Expression<Func<TInput, TOutput>> expression, IChainedExpressionHandler<TOutput> nextExpressionHandler)
        {
            this.expression = expression;
            this.nextExpressionHandler = nextExpressionHandler;
            this.getOutputFunc = expression.Compile();
            this.watchedProperties = MemberAccessExtractor.Instance.ExtractMembers(expression);
            source = null;
        }

        public void NotifySourceChanged(TInput newSource)
        {
            // Nothing to do in this case
            if (source == newSource)
                return;

            if (source != null)
                source.PropertyChanged -= HandleSourcePropertyChanged;

            source = newSource;

            if (source != null)
                source.PropertyChanged += HandleSourcePropertyChanged;

            Update();
        }
    }
}
