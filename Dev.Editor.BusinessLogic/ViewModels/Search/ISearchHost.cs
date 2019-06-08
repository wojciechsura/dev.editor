using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dev.Editor.BusinessLogic.Models.Search;
using Dev.Editor.Common.Conditions;

namespace Dev.Editor.BusinessLogic.ViewModels.Search
{
    public interface ISearchHost
    {
        void ReplaceAll(ReplaceAllModel replaceModel);
        void Replace(ReplaceModel replaceModel);
        void FindNext(SearchModel searchModel);

        BaseCondition CanSearchCondition { get; }
        BaseCondition SelectionAvailableCondition { get; }
    }
}
