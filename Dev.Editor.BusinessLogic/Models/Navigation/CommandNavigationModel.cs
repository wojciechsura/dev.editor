using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace Dev.Editor.BusinessLogic.Models.Navigation
{
    public class CommandNavigationModel : BaseNavigationModel
    {
        public CommandNavigationModel(string title, string group, ImageSource icon, bool enabled, ICommand command)
            : base(title, group, icon, enabled)
        {
            Command = command;
        }

        public ICommand Command { get; }
    }
}
