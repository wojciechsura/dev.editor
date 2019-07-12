using Dev.Editor.BusinessLogic.Types.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Dev.Editor.Converters
{
    public class SidePanelToColumnConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SidePanelPlacement placement)
            {
                if (placement == SidePanelPlacement.Left)
                    return 0;
                else if (placement == SidePanelPlacement.Right)
                    return 4;
                else
                    return Binding.DoNothing;
            }
            else
                return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
