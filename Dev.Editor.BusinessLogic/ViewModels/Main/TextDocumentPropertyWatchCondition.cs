using Dev.Editor.BusinessLogic.ViewModels.Document;
using Dev.Editor.BusinessLogic.ViewModels.Main.Documents;
using Spooksoft.VisualStateManager.Conditions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.ViewModels.Main
{
    internal class TextDocumentPropertyWatchCondition : BaseCondition
    {
        private readonly DocumentsManager documentsManager;
        private readonly bool defaultValue;
        private readonly string valuePropertyName;
        private readonly Func<TextDocumentViewModel, bool> getValueFunc;
        private TextDocumentViewModel source;

        private PropertyInfo GetPropertyInfo<TClass, TProperty>(Expression<Func<TClass, TProperty>> propertyLambda)
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

        private void Detach(TextDocumentViewModel source)
        {
            if (source != null)
                source.PropertyChanged -= HandleTextDocumentPropertyChanged;
        }

        private void Attach(TextDocumentViewModel source)
        {
            if (source != null)
                source.PropertyChanged += HandleTextDocumentPropertyChanged;
        }

        private void HandleTextDocumentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(valuePropertyName))
                OnValueChanged(GetValue());
        }

        private void HandleDocumentsManagerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DocumentsManager.ActiveDocument))
            {
                Detach(source);

                source = documentsManager.ActiveDocument as TextDocumentViewModel;

                Attach(source);

                OnValueChanged(GetValue());
            }
        }

        public TextDocumentPropertyWatchCondition(DocumentsManager documentsManager, Expression<Func<TextDocumentViewModel, bool>> getValueExpression, bool defaultValue)
        {
            this.defaultValue = defaultValue;

            this.documentsManager = documentsManager ?? throw new ArgumentNullException(nameof(documentsManager));
            documentsManager.PropertyChanged += HandleDocumentsManagerPropertyChanged;

            var propInfo = GetPropertyInfo<TextDocumentViewModel, bool>(getValueExpression);
            valuePropertyName = propInfo.Name;
            getValueFunc = getValueExpression.Compile();

            source = documentsManager.ActiveDocument as TextDocumentViewModel;
            Attach(source);
        }

        public override bool GetValue()
        {
            return source != null ? getValueFunc(source) : defaultValue;
        }
    }
}
