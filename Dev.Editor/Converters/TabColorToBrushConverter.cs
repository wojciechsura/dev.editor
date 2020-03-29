using Dev.Editor.BusinessLogic.Types.Document;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Dev.Editor.Converters
{
    public class TabColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TabColor tabColor)
            {
                switch (tabColor)
                {
                    case TabColor.Default:
                        return new SolidColorBrush(Colors.Transparent);
                    case TabColor.Red:
                        return Application.Current.FindResource("RedTabBackgroundBrush") as SolidColorBrush;
                    case TabColor.Yellow:
                        return Application.Current.FindResource("YellowTabBackgroundBrush") as SolidColorBrush;
                    case TabColor.Green:
                        return Application.Current.FindResource("GreenTabBackgroundBrush") as SolidColorBrush;
                    case TabColor.Blue:
                        return Application.Current.FindResource("BlueTabBackgroundBrush") as SolidColorBrush;
                    default:
                        throw new InvalidEnumArgumentException("Unsupported tab color!");
                }
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
