using Dev.Editor.BinAnalyzer.Data;
using Dev.Editor.BinAnalyzer.AnalyzerDefinition.Definitions;
using Dev.Editor.BinAnalyzer.AnalyzerDefinition.Expressions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dev.Editor.BinAnalyzer.Exceptions;
using Dev.Editor.Resources;

namespace Dev.Editor.BinAnalyzer.AnalyzerDefinition.Statements
{
    class StructArrayFieldStatement : BaseFieldStatement
    {
        private readonly StructDefinition structDef;
        private readonly Expression count;

        internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
        {
            try
            {
                long arrayOffset = reader.BaseStream.Position;

                dynamic countValue = count.Eval(scope);
                int countInt = (int)countValue;

                StructData[] data = new StructData[countInt];
                for (int i = 0; i < countInt; i++)
                {
                    long structOffset = reader.BaseStream.Position;

                    List<BaseData> members = new List<BaseData>();

                    Scope structScope = new Scope(scope);
                    for (int j = 0; j < structDef.Statements.Count; j++)
                    {
                        structDef.Statements[j].Read(reader, members, structScope);
                    }

                    var element = new StructData(name, structOffset, structDef.Name, members);
                    data[i] = element;
                }

                ArrayData<StructData> item = new ArrayData<StructData>(name, arrayOffset, structDef.Name, data);

                result.Add(item);
                scope.AddContent(name, item);
            }
            catch (BaseLocalizedException e)
            {
                throw new AnalysisException(Line, Column, "Failed to read struct array!", string.Format(Strings.Message_AnalysisError_FailedToReadStructArray, name, e.LocalizedErrorMessage));
            }
            catch (Exception e)
            {
                throw new AnalysisException(Line, Column, "Failed to read struct array!", string.Format(Strings.Message_AnalysisError_FailedToReadStructArray, name, e.Message));
            }
        }

        public StructArrayFieldStatement(int line, int column, string name, StructDefinition structDef, Expression count) 
            : base(line, column, name)
        {
            this.structDef = structDef;
            this.count = count;
        }

        public StructDefinition StructDef => structDef;
        public Expression Length => count;
    }
}
