using Dev.Editor.BinAnalyzer.Data;
using Dev.Editor.BinAnalyzer.AnalyzerDefinition.Definitions;
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
    class StructFieldStatement : BaseFieldStatement
    {
        private readonly StructDefinition structDef;

        internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
        {
            try
            {
                List<BaseData> members = new List<BaseData>();

                Scope structScope = new Scope(scope);
                for (int i = 0; i < structDef.Statements.Count; i++)
                    structDef.Statements[i].Read(reader, members, structScope);

                var data = new StructData(name, structDef.Name, members);
                result.Add(data);
                scope.AddContent(name, data);
            }
            catch (BaseLocalizedException e)
            {
                throw new AnalysisException(Line, Column, "Failed to read struct array!", string.Format(Strings.Message_AnalysisError_FailedToReadStruct, name, e.LocalizedErrorMessage));
            }
            catch (Exception e)
            {
                throw new AnalysisException(Line, Column, "Failed to read struct array!", string.Format(Strings.Message_AnalysisError_FailedToReadStruct, name, e.Message));
            }
        }

        public StructFieldStatement(int line, int column, string name, StructDefinition structDef) : base(line, column, name)
        {
            this.structDef = structDef;
        }

        public StructDefinition StructDef => structDef;
    }
}
