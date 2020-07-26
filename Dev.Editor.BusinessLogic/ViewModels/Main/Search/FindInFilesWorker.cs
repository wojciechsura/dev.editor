using Dev.Editor.BusinessLogic.Models.Search;
using Dev.Editor.BusinessLogic.ViewModels.Search;
using Dev.Editor.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Injection;

namespace Dev.Editor.BusinessLogic.ViewModels.Main.Search
{
    public class FindInFilesWorker : BackgroundWorker
    {
        private readonly SearchReplaceModel model;

        private List<string> FindFilesToSearch()
        {
            var result = new List<string>();

            string progressText = Strings.FindInFiles_ProcessingDirectory;

            Queue<string> paths = new Queue<string>();
            paths.Enqueue(model.Location);

            while (paths.Any())
            {
                if (CancellationPending)
                    return result;

                var currentPath = paths.Dequeue();

                ReportProgress(0, String.Format(progressText, currentPath));
                
                // Files
                result.AddRange(Directory.EnumerateFiles(currentPath, model.FileMask));

                Directory.EnumerateDirectories(currentPath)
                    .ToList()
                    .ForEach(newPath => paths.Enqueue(newPath));
            }

            return result;
        }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            // Collect files

            List<string> filesToSearch = FindFilesToSearch();

            if (CancellationPending)
                return;

            string progressText = Strings.FindInFiles_SearchingInFile;

            List<FileSearchResultViewModel> result = new List<FileSearchResultViewModel>();

            for (int i = 0; i < filesToSearch.Count; i++)
            {
                if (CancellationPending)
                    return;

                ReportProgress((100 * i / filesToSearch.Count), string.Format(progressText, Path.GetFileName(filesToSearch[i])));

                // Perform search

                
            }
        }

        public FindInFilesWorker(SearchReplaceModel model)
        {
            this.WorkerSupportsCancellation = true;
            this.WorkerReportsProgress = true;
            this.model = model;            
        }
    }
}
