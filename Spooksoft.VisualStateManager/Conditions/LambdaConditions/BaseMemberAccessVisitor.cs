using System.Collections.Generic;
using System.Linq.Expressions;

namespace Spooksoft.VisualStateManager.Conditions
{
    internal abstract class BaseMemberAccessVisitor : ExpressionVisitor
    {
        public abstract IReadOnlyList<Expression> Expressions { get; }
    }
}
