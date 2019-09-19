using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dev.Editor.BinAnalyzer.Data;

namespace Dev.Editor.BinAnalyzer.AnalyzerDefinition.Statements
{
    abstract class BaseStatement
    {
        internal BaseStatement(int line, int column)
        {
            this.Line = line;
            this.Column = column;
        }

        public int Line { get; }
        public int Column { get; }

        internal abstract void Read(BinaryReader reader, List<BaseData> result, Scope scope);        
    }
}
