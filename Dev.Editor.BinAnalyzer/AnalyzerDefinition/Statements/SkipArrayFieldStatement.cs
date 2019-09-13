using Dev.Editor.BinAnalyzer.Data;
using Dev.Editor.BinAnalyzer.AnalyzerDefinition.Expressions;
using System;
using System.Collections.Generic;
using System.IO;

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

                reader.BaseStream.Seek(countInt, SeekOrigin.Current);
            }
            catch
            {
                throw new InvalidOperationException("Cannot load data!");
            }
        }

        public SkipArrayFieldStatement(string name, Expression count) : base(name)
        {
            this.count = count;
        }
    }
}