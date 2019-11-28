using Dev.Editor.BusinessLogic.Types.Folding;
using Dev.Editor.BusinessLogic.Types.UI;
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
        public HighlightingInfo(string name, IHighlightingDefinition definition, ImageSource icon, FoldingKind foldingKind, bool hidden, AdditionalToolset additionalToolset)
        {
            Name = name;
            Definition = definition;
            Icon = icon;
            FoldingKind = foldingKind;
            Hidden = hidden;
            AdditionalToolset = additionalToolset;
        }

        public string Name { get; }

        public IHighlightingDefinition Definition { get; }

        public ImageSource Icon { get; }

        public FoldingKind FoldingKind { get; }

        public bool Hidden { get; }

        public AdditionalToolset AdditionalToolset { get; }

        public string Initial => Name.Substring(0, 1);
    }
}
