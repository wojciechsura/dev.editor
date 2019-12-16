using Dev.Editor.BusinessLogic.ViewModels.Document;
using ICSharpCode.AvalonEdit.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.ViewModels.Main
{
    public partial class MainWindowViewModel
    {
        private (int startOffset, int endOffset) ExpandSelectionToFullLines(TextDocumentViewModel document)
        {
            (int selStart, int selLength) = document.GetSelection();

            if (selLength == 0)
            {
                selStart = 0;
                selLength = document.Document.TextLength;
            }

            DocumentLine startLine = document.Document.GetLineByOffset(selStart);
            var startOffset = startLine.Offset;
            DocumentLine endLine = document.Document.GetLineByOffset(Math.Max(0, selStart + selLength - 1));
            var endOffset = endLine.Offset + endLine.Length - 1;

            return (startOffset, endOffset);
        }

        private void TransformLines(Func<string, (bool, string)> func)
        {
            var document = (TextDocumentViewModel)activeDocument;

            (int startOffset, int endOffset) = ExpandSelectionToFullLines(document);

            var textToTransform = document.Document.GetText(startOffset, endOffset - startOffset + 1);

            (bool result, string resultText) = func(textToTransform);

            if (result)
            {
                document.RunAsSingleHistoryEntry(() =>
                {
                    document.Document.Replace(startOffset, endOffset - startOffset + 1, resultText);
                });
            }
        }
    }
}
