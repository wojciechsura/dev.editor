using Dev.Editor.BusinessLogic.Models.Search;
using Dev.Editor.BusinessLogic.ViewModels.Search;
using Dev.Editor.Common.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.ViewModels.Main
{
    public partial class MainWindowViewModel : ISearchHost
    {
        public void FindNext(SearchModel searchModel)
        {
			// TODO
        }

        public void Replace(ReplaceModel replaceModel)
        {
			// TODO
        }

        public void ReplaceAll(ReplaceModel replaceModel)
        {
			// TODO
        }

        public BaseCondition CanSearchCondition => documentExistsCondition;
    }
}
