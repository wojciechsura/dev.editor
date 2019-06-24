using Dev.Editor.BusinessLogic.Models.Highlighting;
using Dev.Editor.BusinessLogic.Types.Folding;
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

        private void RegisterHighlighting(string name, 
            string[] extensions, 
            string resourceName, 
            string iconResourceName, 
            FoldingKind foldingKind)
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

            var info = new HighlightingInfo(name, LoadDefinition(resourceName), icon, foldingKind);
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
            emptyHighlighting = new HighlightingInfo("#(none)", null, null, FoldingKind.None);
            highlightingInfos.Add(emptyHighlighting);

            RegisterHighlighting("XmlDoc", 
                null, 
                "XmlDoc.xshd",
                null,
                FoldingKind.None);

            RegisterHighlighting("C#", 
                new[] { ".cs" },
                "CSharp-Mode.xshd",
                "cs.png",
                FoldingKind.Braces);

            RegisterHighlighting("JavaScript", 
                new[] { ".js" },
                "JavaScript-Mode.xshd",
                "js.png",
                FoldingKind.Braces);

            RegisterHighlighting("HTML", 
                new[] { ".htm", ".html" },
                "HTML-Mode.xshd",
                "html.png",
                FoldingKind.None);

            RegisterHighlighting("ASP", 
                new[] { ".asp", ".aspx", ".asax", ".asmx", ".ascx", ".master" },
                "ASPX.xshd",
                "asp.png",
                FoldingKind.None);

            RegisterHighlighting("CSS", 
                new[] { ".css" },
                "CSS-Mode.xshd",
                "css.png",
                FoldingKind.Braces);

            RegisterHighlighting("C++", 
                new[] { ".c", ".h", ".cc", ".cpp", ".hpp" },
                "CPP-Mode.xshd",
                "cpp.png",
                FoldingKind.Braces);

            RegisterHighlighting("Java", 
                new[] { ".java" },
                "Java-Mode.xshd",
                "java.png",
                FoldingKind.Braces);

            RegisterHighlighting("Patch", 
                new[] { ".patch", ".diff" },
                "Patch-Mode.xshd",
                null,
                FoldingKind.None);

            RegisterHighlighting("PowerShell", 
                new[] { ".ps1", ".psm1", ".psd1" },
                "PowerShell.xshd",
                "powershell.png",
                FoldingKind.Braces);

            RegisterHighlighting("PHP", 
                new[] { ".php" },
                "PHP-Mode.xshd",
                "php.png",
                FoldingKind.Braces);

            RegisterHighlighting("Python", 
                new[] { ".py", ".pyw" },
                "Python-Mode.xshd",
                "python.png",
                FoldingKind.None);

            RegisterHighlighting("TeX", 
                new[] { ".tex" },
                "Tex-Mode.xshd",
                "tex.png",
                FoldingKind.None);

            RegisterHighlighting("TSQL", 
                new[] { ".sql" },
                "TSQL-Mode.xshd",
                "sql.png",
                FoldingKind.None);

            RegisterHighlighting("VB", 
                new[] { ".vb" },
                "VB-Mode.xshd",
                "vb.png",
                FoldingKind.None);

            RegisterHighlighting("XML", 
                new[] {".xml", ".xsl", ".xslt", ".xsd", ".manifest", ".config", ".addin", ".xshd", ".wxs", ".wxi", ".wxl", ".proj", ".csproj", ".vbproj", ".ilproj", ".booproj", ".build", ".xfrm", ".targets", ".xaml", ".xpt", ".xft", ".map", ".wsdl", ".disco", ".ps1xml", ".nuspec" },
                "XML-Mode.xshd",
                "xml.png",
                FoldingKind.Xml);

            RegisterHighlighting("MarkDown", 
                new[] { ".md" },
                "MarkDown-Mode.xshd",
                null,
                FoldingKind.None);

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
