using Dev.Editor.BusinessLogic.Types.Search;
using Dev.Editor.Services.ImageResources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Dev.Editor.Converters
{
    public class SearchReplaceOperationToImageConverter : IValueConverter
    {
        private ImageSource searchIcon;
        private ImageSource replaceIcon;

        public SearchReplaceOperationToImageConverter()
        {
            var imageResources = new ImageResources();
            searchIcon = imageResources.GetIconByName("Search16.png");
            replaceIcon = imageResources.GetIconByName("Replace16.png");
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SearchReplaceOperation operation)
            {
                switch (operation)
                {
                    case SearchReplaceOperation.Search:
                        return searchIcon;
                    case SearchReplaceOperation.Replace:
                        return replaceIcon;
                    default:
                        throw new InvalidEnumArgumentException("Unsupported search/replace operation!");
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
