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
    public class MutablePropertyFuncCondition<TSource, TProperty> : BaseCondition
            where TSource : class, INotifyPropertyChanged
    {
        private TSource source;
        private readonly string sourcePropertyName;
        private readonly Func<TSource, TProperty> getValueFunc;
        private readonly string valuePropertyName;
        private readonly Func<TProperty, bool> evalValueFunc;
        private bool value;

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

        private void HandleSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(valuePropertyName))
            {
                Update();
            }
        }

        private void Update()
        {
            var newValue = evalValueFunc(getValueFunc(source));

            if (newValue != value)
            {
                OnValueChanged(newValue);
            }
        }

        public MutablePropertyFuncCondition(TSource source,
            Expression<Func<TSource, TProperty>> getValueExpression,
            Func<TProperty, bool> evalValueFunc)
        {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
            source.PropertyChanged += HandleSourcePropertyChanged;

            var propInfo = GetPropertyInfo(getValueExpression);
            valuePropertyName = propInfo.Name;
            getValueFunc = getValueExpression.Compile();

            this.evalValueFunc = evalValueFunc ?? throw new ArgumentNullException(nameof(evalValueFunc));

            Update();
        }

        public override bool GetValue() => value;
    }
}
