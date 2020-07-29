﻿using Dev.Editor.BusinessLogic.Types.BottomTools;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Dev.Editor.Converters
{
    public class BottomToolToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is BottomTool bottomTool)
                return (int)bottomTool;

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int iValue)
                return (BottomTool)iValue;

            return Binding.DoNothing;
        }
    }
}
