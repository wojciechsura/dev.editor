using Dev.Editor.BinAnalyzer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BinAnalyzer.ProgramItems.Statements
{
    class Scope
    {
        public Scope(Scope parentScope = null)
        {
            ParentScope = parentScope;
            Contents = new Dictionary<string, BaseData>();
        }

        public Dictionary<string, BaseData> Contents { get; }
        public Scope ParentScope { get; }
    }
}
