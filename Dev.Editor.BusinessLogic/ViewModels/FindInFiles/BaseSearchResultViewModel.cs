using Dev.Editor.BusinessLogic.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.ViewModels.FindInFiles
{
    public class BaseSearchResultViewModel : BaseViewModel
    {
        private bool isSelected;
        private bool isExpanded;

        public BaseSearchResultViewModel()
        {
            isSelected = false;
            isExpanded = false;
        }

        public bool IsSelected 
        {
            get => isSelected;
            set => Set(ref isSelected, () => IsSelected, value);
        }

        public bool IsExpanded
        {
            get => isExpanded;
            set => Set(ref isExpanded, () => IsExpanded, value);
        }
    }
}
