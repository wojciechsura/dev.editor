using Dev.Editor.BusinessLogic.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Dev.Editor.BusinessLogic.ViewModels.BottomTools.Base
{
    public abstract class BaseBottomToolViewModel : BaseViewModel
    {
        public static string MessagesUid = "messages";

        public abstract string Title { get; }
        public abstract ImageSource Icon { get; }
        public abstract string Uid { get; }
    }
}
