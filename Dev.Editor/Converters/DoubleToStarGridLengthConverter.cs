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
    public class DoubleToStarGridLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double dValue)
            {
                return new GridLength(dValue, GridUnitType.Star);
            }
            else
                return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is GridLength gridLength)
            {
                if (gridLength.IsStar)
                    return gridLength.Value;
                else
                    return Binding.DoNothing;
            }
            else
                return Binding.DoNothing;
        }
    }
}
