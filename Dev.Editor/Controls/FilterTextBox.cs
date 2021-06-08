using Spooksoft.VisualStateManager.Commands;
using Spooksoft.VisualStateManager.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Dev.Editor.Controls
{
    public class FilterTextBox : TextBox
    {
        private void DoClear()
        {
            Text = String.Empty;
        }

        public FilterTextBox()
        {
            ClearCommand = new AppCommand(obj => DoClear());
        }

        public ICommand ClearCommand { get; }
    }
}
