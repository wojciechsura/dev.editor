using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Dev.Editor.Wpf
{
    public class ScreenBoundsConverter
    {
        private Matrix _transform;

        public ScreenBoundsConverter(Visual visual)
        {
            _transform = PresentationSource.FromVisual(visual).CompositionTarget.TransformFromDevice;
        }

        public Rect ConvertBounds(Rectangle bounds)
        {
            var result = new Rect(bounds.X, bounds.Y, bounds.Width, bounds.Height);
            result.Transform(_transform);
            return result;
        }
    }
}
