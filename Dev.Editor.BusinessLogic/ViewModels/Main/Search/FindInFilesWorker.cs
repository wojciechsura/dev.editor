using Dev.Editor.BusinessLogic.Models.Search;
using Dev.Editor.BusinessLogic.ViewModels.Search;
using Dev.Editor.Resources;
using System;
using System.Collections;
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
    public class FindInFilesWorker : BackgroundWorker
    {
        // Private types ------------------------------------------------------

        private abstract class BaseSearchItem
        {
            public BaseSearchItem(string name)
            {
                Path = name;
            }

            public string Path { get; }

            public abstract BaseFilesystemSearchResultViewModel Build();
        }

        private abstract class BaseSearchContainerItem : BaseSearchItem, IEnumerable<BaseSearchItem>
        {
            protected readonly List<BaseSearchItem> items = new List<BaseSearchItem>();

            protected List<BaseFilesystemSearchResultViewModel> GenerateResultList()
            {
                var results = new List<BaseFilesystemSearchResultViewModel>();

                foreach (var item in items)
                {
                    var builtItem = item.Build();
                    if (builtItem != null)
                        results.Add(builtItem);
                }

                return results;
            }

            public BaseSearchContainerItem(string path) 
                : base(path)
            {
            }

            public void Add(BaseSearchItem item) => items.Add(item);

            public IEnumerator<BaseSearchItem> GetEnumerator() => ((IEnumerable<BaseSearchItem>)items).GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<BaseSearchItem>)items).GetEnumerator();
        }

        private class SearchRoot : BaseSearchContainerItem
        {
            public SearchRoot(string path)
                : base(path)
            {

            }

            public override BaseFilesystemSearchResultViewModel Build()
            {
                List<BaseFilesystemSearchResultViewModel> results = GenerateResultList();

                return new RootSearchResultViewModel(results);
            }
        }

        private class SearchedFolder : BaseSearchContainerItem
        {
            public SearchedFolder(string path)
                : base(path)
            {
            }

            public override BaseFilesystemSearchResultViewModel Build()
            {
                List<BaseFilesystemSearchResultViewModel> results = GenerateResultList();

                if (results.Any())
                    return new FolderSearchResultViewModel(Path, results);
                else
                    return null;
            }
        }

        private class SearchedFile : BaseSearchItem, IEnumerable<SearchResult>
        {
            private readonly List<SearchResult> results;

            public SearchedFile(string path)
                : base(path)
            {
            }

            public void Add(SearchResult result) => results.Add(result);

            public override BaseFilesystemSearchResultViewModel Build()
            {
                if (results.Any())
                {
                    var resultList = new List<SearchResultViewModel>();
                    foreach (var result in results)
                    {
                        var resultViewModel = new SearchResultViewModel("<TODO>", result.Row, result.Col);
                        resultList.Add(resultViewModel);
                    }

                    return new FileSearchResultViewModel(Path, resultList);
                }
                else
                    return null;
            }

            public IEnumerator<SearchResult> GetEnumerator() => ((IEnumerable<SearchResult>)results).GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<SearchResult>)results).GetEnumerator();
        }

        private class SearchResult
        {
            public SearchResult(int row, int col)
            {
                Row = row;
                Col = col;
            }

            public int Row { get; }
            public int Col { get; }
        }

        // Private fields -----------------------------------------------------

        private readonly SearchReplaceModel model;

        // Private methods ----------------------------------------------------

        private void FindFilesRecursive(BaseSearchContainerItem dir, ref int fileCount)
        {
            ReportProgress(0, String.Format(Strings.FindInFiles_ProcessingDirectory, dir.Path, fileCount));

            // Folders

            foreach (var folder in Directory.EnumerateDirectories(dir.Path))
            {
                if (CancellationPending)
                    return;

                var subFolder = new SearchedFolder(folder);

                FindFilesRecursive(subFolder, ref fileCount);
                if (subFolder.Any())
                    dir.Add(subFolder);
            }

            // Files

            foreach (var file in Directory.EnumerateFiles(dir.Path, model.FileMask))
            {
                if (CancellationPending)
                    return;

                var dirFile = new SearchedFile(file);
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
                else if (item is SearchedFile file)
                {
                    current++;
                    ReportProgress(100 * current / total, String.Format(Strings.FindInFiles_SearchingInFile, file.Path));

                    FindInFile(file);
                }
            }
        }

        private void FindInFile(SearchedFile file)
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

                        var result = new SearchResult(row, col);
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

            SearchRoot root = new SearchRoot(model.Location);

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

            BaseFilesystemSearchResultViewModel result = root.Build();

            e.Result = result;
        }

        // Public methods -----------------------------------------------------

        public FindInFilesWorker(SearchReplaceModel model)
        {
            this.WorkerSupportsCancellation = true;
            this.WorkerReportsProgress = true;
            this.model = model;            
        }
    }
}
