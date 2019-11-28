using Dev.Editor.BusinessLogic.Types.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Dev.Editor.Converters
{
    public class AdditionalToolsetToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;

            if (value is AdditionalToolset additionalToolset && parameter is AdditionalToolset requiredAdditionalToolset)
            {
                if (additionalToolset == requiredAdditionalToolset)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
