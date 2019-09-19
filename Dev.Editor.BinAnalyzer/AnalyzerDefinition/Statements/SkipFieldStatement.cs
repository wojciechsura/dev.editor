using System;
using System.Collections.Generic;
using System.IO;
using Dev.Editor.BinAnalyzer.Data;
using Dev.Editor.BinAnalyzer.Exceptions;
using Dev.Editor.Resources;

namespace Dev.Editor.BinAnalyzer.AnalyzerDefinition.Statements
{
    internal class SkipFieldStatement : BaseFieldStatement
    {
        public SkipFieldStatement(int line, int column, string name) : base(line, column, name)
        {
        }

        internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
        {
            try
            {
                if (reader.BaseStream.Position + sizeof(byte) >= reader.BaseStream.Length)
                    throw new AnalysisException(Line, Column, "Unexpected end of stream", Strings.Message_AnalysisError_UnexpectedEndOfStream);

                // Skip byte
                reader.BaseStream.Seek(1, SeekOrigin.Current);
            }
            catch (BaseLocalizedException e)
            {
                throw new AnalysisException(Line, Column, "Failed to skip bytes!", string.Format(Strings.Message_AnalysisError_FailedToSkip, e.LocalizedErrorMessage));
            }
            catch (Exception e)
            {
                throw new AnalysisException(Line, Column, "Failed to skip bytes!", string.Format(Strings.Message_AnalysisError_FailedToSkip, e.Message));
            }
        }
    }
}