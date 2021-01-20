using Dev.Editor.BusinessLogic.Models.SearchResults;
using Dev.Editor.BusinessLogic.ViewModels.BottomTools.SearchResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Dev.Editor.BusinessLogic.ViewModels.FindInFiles
{
    public class SearchResultsViewModel : BaseSearchResultRootViewModel
    {
        private bool isFiltered;

        public SearchResultsViewModel(string fullPath, string searchPattern, ImageSource icon, List<BaseFilesystemSearchResultViewModel> results)
        {
            isFiltered = false;

            FullPath = fullPath;
            SearchPattern = searchPattern;
            Icon = icon;
            Results = results;

            IsExpanded = true;

            Count = results.Sum(r => r.Count);
        }

        public override void ApplyFilter(SearchResultFilterModel model)
        {
            foreach (var result in Results)
            {
                ApplyFilterRecursive(result, model);
            }

            IsFiltered = true;
        }

        public override void ClearFilter()
        {
            foreach (var result in Results)
            {
                ClearFilterRecursive(result);
            }

            IsFiltered = false;
        }

        private void ClearFilterRecursive(BaseFilesystemSearchResultViewModel result)
        {
            result.IsVisible = true;
            if (result is FolderSearchResultViewModel folderViewModel)
            {
                foreach (var item in folderViewModel.Files)
                    ClearFilterRecursive(item);
            }
        }

        private bool ApplyFilterRecursive(BaseFilesystemSearchResultViewModel result, SearchResultFilterModel model)
        {
            string updatedFilter = model.CaseSensitive ? model.Filter : model.Filter.ToLower();
            bool anyVisible = false;

            if (result is FileSearchResultViewModel fileViewModel)
            {
                string data = model.CaseSensitive ? fileViewModel.Name : fileViewModel.Name.ToLower();

                fileViewModel.IsVisible = data.Contains(updatedFilter) ^ model.FilterExcludes;
                anyVisible = fileViewModel.IsVisible;
            }
            else if (result is FolderSearchResultViewModel folderViewModel)
            {
                foreach (var item in folderViewModel.Files)
                {
                    anyVisible |= ApplyFilterRecursive(item, model);
                }

                folderViewModel.IsVisible = anyVisible;
            }
            else throw new InvalidOperationException("Unsupported search result viewmodel!");

            return anyVisible;
        }

        public string FullPath { get; }
        public string SearchPattern { get; }
        public ImageSource Icon { get; }
        public List<BaseFilesystemSearchResultViewModel> Results { get; }

        public int Count { get; }

        public override bool IsFiltered 
        { 
            get => isFiltered;
            set => Set(ref isFiltered, () => IsFiltered, value);
        }
    }
}
