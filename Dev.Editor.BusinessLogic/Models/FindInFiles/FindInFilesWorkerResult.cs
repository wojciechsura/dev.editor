using Dev.Editor.BusinessLogic.Types.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.FindInFiles
{
    public class FindInFilesWorkerResult
    {
        public FindInFilesWorkerResult(RootSearchItem root, SearchReplaceOperation operation)
        {
            Root = root;
            Operation = operation;
        }

        public RootSearchItem Root { get; }
        public SearchReplaceOperation Operation { get; }
    }
}
