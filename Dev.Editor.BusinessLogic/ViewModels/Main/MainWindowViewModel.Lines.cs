using Dev.Editor.BusinessLogic.Models.DuplicatedLines;
using Dev.Editor.BusinessLogic.ViewModels.Document;
using Dev.Editor.BusinessLogic.ViewModels.DuplicatedLines;
using Dev.Editor.BusinessLogic.ViewModels.Main.DuplicatedLines;
using Dev.Editor.Resources;
using ICSharpCode.AvalonEdit.Document;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dev.Editor.BusinessLogic.Types.UI;
using Dev.Editor.BusinessLogic.Types.BottomTools;
using Dev.Editor.BusinessLogic.Types.DuplicatedLines;

namespace Dev.Editor.BusinessLogic.ViewModels.Main
{
    public partial class MainWindowViewModel
    {
        private void HandleFindDuplicatedLinesCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
                return;

            var result = e.Result as DuplicatedLinesResult;

            List<DuplicatedLinesResultEntry> sortedEntries = null;

            switch (result.Config.ResultSortKind)
            {
                case DuplicatedLinesResultSortKind.FirstByLinesThenByFiles:
                    {
                        sortedEntries = result.Entries
                            .OrderByDescending(entry => entry.Lines.Count)
                            .ThenByDescending(entry => entry.Filenames.Count)
                            .ToList();
                        break;
                    }
                case DuplicatedLinesResultSortKind.FirstByFilesThenByLines:
                    {
                        sortedEntries = result.Entries
                            .OrderByDescending(entry => entry.Filenames.Count)
                            .ThenByDescending(entry => entry.Lines.Count)
                            .ToList();
                        break;
                    }
                default:
                    throw new InvalidEnumArgumentException("Unsupported result sorting kind!");
            }

            List<DuplicatedLineCaseViewModel> cases = new List<DuplicatedLineCaseViewModel>();

            foreach (var entry in sortedEntries)
            {
                List<BaseDuplicatedLineDetailsViewModel> details = new List<BaseDuplicatedLineDetailsViewModel>();
                foreach (var file in entry.Filenames)
                {
                    var fileDetail = new FileReferenceViewModel(file.Path, file.StartLine, file.EndLine);
                    details.Add(fileDetail);
                }

                var preview = new DuplicatedContentPreviewViewModel(String.Join("\n", entry.Lines));
                details.Add(preview);

                var @case = new DuplicatedLineCaseViewModel(entry.Lines.Count, entry.Filenames.Count, details);
                cases.Add(@case);
            }

            var results = new DuplicatedLinesResultViewModel(cases.Count, cases);
           
            SearchResultsBottomToolViewModel.SetResults(results);

            BottomPanelVisibility = BottomPanelVisibility.Visible;
            SelectedBottomTool = BottomTool.SearchResults;
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

                return (true, string.Join("\r\n", lines));
            });            
        }

        private void RemoveEmptyLines(bool withTrim)
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
            RemoveEmptyLines(true);
        }

        private void DoRemoveEmptyLines()
        {
            RemoveEmptyLines(false);
        }

        private void DoRemoveDuplicatedNeighboringLines()
        {
            TransformLines(text =>
            {
                var lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None).ToList();

                int i = 1;
                while (i < lines.Count)
                {
                    if (string.Equals(lines[i], lines[i - 1]))
                        lines.RemoveAt(i);
                    else
                        i++;
                }

                return (true, String.Join("\r\n", lines));
            });
        }

        private void DoRemoveDuplicatedLines()
        {
            TransformLines(text =>
            {
                var lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None).ToList();

                HashSet<string> existingLines = new HashSet<string>();

                int i = 0;
                while (i < lines.Count)
                {
                    if (existingLines.Contains(lines[i]))
                        lines.RemoveAt(i);
                    else
                    {
                        existingLines.Add(lines[i]);
                        i++;
                    }
                }

                return (true, String.Join("\r\n", lines));
            });
        }

        private void DoFindDuplicatedLines()
        {
            string defaultFolder = GetExplorerFolderIfPossible();
            var model = new DuplicatedLinesFinderConfigModel(defaultFolder);

            (bool result, DuplicatedLinesFinderConfig config) = dialogService.ShowDuplicatedLinesFinderConfigDialog(model);

            if (result)
            {
                var worker = new DuplicatedLinesWorker(config, textComparisonService);
                worker.RunWorkerCompleted += HandleFindDuplicatedLinesCompleted;

                dialogService.ShowProgressDialog(Strings.Operation_SearchingInFiles, worker);
            }
        }
    }
}
