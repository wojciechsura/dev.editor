using Dev.Editor.BusinessLogic.ViewModels.Base;
using Dev.Editor.Common.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace Dev.Editor.BusinessLogic.ViewModels.Tools.Base
{
    public abstract class BaseToolViewModel : BaseViewModel
    {
        protected readonly IToolHandler handler;

        public static string ExplorerUid = "explorer";
        public static string BinaryDefinitionsUid = "binaryDefinitions";

        private void DoCloseTools()
        {
            handler.CloseTools();
        }

        public BaseToolViewModel(IToolHandler handler)
        {
            this.handler = handler;

            CloseToolsCommand = new AppCommand(obj => DoCloseTools());
        }

        public abstract string Title { get; }
        public abstract ImageSource Icon { get; }
        public abstract string Uid { get; }

        public ICommand CloseToolsCommand { get; }
    }
}
