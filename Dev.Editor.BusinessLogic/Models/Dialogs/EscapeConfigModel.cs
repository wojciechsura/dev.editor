using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.Dialogs
{
    public class EscapeConfigModel
    {
        public EscapeConfigModel(bool forward)
        {
            Forward = forward;
        }

        public bool Forward { get; }
    }
}
