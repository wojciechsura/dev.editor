using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Dev.Editor.Controls
{
    public class ComboBoxEx : ComboBox
    {
        private TextBoxBase textBox;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            textBox = GetTemplateChild("PART_EditableTextBox") as TextBoxBase;
        }

        public void SelectAll()
        {
            textBox?.SelectAll();
        }
    }
}
