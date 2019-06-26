using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Dev.Editor.BusinessLogic.Services.ImageResources
{
    public interface IImageResources
    {
        ImageSource GetImageFromResource(string resourceName);
    }
}
