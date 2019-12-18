using Dev.Editor.BinAnalyzer.AnalyzerDefinition.Definitions;
using Dev.Editor.BinAnalyzer.Data;
using Dev.Editor.BinAnalyzer.Exceptions;
using Dev.Editor.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BinAnalyzer.AnalyzerDefinition.Show
{
    class UnsignedEnumShowValue : BaseShowValue
    {
        private readonly UnsignedEnumDefinition enumDefinition;
        private readonly string memberName;

        public UnsignedEnumShowValue(UnsignedEnumDefinition enumDefinition, string member)
        {
            this.enumDefinition = enumDefinition;
            this.memberName = member;
        }

        public override BaseData Eval(string alias, Scope scope)
        {
            var member = enumDefinition.Items.FirstOrDefault(i => i.Name.Equals(memberName));

            if (member == null)
                throw new BaseLocalizedException(
                    $"Enum {enumDefinition.Name} does not contain member {memberName}!",
                    string.Format(Strings.Message_EvalError_EnumDoesNotContainMember, enumDefinition.Name, memberName));

            // TODO: cast to type may fail

            switch (enumDefinition.Type)
            {
                case "byte":
                    return new ByteEnumData(alias, enumDefinition.Name, (byte)member.Value, member.Name);
                case "ushort":
                    return new UshortEnumData(alias, enumDefinition.Name, (ushort)member.Value, member.Name);
                case "uint":
                    return new UintEnumData(alias, enumDefinition.Name, (uint)member.Value, member.Name);
                case "ulong":
                    return new UlongEnumData(alias, enumDefinition.Name, (ulong)member.Value, member.Name);
                default:
                    throw new InvalidEnumArgumentException("Unsupported enum type!");
            }
        }
    }
}
