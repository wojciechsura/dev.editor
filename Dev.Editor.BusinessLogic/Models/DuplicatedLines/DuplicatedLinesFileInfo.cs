using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.DuplicatedLines
{
    public class DuplicatedLinesFileInfo
    {
        public DuplicatedLinesFileInfo(string path, int startLine, int endLine)
        {
            Path = path;
            StartLine = startLine;
            EndLine = endLine;
        }

        public string Path { get; }
        public int StartLine { get; }
        public int EndLine { get; }
    }
}
