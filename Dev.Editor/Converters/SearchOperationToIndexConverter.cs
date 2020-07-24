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
            {
                switch (operation)
                {
                    case SearchReplaceOperation.Search:
                        return 0;
                    case SearchReplaceOperation.Replace:
                        return 1;
                    case SearchReplaceOperation.FindInFiles:
                        return 2;
                    default:
                        throw new InvalidEnumArgumentException("Unsupported search operation!");
                }
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int index)
            {
                switch (index)
                {
                    case 0:
                        return SearchReplaceOperation.Search;
                    case 1:
                        return SearchReplaceOperation.Replace;
                    case 2:
                        return SearchReplaceOperation.FindInFiles;
                    default:
                        throw new ArgumentOutOfRangeException("Unsupported index!");
                }
            }

            return Binding.DoNothing;
        }
    }
}
