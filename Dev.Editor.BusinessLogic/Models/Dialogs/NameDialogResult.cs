using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.Dialogs
{
    public class NameDialogResult
    {
        public NameDialogResult(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
