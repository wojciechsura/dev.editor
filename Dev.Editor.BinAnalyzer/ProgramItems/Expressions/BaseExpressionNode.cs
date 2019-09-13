using Dev.Editor.BinAnalyzer.ProgramItems.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BinAnalyzer.ProgramItems.Expressions
{
    abstract class BaseExpressionNode
    {
        public abstract dynamic Eval(Scope scope);
    }
}
