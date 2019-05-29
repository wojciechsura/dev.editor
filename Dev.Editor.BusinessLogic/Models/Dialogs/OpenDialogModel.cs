using Dev.Editor.BusinessLogic.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.Dialogs
{
    public class OpenDialogModel
    {
        public OpenDialogModel()
        {
            Filter = Resources.DefaultFilter;
        }

        public string Title { get; set; } = null;
        public string FileName { get; set; } = null;
        public string Filter { get; set; }
    }
}
