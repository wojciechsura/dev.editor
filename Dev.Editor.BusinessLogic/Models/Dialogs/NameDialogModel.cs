using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.Dialogs
{
    public class NameDialogModel
    {
        public NameDialogModel(string currentName, string windowTitle, string groupboxTitle)
        {
            CurrentName = currentName;
            WindowTitle = windowTitle;
            GroupboxTitle = groupboxTitle;
        }

        public string CurrentName { get; }
        public string WindowTitle { get; }
        public string GroupboxTitle { get; }
    }
}
