using Dev.Editor.BusinessLogic.Types.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.UI
{
    public class BottomPanelVisibilityModel
    {
        public BottomPanelVisibilityModel(string display, BottomPanelVisibility value)
        {
            Display = display;
            Value = value;
        }

        public string Display { get; }
        public BottomPanelVisibility Value { get; }
    }
}
