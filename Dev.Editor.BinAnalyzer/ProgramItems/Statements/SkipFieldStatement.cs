using System;
using System.Collections.Generic;
using System.IO;
using Dev.Editor.BinAnalyzer.Data;

namespace Dev.Editor.BinAnalyzer.ProgramItems.Statements
{
    internal class SkipFieldStatement : BaseFieldStatement
    {
        public SkipFieldStatement(string name) : base(name)
        {
        }

        internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
        {
            try
            {
                // Skip byte
                reader.ReadByte();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Cannot read data!");
            }
        }
    }
}