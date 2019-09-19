using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dev.Editor.BinAnalyzer.Data;
using Dev.Editor.BinAnalyzer.Exceptions;
using Dev.Editor.Resources;

namespace Dev.Editor.BinAnalyzer.AnalyzerDefinition.Statements
{
    class CharFieldStatement : BaseFieldStatement
    {
        public CharFieldStatement(int line, int column, string name) : base(line, column, name)
        {

        }

        internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
        {
            try
            {
                if (reader.BaseStream.Position + sizeof(byte) >= reader.BaseStream.Length)
                    throw new AnalysisException(Line, Column, "Unexpected end of stream!", Strings.Message_AnalysisError_UnexpectedEndOfStream);

                byte value = reader.ReadByte();

                BaseData data;
                if (value < 128)
                    data = new CharData(name, (char)value);
                else
                    data = new CharData(name, '?');

                result.Add(data);
               
                scope.AddContent(name, data);
            }
            catch
            {
                throw new InvalidOperationException("Cannot load data!");
            }
        }
    }
}
