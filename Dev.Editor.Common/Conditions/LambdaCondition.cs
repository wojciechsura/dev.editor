using Dev.Editor.Common.Conditions.Expressions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

/**
 * A series of lambda-driven conditions allowing to perform checks on nested properties.
 * Features:
 * 
 * * Automatically reacts to changes on any level of nesting
 * * Fully compatible with Condition system, may be used as a condition for AppCommand
 * 
 * Requirements:
 * 
 * * All objects on each level of nesting _must_ implement INotifyPropertyChanged
 * * On each level only direct member access is allowed. For instance, if you want
 *   to monitor nested property:
 *   
 *   a => a.B.C
 *   
 *   You should break them into two expressions:
 *   
 *   a => a.B, b => b.C
 *   
 *   This rule is applied only to expression parameter though.
 **/
namespace Dev.Editor.Common.Conditions
{
    public class LambdaCondition<TSource> : BaseLambdaCondition<TSource>
        where TSource : class, INotifyPropertyChanged
    {
        public LambdaCondition(TSource source, Expression<Func<TSource, bool>> expression, bool defaultValue = false) 
            : base(expression, defaultValue)
        {
            NotifySourceChanged(source);
        }
    }

    public class LambdaCondition<TSource, T1> : BaseLambdaCondition<T1>
        where TSource : class, INotifyPropertyChanged
        where T1 : class, INotifyPropertyChanged
    {
        private readonly ExpressionHandler<TSource, T1> firstExpressionHandler;

        public LambdaCondition(TSource source, Expression<Func<TSource, T1>> expression1, Expression<Func<T1, bool>> expression2, bool defaultValue = false) 
            : base(expression2, defaultValue)
        {          
            firstExpressionHandler = new ExpressionHandler<TSource, T1>(expression1, this);
            firstExpressionHandler.NotifySourceChanged(source);
        }
    }

    public class LambdaCondition<TSource, T1, T2> : BaseLambdaCondition<T2>
        where TSource : class, INotifyPropertyChanged
        where T1 : class, INotifyPropertyChanged
        where T2 : class, INotifyPropertyChanged
    {
        private readonly ExpressionHandler<TSource, T1> firstExpressionHandler;

        public LambdaCondition(TSource source, Expression<Func<TSource, T1>> expression1, Expression<Func<T1, T2>> expression2, Expression<Func<T2, bool>> expression3, bool defaultValue = false) : base(expression3, defaultValue)
        {
            var secondExpressionHandler = new ExpressionHandler<T1, T2>(expression2, this);
            firstExpressionHandler = new ExpressionHandler<TSource, T1>(expression1, secondExpressionHandler);

            firstExpressionHandler.NotifySourceChanged(source);
        }
    }

    public class LambdaCondition<TSource, T1, T2, T3> : BaseLambdaCondition<T3>
        where TSource : class, INotifyPropertyChanged
        where T1 : class, INotifyPropertyChanged
        where T2 : class, INotifyPropertyChanged
        where T3 : class, INotifyPropertyChanged
    {
        private readonly ExpressionHandler<TSource, T1> firstExpressionHandler;

        public LambdaCondition(TSource source, Expression<Func<TSource, T1>> expression1, Expression<Func<T1, T2>> expression2, Expression<Func<T2, T3>> expression3, Expression<Func<T3, bool>> expression4, bool defaultValue = false) : base(expression4, defaultValue)
        {
            var thirdExpressionHandler = new ExpressionHandler<T2, T3>(expression3, this);
            var secondExpressionHandler = new ExpressionHandler<T1, T2>(expression2, thirdExpressionHandler);
            firstExpressionHandler = new ExpressionHandler<TSource, T1>(expression1, secondExpressionHandler);

            firstExpressionHandler.NotifySourceChanged(source);
        }
    }
}
