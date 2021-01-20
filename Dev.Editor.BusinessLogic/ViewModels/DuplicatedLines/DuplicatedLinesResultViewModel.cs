using Dev.Editor.BusinessLogic.Models.SearchResults;
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

        public override void ApplyFilter(SearchResultFilterModel model)
        {
            string updatedFilter = model.CaseSensitive ? model.Filter : model.Filter.ToLower();

            foreach (var c in Cases)
            {
                bool filterMatches = false;

                if (model.CaseSensitive)
                {
                    filterMatches = (model.FilterFilenames && c.Details.OfType<FileReferenceViewModel>().Any(f => f.Path.Contains(updatedFilter))) ||
                        (model.FilterContents && c.Details.OfType<DuplicatedContentPreviewViewModel>().Any(cp => cp.Text.Contains(updatedFilter)));
                }
                else
                {
                    filterMatches = (model.FilterFilenames && c.Details.OfType<FileReferenceViewModel>().Any(f => f.Path.ToLower().Contains(updatedFilter))) ||
                        (model.FilterContents && c.Details.OfType<DuplicatedContentPreviewViewModel>().Any(cp => cp.Text.ToLower().Contains(updatedFilter)));
                }

                c.IsVisible = filterMatches ^ model.FilterExcludes;
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
