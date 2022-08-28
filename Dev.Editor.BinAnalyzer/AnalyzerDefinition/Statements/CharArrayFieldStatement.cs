using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dev.Editor.BinAnalyzer.AnalyzerDefinition.Expressions;
using Dev.Editor.BinAnalyzer.Data;
using Dev.Editor.BinAnalyzer.Exceptions;
using Dev.Editor.Resources;

namespace Dev.Editor.BinAnalyzer.AnalyzerDefinition.Statements
{
    class CharArrayFieldStatement : BaseFieldStatement
    {
        private readonly Expression count;

        public CharArrayFieldStatement(int line, int column, string name, Expression count) 
            : base(line, column, name)
        {
            this.count = count;
        }

        internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
        {
            try
            {
                long arrayOffset = reader.BaseStream.Position;

                dynamic countValue = count.Eval(scope);
                int countInt = (int)countValue;

                if (reader.BaseStream.Position + countInt >= reader.BaseStream.Length)
                    throw new AnalysisException(Line, Column, "Unexpected end of stream", Strings.Message_AnalysisError_UnexpectedEndOfStream);

                List<char> chars = new List<char>();                
                for (int i = 0; i < countInt; i++)
                {
                    byte value = reader.ReadByte();

                    if (value < 128)
                        chars.Add((char)value);
                    else
                        chars.Add('?');                            
                }

                var item = new CharArrayData(name, arrayOffset, chars.ToArray());
                
                result.Add(item);

                scope.AddContent(name, item);
            }
            catch (BaseLocalizedException e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.LocalizedErrorMessage));
            }
            catch (Exception e)
            {
                throw new AnalysisException(Line, Column, "Failed to load field!", string.Format(Strings.Message_AnalysisError_FailedToReadField, name, e.Message));
            }
        }
    }
}
