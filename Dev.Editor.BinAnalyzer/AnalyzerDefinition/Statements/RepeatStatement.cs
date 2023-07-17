using Dev.Editor.BinAnalyzer.AnalyzerDefinition.Definitions;
using Dev.Editor.BinAnalyzer.Data;
using Dev.Editor.BinAnalyzer.Exceptions;
using Dev.Editor.Resources;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Dev.Editor.BinAnalyzer.AnalyzerDefinition.Statements
{
    internal class RepeatStatement : BaseFieldStatement
    {
        private readonly StructDefinition structDef;

        internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
        {
            try
            {
                long arrayOffset = reader.BaseStream.Position;

                List<StructData> data = new List<StructData>();

                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    long structOffset = reader.BaseStream.Position;

                    List<BaseData> members = new List<BaseData>();

                    Scope structScope = new Scope(scope);
                    for (int j = 0; j < structDef.Statements.Count; j++)
                    {
                        structDef.Statements[j].Read(reader, members, structScope);
                    }

                    var element = new StructData(name, structOffset, structDef.Name, members);
                    data.Add(element);
                }

                ArrayData<StructData> item = new ArrayData<StructData>(name, arrayOffset, structDef.Name, data.ToArray());

                result.Add(item);
                scope.AddContent(name, item);
            }
            catch (BaseLocalizedException e)
            {
                throw new AnalysisException(Line, Column, "Failed to read repeated array!", string.Format(Strings.Message_AnalysisError_FailedToReadRepeatedArray, name, e.LocalizedErrorMessage));
            }
            catch (Exception e)
            {
                throw new AnalysisException(Line, Column, "Failed to read repeated array!", string.Format(Strings.Message_AnalysisError_FailedToReadRepeatedArray, name, e.Message));
            }
        }

        public RepeatStatement(int line, int column, string name, StructDefinition structDef)
            : base(line, column, name)
        {
            this.structDef = structDef;
        }

        public StructDefinition StructDef => structDef;
    }
}
