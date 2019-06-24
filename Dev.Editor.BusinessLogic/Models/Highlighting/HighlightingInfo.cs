using Dev.Editor.BusinessLogic.Types.Folding;
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
        public HighlightingInfo(string name, IHighlightingDefinition definition, ImageSource icon, FoldingKind foldingKind)
        {
            Name = name;
            Definition = definition;
            Icon = icon;
            FoldingKind = foldingKind;
        }

        public string Name { get; }

        public IHighlightingDefinition Definition { get; }

        public ImageSource Icon { get; }

        public FoldingKind FoldingKind { get; }
    }
}
