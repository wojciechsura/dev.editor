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
    class SignedEnumShowValue : BaseShowValue
    {
        private readonly SignedEnumDefinition enumDefinition;
        private readonly string memberName;

        public SignedEnumShowValue(SignedEnumDefinition enumDefinition, string member)
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
                case "sbyte":
                    return new SbyteEnumData(alias, enumDefinition.Name, (sbyte)member.Value, member.Name);
                case "short":
                    return new ShortEnumData(alias, enumDefinition.Name, (short)member.Value, member.Name);
                case "int":
                    return new IntEnumData(alias, enumDefinition.Name, (int)member.Value, member.Name);
                case "long":
                    return new LongEnumData(alias, enumDefinition.Name, (long)member.Value, member.Name);
                default:
                    throw new InvalidEnumArgumentException("Unsupported enum type!");
            }
        }
    }
}
