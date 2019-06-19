using ICSharpCode.AvalonEdit.Highlighting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Dev.Editor.BusinessLogic.Models.Highlighting
{
    public class HighlightingInfo
    {
        public HighlightingInfo(string name, IHighlightingDefinition definition, string[] extensions, ImageSource icon)
        {
            Name = name;
            Definition = definition;
            Extensions = extensions;
            Icon = icon;
        }

        public string Name { get; }

        public IHighlightingDefinition Definition { get; }

        public string[] Extensions { get; }

        public ImageSource Icon { get; }
    }
}
