using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.Common.Conditions
{
    /// <summary>
    /// Condition, which refers to some object's any-type property, converted
    /// to bool by given evalValueFunc.
    /// It is assumed, that object may change and this condition accounts for it.
    /// </summary>
    public class MutableSourcePropertyFuncCondition<TSourceProvider, TSource, TProperty> : BaseCondition
        where TSource : class, INotifyPropertyChanged
        where TSourceProvider : class, INotifyPropertyChanged
    {
        private readonly TSourceProvider sourceProvider;
        private readonly Func<TSourceProvider, TSource> getSourceFunc;
        private readonly string sourcePropertyName;
        private TSource source;
        private readonly Func<TSource, TProperty> getValueFunc;
        private readonly string valuePropertyName;
        private readonly Func<TProperty, bool> evalValueFunc;
        private readonly bool defaultValue;

        private PropertyInfo GetPropertyInfo<TClass, TClassProperty>(Expression<Func<TClass, TClassProperty>> propertyLambda)
        {
            Type type = typeof(TClass);

            var member = propertyLambda.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda.ToString()));

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertyLambda.ToString()));

            if (type != propInfo.ReflectedType &&
                !type.IsSubclassOf(propInfo.ReflectedType))
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a property that is not from type {1}.",
                    propertyLambda.ToString(),
                    type));

            return propInfo;
        }

        private void Detach(TSource source)
        {
            if (source != null)
                source.PropertyChanged -= HandleSourcePropertyChanged;
        }

        private void Attach(TSource source)
        {
            if (source != null)
                source.PropertyChanged += HandleSourcePropertyChanged;
        }

        private void HandleSourceProviderPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(sourcePropertyName))
            {
                Detach(source);

                source = getSourceFunc(sourceProvider);

                Attach(source);

                OnValueChanged(GetValue());
            }
        }

        private void HandleSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(valuePropertyName))
                OnValueChanged(GetValue());
        }

        public MutableSourcePropertyFuncCondition(TSourceProvider sourceProvider,
            Expression<Func<TSourceProvider, TSource>> getSourceExpression,
            Expression<Func<TSource, TProperty>> getValueExpression,
            Func<TProperty, bool> evalValueFunc,
            bool defaultValue)
        {
            this.sourceProvider = sourceProvider ?? throw new ArgumentNullException(nameof(sourceProvider));
            sourceProvider.PropertyChanged += HandleSourceProviderPropertyChanged;

            var sourcePropInfo = GetPropertyInfo(getSourceExpression);
            sourcePropertyName = sourcePropInfo.Name;
            getSourceFunc = getSourceExpression.Compile();

            var propInfo = GetPropertyInfo(getValueExpression);
            valuePropertyName = propInfo.Name;
            getValueFunc = getValueExpression.Compile();

            source = getSourceFunc(sourceProvider);
            Attach(source);

            this.evalValueFunc = evalValueFunc ?? throw new ArgumentNullException(nameof(evalValueFunc));

            this.defaultValue = defaultValue;
        }

        public override bool GetValue() => source != null ? evalValueFunc(getValueFunc(source)) : defaultValue;
    }
}
