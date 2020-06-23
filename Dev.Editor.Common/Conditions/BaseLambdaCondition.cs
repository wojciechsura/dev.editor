using Dev.Editor.Common.Conditions.Expressions;
using Dev.Editor.Common.Conditions.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.Common.Conditions
{
    public abstract class BaseLambdaCondition<TFinalSource> : BaseCondition, IChainedExpressionHandler<TFinalSource>
        where TFinalSource : class, INotifyPropertyChanged
    {       
        private readonly bool defaultValue;
        private readonly Expression<Func<TFinalSource, bool>> expression;
        private readonly Func<TFinalSource, bool> getValueExpression;
        private readonly List<string> watchedProperties;

        private TFinalSource source;
        private bool value;

        private void UpdateValue()
        {
            bool newValue = source != null ? getValueExpression(source) : defaultValue;
            if (value != newValue)
            {
                value = newValue;
                OnValueChanged(newValue);
            }
        }

        public BaseLambdaCondition(Expression<Func<TFinalSource, bool>> expression, bool defaultValue = false)
        {
            this.expression = expression;
            this.getValueExpression = expression.Compile();
            watchedProperties = MemberAccessExtractor.Instance.ExtractMembers(expression);
            this.defaultValue = defaultValue;
            this.source = null;
            this.value = defaultValue;
        }

        public void NotifySourceChanged(TFinalSource newSource)
        {
            if (source != null)
                source.PropertyChanged -= HandleSourcePropertyChanged;

            source = newSource;

            if (source != null)
                source.PropertyChanged += HandleSourcePropertyChanged;

            UpdateValue();
        }

        private void HandleSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (watchedProperties.Contains(e.PropertyName))
                UpdateValue();
        }

        public override bool GetValue()
        {
            return value;
        }
    }
}
