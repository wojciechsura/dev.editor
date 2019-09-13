using Dev.Editor.BinAnalyzer.ProgramItems.Expressions;

namespace Dev.Editor.BinAnalyzer.ProgramItems.Statements
{
    internal class SkipArrayFieldStatement : BaseFieldStatement
    {
        private readonly Expression count;

        public SkipArrayFieldStatement(string name, Expression count) : base(name)
        {
            this.count = count;
        }
    }
}