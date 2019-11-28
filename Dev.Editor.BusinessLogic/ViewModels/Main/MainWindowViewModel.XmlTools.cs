using Dev.Editor.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Dev.Editor.BusinessLogic.ViewModels.Main
{
    public partial class MainWindowViewModel
    {
        public void DoFormatXml()
        {
            TransformLines(text =>
            {
                try
                {
                    var memoryStream = new MemoryStream();
                    XmlTextWriter writer = new XmlTextWriter(memoryStream, Encoding.Unicode);
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(text);

                    writer.Formatting = Formatting.Indented;

                    doc.WriteContentTo(writer);
                    writer.Flush();
                    memoryStream.Flush();

                    memoryStream.Seek(0, SeekOrigin.Begin);

                    var reader = new StreamReader(memoryStream);

                    string formatted = reader.ReadToEnd();

                    return (true, formatted);
                }
                catch
                {
                    messagingService.ShowError(Strings.Message_CannotFormatInvalidXml);

                    return (false, null);
                }
            });
        }
    }
}
