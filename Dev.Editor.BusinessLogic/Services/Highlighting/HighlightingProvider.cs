using Dev.Editor.BusinessLogic.Models.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;

namespace Dev.Editor.BusinessLogic.Services.Highlighting
{
    class HighlightingProvider : IHighlightingProvider, IHighlightingDefinitionReferenceResolver
    {
        // Private fields -----------------------------------------------------

        private const string ResourcePrefix = "Dev.Editor.BusinessLogic.Resources.Highlighting.";

        private readonly HighlightingInfo emptyHighlighting;
        private readonly List<HighlightingInfo> highlightingInfos = new List<HighlightingInfo>();
        private readonly Dictionary<string, HighlightingInfo> highlightingsByExt = new Dictionary<string, HighlightingInfo>(StringComparer.OrdinalIgnoreCase);

        // Private methods ----------------------------------------------------

        private Stream OpenResourceStream(string resource)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourcePrefix + resource);
        }

        private IHighlightingDefinition LoadDefinition(string resourceName)
        {
            XshdSyntaxDefinition xshd;
            using (Stream s = OpenResourceStream(resourceName))
            using (XmlTextReader reader = new XmlTextReader(s))
            {
                xshd = HighlightingLoader.LoadXshd(reader);
            }

            return HighlightingLoader.Load(xshd, this);
        }

        private void RegisterHighlighting(string name, string[] extensions, string resourceName, string iconResourceName)
        {
            ImageSource icon = null;
            if (iconResourceName != null)
            {
                using (var iconStream = OpenResourceStream(iconResourceName))
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.StreamSource = iconStream;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();
                    image.Freeze();

                    icon = image;
                }
            }

            var info = new HighlightingInfo(name, LoadDefinition(resourceName), icon);
            highlightingInfos.Add(info);

            if (extensions != null)
                foreach (var ext in extensions)
                    highlightingsByExt.Add(ext, info);
        }

        // IHighlightingDefinitionReferenceResolver implementation ------------

        IHighlightingDefinition IHighlightingDefinitionReferenceResolver.GetDefinition(string name)
        {
            return highlightingInfos.First(hi => string.Equals(hi.Definition?.Name, name))
                .Definition;
        }

        // Public methods -----------------------------------------------------

        public HighlightingProvider()
        {
            emptyHighlighting = new HighlightingInfo("#(none)", null, null);
            highlightingInfos.Add(emptyHighlighting);

            RegisterHighlighting("XmlDoc", 
                null, 
                "XmlDoc.xshd",
                null);

            RegisterHighlighting("C#", 
                new[] { ".cs" },
                "CSharp-Mode.xshd",
                "cs.png");

            RegisterHighlighting("JavaScript", 
                new[] { ".js" },
                "JavaScript-Mode.xshd",
                "js.png");

            RegisterHighlighting("HTML", 
                new[] { ".htm", ".html" },
                "HTML-Mode.xshd",
                "html.png");

            RegisterHighlighting("ASP", 
                new[] { ".asp", ".aspx", ".asax", ".asmx", ".ascx", ".master" },
                "ASPX.xshd",
                null);

            RegisterHighlighting("Boo", 
                new[] { ".boo" },
                "Boo.xshd",
                null);

            RegisterHighlighting("Coco", 
                new[] { ".atg" },
                "Coco-Mode.xshd",
                null);

            RegisterHighlighting("CSS", 
                new[] { ".css" },
                "CSS-Mode.xshd",
                "css.png");

            RegisterHighlighting("C++", 
                new[] { ".c", ".h", ".cc", ".cpp", ".hpp" },
                "CPP-Mode.xshd",
                "cpp.png");

            RegisterHighlighting("Java", 
                new[] { ".java" },
                "Java-Mode.xshd",
                null);

            RegisterHighlighting("Patch", 
                new[] { ".patch", ".diff" },
                "Patch-Mode.xshd",
                null);

            RegisterHighlighting("PowerShell", 
                new[] { ".ps1", ".psm1", ".psd1" },
                "PowerShell.xshd",
                null);

            RegisterHighlighting("PHP", 
                new[] { ".php" },
                "PHP-Mode.xshd",
                "php.png");

            RegisterHighlighting("Python", 
                new[] { ".py", ".pyw" },
                "Python-Mode.xshd",
                "python.png");

            RegisterHighlighting("TeX", 
                new[] { ".tex" },
                "Tex-Mode.xshd",
                null);

            RegisterHighlighting("TSQL", 
                new[] { ".sql" },
                "TSQL-Mode.xshd",
                null);

            RegisterHighlighting("VB", 
                new[] { ".vb" },
                "VB-Mode.xshd",
                null);

            RegisterHighlighting("XML", 
                new[] {".xml", ".xsl", ".xslt", ".xsd", ".manifest", ".config", ".addin", ".xshd", ".wxs", ".wxi", ".wxl", ".proj", ".csproj", ".vbproj", ".ilproj", ".booproj", ".build", ".xfrm", ".targets", ".xaml", ".xpt", ".xft", ".map", ".wsdl", ".disco", ".ps1xml", ".nuspec" },
                "XML-Mode.xshd",
                null);

            RegisterHighlighting("MarkDown", 
                new[] { ".md" },
                "MarkDown-Mode.xshd",
                null);

            highlightingInfos.Sort((i1, i2) => i1.Name.CompareTo(i2.Name));
        }

        public HighlightingInfo GetDefinitionByExtension(string extension)
        {
            if (highlightingsByExt.ContainsKey(extension))
                return highlightingsByExt[extension];

            return emptyHighlighting;
        }

        public HighlightingInfo GetDefinitionByName(string name)
        {
            return highlightingInfos
                .FirstOrDefault(hi => String.Equals(hi.Name, name))
                ?? emptyHighlighting;
        }

        // Public properties --------------------------------------------------

        public IReadOnlyList<HighlightingInfo> HighlightingDefinitions => highlightingInfos;

        public HighlightingInfo EmptyHighlighting => emptyHighlighting;
    }
}
