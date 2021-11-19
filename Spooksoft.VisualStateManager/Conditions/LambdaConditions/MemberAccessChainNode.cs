using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace Spooksoft.VisualStateManager.Conditions
{
    internal class MemberAccessChainNode
    {
        private readonly MemberAccessChainNode next;
        private readonly NotificationRegistry notificationRegistry;
        private INotifyPropertyChanged source;
        private MemberExpression expression;

        private void UpdateNext()
        {
            if (next == null)
                return;

            if (source != null)
            {
                object value = (expression.Member as PropertyInfo).GetValue(source);

                if (value != null)
                {
                    if (!(value is INotifyPropertyChanged notifying))
                        throw new InvalidOperationException($"For LambdaCondition to work properly, all intermediate classes in the member access chain must implement INotifyPropertyChanged. {value.GetType().Name} being value of {source.GetType().Name}.{expression.Member.Name} doesn't match this requirement.");

                    next.NotifySourceChanged(notifying);
                }
                else
                {
                    next.NotifySourceChanged(null);
                }
            }
            else
            {
                next.NotifySourceChanged(null);
            }
        }

        internal MemberAccessChainNode(MemberAccessChainNode next, NotificationRegistry notificationRegistry, MemberExpression expression)
        {
            if (expression.Member.MemberType != MemberTypes.Property ||
                !(expression.Member is PropertyInfo))
                throw new ArgumentException("Currenlty only property member accesses are implemented!");

            this.next = next;
            this.notificationRegistry = notificationRegistry;
            this.expression = expression;
        }

        internal void NotifySourceChanged(INotifyPropertyChanged newSource)
        {
            if (source == newSource)
                return;

            // Unregister old source
            if (source != null)
                notificationRegistry.Unregister(this, expression.Member.Name, source);

            source = newSource;

            if (newSource != null)
            {
                notificationRegistry.Register(this, expression.Member.Name, source);
            }

            UpdateNext();
        }

        internal void NotifyPropertyChanged()
        {
            UpdateNext();
        }

        internal bool SourceInChainIsValid()
        {
            return source != null && (next?.SourceInChainIsValid() ?? true);
        }
    }
}
