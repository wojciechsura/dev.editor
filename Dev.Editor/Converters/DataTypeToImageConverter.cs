using Dev.Editor.BinAnalyzer.Data;
using Dev.Editor.BusinessLogic.Services.ImageResources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Unity;

namespace Dev.Editor.Converters
{
    public class DataTypeToImageConverter : IValueConverter
    {
        private readonly IImageResources imageResources;

        public DataTypeToImageConverter()
        {
            imageResources = Dependencies.Container.Instance.Resolve<IImageResources>();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DataType dataType)
            {
                switch (dataType)
                {
                    case DataType.Enum:
                        {
                            return imageResources.GetIconByName("Enum16.png");
                        }
                    case DataType.Field:
                        {
                            return imageResources.GetIconByName("Member16.png");
                        }
                    case DataType.Struct:
                        {
                            return imageResources.GetIconByName("Struct16.png");
                        }
                    default:
                        throw new InvalidEnumArgumentException("Unsupported data type!");
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
