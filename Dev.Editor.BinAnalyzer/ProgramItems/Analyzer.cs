using Dev.Editor.BinAnalyzer.Data;
using Dev.Editor.BinAnalyzer.ProgramItems.Statements;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BinAnalyzer.ProgramItems
{
    public class Analyzer
    {
        private readonly List<BaseStatement> statements;

        internal Analyzer(List<BaseStatement> statements)
        {
            this.statements = statements;
        }

        public List<BaseData> Analyze(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);

            List<BaseData> result = new List<BaseData>();
            Scope scope = new Scope();
            for (int i = 0; i < statements.Count; i++)
            {
                statements[i].Read(reader, result, scope);
            }

            return result;
        }
    }
}
