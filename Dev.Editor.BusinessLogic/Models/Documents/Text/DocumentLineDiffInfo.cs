using Dev.Editor.BusinessLogic.Models.TextComparison;
using Dev.Editor.BusinessLogic.Types.Document.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.Documents.Text
{
    public class DocumentLineDiffInfo : BaseDocumentDiffInfo
    {
        public DocumentLineDiffInfo(List<List<LineChangeInstance>> changes, DiffDisplayMode mode)
        {
            Changes = changes;
            Mode = mode;
        }

        public List<List<LineChangeInstance>> Changes { get; }
        public DiffDisplayMode Mode { get; }
    }
}
