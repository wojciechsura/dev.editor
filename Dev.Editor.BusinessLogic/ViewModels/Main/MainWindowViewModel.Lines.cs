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
        private (int startOffset, int endOffset) ExpandSelectionToFullLines()
        {
            (int selStart, int selLength) = activeDocument.GetSelection();

            if (selLength == 0)
            {
                selStart = 0;
                selLength = activeDocument.Document.TextLength;
            }

            DocumentLine startLine = activeDocument.Document.GetLineByOffset(selStart);
            var startOffset = startLine.Offset;
            DocumentLine endLine = activeDocument.Document.GetLineByOffset(selStart + selLength - 1);
            var endOffset = endLine.Offset + endLine.Length - 1;

            return (startOffset, endOffset);
        }

        private void SortSelectedLines(bool ascending)
        {
            (int startOffset, int endOffset) = ExpandSelectionToFullLines();

            var textToSort = activeDocument.Document.GetText(startOffset, endOffset - startOffset + 1);
            var lines = textToSort.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None).ToList();

            if (ascending)
                lines.Sort((s1, s2) => s1.CompareTo(s2));
            else
                lines.Sort((s1, s2) => -s1.CompareTo(s2));

            string sorted = string.Join("\r\n", lines);

            activeDocument.RunAsSingleHistoryEntry(() =>
            {
                activeDocument.Document.Replace(startOffset, endOffset - startOffset + 1, sorted);
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

    }
}
