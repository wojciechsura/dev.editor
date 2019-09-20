using Dev.Editor.BusinessLogic.ViewModels.BottomTools.Base;
using Dev.Editor.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Dev.Editor.BusinessLogic.ViewModels.BottomTools.Messages
{
    public class MessagesBottomToolViewModel : BaseBottomToolViewModel
    {
        public override string Title => Strings.BottomTool_Messages_Title;

        public override ImageSource Icon => null;

        public override string Uid => MessagesUid;
    }
}
