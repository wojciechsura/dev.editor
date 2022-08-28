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
    class EnumShowValue : BaseShowValue
    {
        private readonly BaseEnumDefinition enumDefinition;
        private readonly string memberName;

        public EnumShowValue(BaseEnumDefinition enumDefinition, string member)
        {
            this.enumDefinition = enumDefinition;
            this.memberName = member;
        }

        public override BaseData Eval(string alias, Scope scope)
        {
            try
            {
                return enumDefinition.GenerateEnumData(alias, -1, memberName);
            }
            catch (ArgumentException)
            {
                throw new BaseLocalizedException(
                    $"Enum {enumDefinition.Name} does not contain member {memberName}!",
                    string.Format(Strings.Message_EvalError_EnumDoesNotContainMember, enumDefinition.Name, memberName));
            }
        }
    }
}
