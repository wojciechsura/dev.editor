using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BinAnalyzer.AnalyzerDefinition.Statements
{
    abstract class BaseFieldStatement : BaseStatement
    {
        protected readonly string name;

        public BaseFieldStatement(string name)
        {
            this.name = name;
        }

        public string Name => name;
    }
}
