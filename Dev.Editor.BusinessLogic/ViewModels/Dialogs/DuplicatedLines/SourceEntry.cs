using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.ViewModels.Dialogs.DuplicatedLines
{
    public class SourceEntry
    {
        public SourceEntry(string path)
        {
            Path = path;
        }

        public string Path { get; }
    }
}
