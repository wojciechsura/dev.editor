using Dev.Editor.BusinessLogic.Services.ImageResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Dev.Editor.Services.ImageResources
{
    public class ImageResources : IImageResources
    {
        private string Prefix = "Dev.Editor.Resources.Images.";

        public ImageSource GetImageFromResource(string resourceName)
        {
            using (var imageStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(Prefix + resourceName))
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = imageStream;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                image.Freeze();

                return image;
            }
        }
    }
}
