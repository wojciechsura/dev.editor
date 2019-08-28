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
            DocumentLine endLine = document.Document.GetLineByOffset(selStart + selLength - 1);
            var endOffset = endLine.Offset + endLine.Length - 1;

            return (startOffset, endOffset);
        }

        private void TransformLines(Func<string, string> func)
        {
            var document = (TextDocumentViewModel)activeDocument;

            (int startOffset, int endOffset) = ExpandSelectionToFullLines(document);

            var textToTransform = document.Document.GetText(startOffset, endOffset - startOffset + 1);

            var result = func(textToTransform);

            document.RunAsSingleHistoryEntry(() =>
            {
                document.Document.Replace(startOffset, endOffset - startOffset + 1, result);
            });
        }

        private void SortSelectedLines(bool ascending)
        {
            TransformLines(textToSort =>
            {
                var lines = textToSort.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None).ToList();

                if (ascending)
                    lines.Sort((s1, s2) => s1.CompareTo(s2));
                else
                    lines.Sort((s1, s2) => -s1.CompareTo(s2));

                return string.Join("\r\n", lines);
            });            
        }

        private void RemoveLines(bool withTrim)
        {
            TransformLines(textToRemove =>
            {
                var lines = textToRemove.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None).ToList();

                if (withTrim)
                    return string.Join("\r\n", lines.Where(l => !String.IsNullOrWhiteSpace(l)));
                else
                    return string.Join("\r\n", lines.Where(l => !String.IsNullOrEmpty(l)));
            });
        }

        private void DoSortDescending()
        {
            SortSelectedLines(false);
        }

        private void DoSortAscending()
        {
            SortSelectedLines(true);
        }

        private void DoRemoveWhitespaceLines()
        {
            RemoveLines(true);
        }

        private void DoRemoveEmptyLines()
        {
            RemoveLines(false);
        }
    }
}
