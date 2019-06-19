using Dev.Editor.BusinessLogic.Models.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Dev.Editor.BusinessLogic.Services.Highlighting
{
    class HighlightingProvider : IHighlightingProvider, IHighlightingDefinitionReferenceResolver
    {
        private const string ResourcePrefix = "Dev.Editor.BusinessLogic.Resources.Highlighting";
        private readonly List<HighlightingInfo> highlighters;

        private Stream OpenResourceStream(string name)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourcePrefix + name);
            if (stream == null)
                throw new FileNotFoundException($"Highlighting resource file '{name}' was not found.");
            return stream;
        }

        private IHighlightingDefinition LoadHighlightingDefinition(string name)
        {
            var stream = OpenResourceStream(name);

            XshdSyntaxDefinition xshd;

            using (XmlTextReader reader = new XmlTextReader(stream))
                xshd = HighlightingLoader.LoadXshd(reader);

            return HighlightingLoader.Load(xshd, this);
        }

        IHighlightingDefinition IHighlightingDefinitionReferenceResolver.GetDefinition(string name)
        {
            return highlighters.First(h => h.Name == name)
                .Definition;
        }

        public HighlightingProvider()
        {
            
        }
    }
}
