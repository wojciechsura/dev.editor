using Dev.Editor.BusinessLogic.Types.Search;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Dev.Editor.Converters
{
    public class SearchOperationToIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SearchReplaceOperation operation)
                return (int)operation;

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int index)
                return (SearchReplaceOperation)index;

            return Binding.DoNothing;
        }
    }
}
