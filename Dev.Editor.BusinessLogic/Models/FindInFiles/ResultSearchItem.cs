using Dev.Editor.BusinessLogic.ViewModels.Search;

namespace Dev.Editor.BusinessLogic.Models.FindInFiles
{
    public class ResultSearchItem
    {
        public ResultSearchItem(string fullPath, int row, int col, int length, string before, string match, string after)
        {
            FullPath = fullPath;
            Row = row;
            Col = col;
            Length = length;
            Before = before;
            Match = match;
            After = after;
        }

        public int Row { get; }
        public int Col { get; }
        public string Before { get; }
        public string Match { get; }
        public string After { get; }
        public string FullPath { get; }
        public int Length { get; }
    }
}
