using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.ViewModels.BottomTools.Messages
{
    public interface IMessagesHandler
    {
        void OpenFileAndFocus(string filename);
    }
}
