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
        private void SortSelectedLines(bool ascending)
        {
            TransformLines(textToSort =>
            {
                var lines = textToSort.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None).ToList();

                if (ascending)
                    lines.Sort((s1, s2) => s1.CompareTo(s2));
                else
                    lines.Sort((s1, s2) => -s1.CompareTo(s2));

                return (true, string.Join("\r\n", lines));
            });            
        }

        private void RemoveLines(bool withTrim)
        {
            TransformLines(textToRemove =>
            {
                var lines = textToRemove.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None).ToList();

                if (withTrim)
                    return (true, string.Join("\r\n", lines.Where(l => !String.IsNullOrWhiteSpace(l))));
                else
                    return (true, string.Join("\r\n", lines.Where(l => !String.IsNullOrEmpty(l))));
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
