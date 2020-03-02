using Dev.Editor.BusinessLogic.Models.Messages;
using Dev.Editor.BusinessLogic.ViewModels.Document;
using Dev.Editor.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

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

        public void DoTransformXslt()
        {
            var result = dialogService.ShowOpenDialog(Strings.Filter_Xslt);

            if (result.Result)
            {
                var document = (TextDocumentViewModel)documentsManager.ActiveDocument;
                var text = document.Document.GetText(0, document.Document.TextLength);

                try
                {
                    // Load text
                    var reader = XmlReader.Create(new StringReader(text));

                    // Load transform
                    XslCompiledTransform myXslTrans = new XslCompiledTransform();

                    myXslTrans.Load(result.FileName);

                    // Perform transformation
                    MemoryStream ms = new MemoryStream();
                    XmlTextWriter writer = new XmlTextWriter(ms, Encoding.UTF8);
                    myXslTrans.Transform(reader, null, writer);

                    // Recover result to string
                    ms.Seek(0, SeekOrigin.Begin);
                    var textReader = new StreamReader(ms);
                    string transformed = textReader.ReadToEnd();

                    // Create new document
                    DoNewTextDocument(documentsManager.ActiveDocumentTab, transformed);
                }
                catch (XsltException e)
                {
                    messagesBottomToolViewModel.AddMessage(new MessageModel(e.Message,
                        Types.Messages.MessageSeverity.Error,
                        $"{result.FileName}",
                        e.LineNumber,
                        e.LinePosition));

                    messagingService.ShowError(string.Format(Strings.Message_CannotTransformXslt));
                    BottomPanelVisibility = Types.UI.BottomPanelVisibility.Visible;
                }
                catch (Exception e)
                {
                    messagesBottomToolViewModel.AddMessage(new MessageModel(e.Message,
                        Types.Messages.MessageSeverity.Error,
                        $"{document.FileName}",
                        0,
                        0));

                    messagingService.ShowError(string.Format(Strings.Message_CannotTransformXslt));
                    BottomPanelVisibility = Types.UI.BottomPanelVisibility.Visible;
                }
            }
        }
    }
}
