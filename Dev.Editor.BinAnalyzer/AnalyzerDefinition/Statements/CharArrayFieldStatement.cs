using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dev.Editor.BinAnalyzer.AnalyzerDefinition.Expressions;
using Dev.Editor.BinAnalyzer.Data;

namespace Dev.Editor.BinAnalyzer.AnalyzerDefinition.Statements
{
    class CharArrayFieldStatement : BaseFieldStatement
    {
        private readonly Expression count;

        public CharArrayFieldStatement(string name, Expression count) 
            : base(name)
        {
            this.count = count;
        }

        internal override void Read(BinaryReader reader, List<BaseData> result, Scope scope)
        {
            try
            {
                dynamic countValue = count.Eval(scope);
                int countInt = (int)countValue;

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < countInt; i++)
                {
                    byte value = reader.ReadByte();

                    if (value < 128)
                        sb.Append((char)value);
                    else
                        sb.Append('?');                            
                }

                var item = new CharArrayData(name, sb.ToString());
                
                result.Add(item);
                scope.Contents.Add(name, item);
            }
            catch
            {
                throw new InvalidOperationException("Cannot load data!");
            }
        }
    }
}
