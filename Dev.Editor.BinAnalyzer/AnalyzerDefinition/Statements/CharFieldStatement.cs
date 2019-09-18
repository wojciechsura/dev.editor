using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dev.Editor.BinAnalyzer.Data;

namespace Dev.Editor.BinAnalyzer.AnalyzerDefinition.Statements
{
    class CharFieldStatement : BaseFieldStatement
    {
        public CharFieldStatement(string name) : base(name)
        {

        }

        internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
        {
            try
            {
                byte value = reader.ReadByte();

                BaseData data;
                if (value < 128)
                    data = new CharData(name, (char)value);
                else
                    data = new CharData(name, '?');

                result.Add(data);
                scope.Contents.Add(name, data);
            }
            catch
            {
                throw new InvalidOperationException("Cannot load data!");
            }
        }
    }
}
