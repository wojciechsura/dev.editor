using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.ViewModels.FindInFiles
{
    public abstract class BaseFilesystemSearchResultViewModel : BaseSearchResultViewModel
    {
        public abstract int Count { get; }
    }
}
