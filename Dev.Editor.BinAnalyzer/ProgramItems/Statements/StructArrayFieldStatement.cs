using Dev.Editor.BinAnalyzer.Data;
using Dev.Editor.BinAnalyzer.ProgramItems.Definitions;
using Dev.Editor.BinAnalyzer.ProgramItems.Expressions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BinAnalyzer.ProgramItems.Statements
{
    class StructArrayFieldStatement : BaseFieldStatement
    {
        private readonly StructDefinition structDef;
        private readonly Expression count;

        internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
        {
            try
            {
                dynamic countValue = count.Eval(scope);
                int countInt = (int)countValue;

                StructData[] data = new StructData[countInt];
                for (int i = 0; i < countInt; i++)
                {
                    List<BaseData> members = new List<BaseData>();

                    Scope structScope = new Scope(scope);
                    for (int j = 0; j < structDef.Statements.Count; j++)
                        structDef.Statements[j].Read(reader, members, structScope);

                    var element = new StructData(name, structDef.Name, members);
                    data[i] = element;
                }

                ArrayData<StructData> item = new ArrayData<StructData>(name, structDef.Name, data);

                result.Add(item);
                scope.Contents.Add(name, item);
            }
            catch
            {
                throw new InvalidOperationException("Cannot read struct array!");
            }
        }

        public StructArrayFieldStatement(string name, StructDefinition structDef, Expression count) 
            : base(name)
        {
            this.structDef = structDef;
            this.count = count;
        }

        public StructDefinition StructDef => structDef;
        public Expression Length => count;
    }
}
