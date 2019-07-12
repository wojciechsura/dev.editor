using Dev.Editor.BusinessLogic.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Dev.Editor.BusinessLogic.ViewModels.Tools.Base
{
    public abstract class BaseToolViewModel : BaseViewModel
    {
        public abstract string Title { get; }
        public abstract ImageSource Icon { get; }
    }
}
