using Dev.Editor.BinAnalyzer.Data;
using Dev.Editor.BinAnalyzer.AnalyzerDefinition.Expressions;
using System;
using System.Collections.Generic;
using System.IO;
using Dev.Editor.BinAnalyzer.Exceptions;
using Dev.Editor.Resources;

namespace Dev.Editor.BinAnalyzer.AnalyzerDefinition.Statements
{
    internal class SkipArrayFieldStatement : BaseFieldStatement
    {
        private readonly Expression count;

        internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
        {
            try
            {
                dynamic countValue = count.Eval(scope);
                int countInt = (int)countValue;

                if (reader.BaseStream.Position + countInt >= reader.BaseStream.Length)
                    throw new AnalysisException(Line, Column, "Unexpected end of stream", Strings.Message_AnalysisError_UnexpectedEndOfStream);

                reader.BaseStream.Seek(countInt, SeekOrigin.Current);
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

        public SkipArrayFieldStatement(int line, int column, string name, Expression count) : base(line, column, name)
        {
            this.count = count;
        }
    }
}