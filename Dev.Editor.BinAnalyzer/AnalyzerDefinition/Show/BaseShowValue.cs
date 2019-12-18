using Dev.Editor.BinAnalyzer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BinAnalyzer.AnalyzerDefinition.Show
{
    abstract class BaseShowValue
    {
        public abstract BaseData Eval(string alias, Scope scope);
    }
}
