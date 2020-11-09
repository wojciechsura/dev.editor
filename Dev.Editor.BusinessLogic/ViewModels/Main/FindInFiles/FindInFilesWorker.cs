using Dev.Editor.BusinessLogic.Models.FindInFiles;
using Dev.Editor.BusinessLogic.Models.Search;
using Dev.Editor.BusinessLogic.Services.FileIcons;
using Dev.Editor.BusinessLogic.Services.ImageResources;
using Dev.Editor.BusinessLogic.Types.Search;
using Dev.Editor.BusinessLogic.ViewModels.Search;
using Dev.Editor.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI.WebControls.WebParts;
using System.Windows.Markup;
using Unity.Injection;

namespace Dev.Editor.BusinessLogic.ViewModels.Main.Search
{
    public partial class FindInFilesWorker : BackgroundWorker
    {
        // Private constants --------------------------------------------------

        private const int CHARS_BEFORE_AFTER = 80;
        private const int MAX_CONTENT_LENGTH = 100;

        // Private fields -----------------------------------------------------

        private readonly SearchReplaceModel model;
        private readonly SearchReplaceOperation operation;

        // Private methods ----------------------------------------------------

        private void FindFilesRecursive(BaseSearchContainerItem dir, ref int fileCount)
        {
            ReportProgress(0, String.Format(Strings.FindInFiles_ProcessingDirectory, dir.Path, fileCount));

            // Folders

            foreach (var folder in Directory.EnumerateDirectories(dir.Path))
            {
                if (CancellationPending)
                    return;

                var subFolder = new FolderSearchItem(folder);

                FindFilesRecursive(subFolder, ref fileCount);
                if (subFolder.Any())
                    dir.Add(subFolder);
            }

            // Files

            foreach (var file in Directory.EnumerateFiles(dir.Path, model.FileMask))
            {
                if (CancellationPending)
                    return;

                var dirFile = new FileSearchItem(file);
                dir.Add(dirFile);

                fileCount++;
                ReportProgress(0, String.Format(Strings.FindInFiles_ProcessingDirectory, dir.Path, fileCount));
            }
        }

        private void FindInFilesRecursive(BaseSearchContainerItem root, ref int current, int total)
        {
            foreach (var item in root)
            {
                if (CancellationPending)
                    return;

                if (item is BaseSearchContainerItem container)
                {
                    FindInFilesRecursive(container, ref current, total);
                }
                else if (item is FileSearchItem file)
                {
                    current++;
                    ReportProgress(100 * current / total, String.Format(Strings.FindInFiles_SearchingInFile, file.Path));

                    FindInFile(file);
                }
            }
        }

        private void FindInFile(FileSearchItem file)
        {
            try
            {
                string contents = File.ReadAllText(file.Path);

                int index = 0;

                int row = 0;
                int col = 0;
                int lineIndex = 0;

                Match match;

                do
                {
                    if (CancellationPending)
                        return;

                    match = model.FindInFilesRegex.Match(contents, index);

                    if (match.Success)
                    {
                        // Eval row and column
                        while (lineIndex < match.Index)
                        {
                            if (contents[lineIndex] == '\n')
                            {
                                row++;
                                col = 0;
                            }
                            else
                                col++;

                            lineIndex++;
                        }

                        var before = contents.Substring(Math.Max(0, match.Index - CHARS_BEFORE_AFTER), Math.Min(match.Index, CHARS_BEFORE_AFTER));
                        int lastLineBreakInBefore = Math.Max(before.LastIndexOf('\n') + 1, before.LastIndexOf("\r\n") + 2);
                        if (lastLineBreakInBefore >= 0)
                            before = before.Substring(lastLineBreakInBefore);
                        else
                            before = "..." + before;

                        string matchContent;
                        if (match.Length < MAX_CONTENT_LENGTH)
                            matchContent = contents.Substring(match.Index, match.Length);
                        else
                            matchContent = contents.Substring(match.Index, MAX_CONTENT_LENGTH / 2) +
                                " ... " +
                                contents.Substring(match.Index + match.Length - MAX_CONTENT_LENGTH / 2, MAX_CONTENT_LENGTH / 2);

                        var after = contents.Substring(match.Index + match.Length, CHARS_BEFORE_AFTER);
                        var offset = match.Index;

                        string replaceWith = null;
                        if (operation == SearchReplaceOperation.ReplaceInFiles)
                        {
                            if (model.IsRegexReplace)
                            {
                                replaceWith = model.FindInFilesRegex.Replace(match.Value, model.Replace);                                
                            }
                            else
                            {
                                replaceWith = model.Replace;
                            }
                        }

                        int firstLineBreakInAfter = Math.Min(after.IndexOf('\n'), after.IndexOf("\r\n"));
                        if (firstLineBreakInAfter >= 0)
                            after = after.Substring(0, firstLineBreakInAfter);
                        else
                            after = after + "...";


                        var result = new ResultSearchItem(file.Path, row, col, offset, match.Length, before, matchContent, replaceWith, after);
                        file.Add(result);

                        index = match.Index + match.Length;
                    }
                }
                while (match.Success);
            }
            catch
            {
                // Ignore any errors (eg. file access problems etc.
                // In future, possibly this might be reported somehow.
            }
        }

        // Protected methods --------------------------------------------------

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            // Collect files

            RootSearchItem root = new RootSearchItem(model.Location, model.Search);

            int fileCount = 0;

            FindFilesRecursive(root, ref fileCount);

            if (CancellationPending)
                return;

            // Now, search in files

            if (fileCount > 0)
            {
                int current = 0;
                FindInFilesRecursive(root, ref current, fileCount);
            }

            // Build results

            e.Result = new FindInFilesWorkerResult(root, operation);
        }

        // Public methods -----------------------------------------------------

        public FindInFilesWorker(SearchReplaceModel model, SearchReplaceOperation operation)
        {
            if (!(new[] { SearchReplaceOperation.FindInFiles, SearchReplaceOperation.ReplaceInFiles }.Contains(operation)))
                throw new ArgumentOutOfRangeException(nameof(operation));

            this.WorkerSupportsCancellation = true;
            this.WorkerReportsProgress = true;
            this.model = model;
            this.operation = operation;
        }
    }
}
