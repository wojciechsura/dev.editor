using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Dev.Editor.Converters
{
    public class FilterToBackgroundConverter : IValueConverter
    {
        private readonly Brush filterBrush = new SolidColorBrush(Color.FromArgb(255, 240, 255, 240));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return SystemColors.WindowBrush;
            else if (value is string s)
            {
                if (!string.IsNullOrEmpty(s))
                    return filterBrush;
                else
                    return SystemColors.WindowBrush;
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
