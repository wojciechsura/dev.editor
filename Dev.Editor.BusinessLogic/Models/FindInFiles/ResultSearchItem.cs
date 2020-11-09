using Dev.Editor.BusinessLogic.ViewModels.Search;

namespace Dev.Editor.BusinessLogic.Models.FindInFiles
{
    public class ResultSearchItem
    {
        public ResultSearchItem(string fullPath, int row, int col, int offset, int length, string before, string match, string replaceWith, string after)
        {
            FullPath = fullPath;
            Row = row;
            Col = col;
            Offset = offset;
            Length = length;
            Before = before;
            Match = match;
            ReplaceWith = replaceWith;
            After = after;
        }

        public int Row { get; }
        public int Col { get; }
        public int Offset { get; }
        public string Before { get; }
        public string Match { get; }
        public string ReplaceWith { get; }
        public string After { get; }
        public string FullPath { get; }
        public int Length { get; }
    }
}
