using Dev.Editor.BinAnalyzer.Data;
using Dev.Editor.BinAnalyzer.AnalyzerDefinition.Definitions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BinAnalyzer.AnalyzerDefinition.Statements
{
    class StructFieldStatement : BaseFieldStatement
    {
        private readonly StructDefinition structDef;

        internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
        {
            List<BaseData> members = new List<BaseData>();

            Scope structScope = new Scope(scope);
            for (int i = 0; i < structDef.Statements.Count; i++)
                structDef.Statements[i].Read(reader, members, structScope);

            var data = new StructData(name, structDef.Name, members);
            result.Add(data);
            scope.Contents.Add(name, data);
        }

        public StructFieldStatement(string name, StructDefinition structDef) : base(name)
        {
            this.structDef = structDef;
        }

        public StructDefinition StructDef => structDef;
    }
}
