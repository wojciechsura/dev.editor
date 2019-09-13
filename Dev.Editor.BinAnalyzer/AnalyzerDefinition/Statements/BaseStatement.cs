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
        internal abstract void Read(BinaryReader reader, List<BaseData> result, Scope scope);        
    }
}
