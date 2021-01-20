using Dev.Editor.BusinessLogic.ViewModels.BottomTools.SearchResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.ViewModels.DuplicatedLines
{
    public class DuplicatedLinesResultViewModel : BaseSearchResultRootViewModel
    {
        private bool isFiltered;

        public DuplicatedLinesResultViewModel(int totalLines, List<DuplicatedLineCaseViewModel> cases)
        {
            isFiltered = false;

            TotalLines = totalLines;
            Cases = cases;
        }

        public override void ApplyFilter(string filter, bool caseSensitive, bool exclude)
        {
            string updatedFilter = caseSensitive ? filter : filter.ToLower();

            foreach (var c in Cases)
            {
                bool filterMatches = false;

                if (caseSensitive)
                {
                    filterMatches = c.Details.OfType<FileReferenceViewModel>().Any(f => f.Path.Contains(updatedFilter)) ||
                        c.Details.OfType<DuplicatedContentPreviewViewModel>().Any(cp => cp.Text.Contains(updatedFilter));
                }
                else
                {
                    filterMatches = c.Details.OfType<FileReferenceViewModel>().Any(f => f.Path.ToLower().Contains(updatedFilter)) ||
                        c.Details.OfType<DuplicatedContentPreviewViewModel>().Any(cp => cp.Text.ToLower().Contains(updatedFilter));
                }

                c.IsVisible = filterMatches ^ exclude;
            }

            IsFiltered = true;
        }

        public override void ClearFilter()
        {
            foreach (var c in Cases)
                c.IsVisible = true;

            IsFiltered = false;
        }

        public int TotalLines { get; }

        public List<DuplicatedLineCaseViewModel> Cases { get; }

        public override bool IsFiltered
        {
            get => isFiltered;
            set => Set(ref isFiltered, () => IsFiltered, value);
        }
    }
}
