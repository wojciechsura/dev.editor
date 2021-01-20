using Dev.Editor.BusinessLogic.Models.TextComparison;
using ICSharpCode.AvalonEdit.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Services.TextComparison
{
    public interface ITextComparisonService
    {
        ChangesResult FindChanges(IReadOnlyList<string> documentA, IReadOnlyList<string> documentB, bool ignoreCase = false, bool ignoreWhitespace = false);
        ContinuousLineDiffResult GenerateContinuousLineDiff(IReadOnlyList<string> documentA, IReadOnlyList<string> documentB, bool ignoreCase = false, bool ignoreWhitespace = false);
    }
}
