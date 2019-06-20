using Dev.Editor.BusinessLogic.Models.Highlighting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Services.Highlighting
{
    public interface IHighlightingProvider
    {
        HighlightingInfo GetDefinitionByExtension(string extension);        
        IReadOnlyList<HighlightingInfo> HighlightingDefinitions { get; }
    }
}
