using Dev.Editor.BusinessLogic.ViewModels.Base;
using Dev.Editor.Common.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace Dev.Editor.BusinessLogic.ViewModels.BottomTools.Base
{
    public abstract class BaseBottomToolViewModel : BaseViewModel
    {
        protected readonly IBottomToolHandler handler;

        public static string MessagesUid = "messages";

        private void DoCloseBottomTools()
        {
            handler.CloseBottomTools();
        }

        public BaseBottomToolViewModel(IBottomToolHandler handler)
        {
            this.handler = handler;

            CloseBottomToolsCommand = new AppCommand(obj => DoCloseBottomTools());
        }

        public abstract string Title { get; }
        public abstract ImageSource Icon { get; }
        public abstract string Uid { get; }

        public ICommand CloseBottomToolsCommand { get; }
    }
}
