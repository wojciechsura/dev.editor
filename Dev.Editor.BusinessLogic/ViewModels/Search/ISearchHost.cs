using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dev.Editor.BusinessLogic.Models.Search;
using Spooksoft.VisualStateManager.Conditions;

namespace Dev.Editor.BusinessLogic.ViewModels.Search
{
    public interface ISearchHost
    {
        void ReplaceAll(SearchReplaceModel replaceModel);
        void Replace(SearchReplaceModel replaceModel);
        void FindNext(SearchReplaceModel searchModel);
        void CountOccurrences(SearchReplaceModel searchModel);

        BaseCondition CanSearchCondition { get; }
        BaseCondition SelectionAvailableCondition { get; }

        void SetFindReplaceSegmentToSelection(bool searchBackwards);
        void ClearFindReplaceSegment();
        void FindInFiles(SearchReplaceModel searchReplaceModel);
        void ReplaceInFiles(SearchReplaceModel searchReplaceModel);
    }
}
