using Dev.Editor.BusinessLogic.Models.Messages;
using Dev.Editor.BusinessLogic.ViewModels.Document;
using Dev.Editor.Resources;
using Markdig;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Dev.Editor.BusinessLogic.ViewModels.Main
{
    public partial class MainWindowViewModel
    {
        private void DoMarkdownHtmlPreview()
        {
            var document = (TextDocumentViewModel)documentsManager.ActiveDocument;

            var pipeline = new MarkdownPipelineBuilder()
                .Use(new Markdig.Extensions.Tables.PipeTableExtension());

            string html = Markdown.ToHtml(document.Document.Text, pipeline.Build());
            string pre, post;

            var preStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Dev.Editor.BusinessLogic.Resources.Html.markdown-pre.html");
            using (StreamReader sr = new StreamReader(preStream))
                pre = sr.ReadToEnd();

            var postStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Dev.Editor.BusinessLogic.Resources.Html.markdown-post.html");
            using (StreamReader sr = new StreamReader(postStream))
                post = sr.ReadToEnd();

            var webBrowserViewModel = dialogService.RequestWebBrowser(this);
            webBrowserViewModel.ShowWindow();
            webBrowserViewModel.DisplayHtml(pre + html + post);
        }

        private void DoInsertMarkdownHeader1()
        {
            InsertAtLineStart("# ");
        }

        private void DoInsertMarkdownHeader2()
        {
            InsertAtLineStart("## ");
        }

        private void DoInsertMarkdownHeader3()
        {
            InsertAtLineStart("### ");
        }

        private void DoInsertMarkdownHeader4()
        {
            InsertAtLineStart("#### ");
        }

        private void DoInsertMarkdownHeader5()
        {
            InsertAtLineStart("##### ");
        }

        private void DoInsertMarkdownHeader6()
        {
            InsertAtLineStart("###### ");
        }

        private void DoInsertMarkdownHorizontalRuleCommand()
        {
            InsertAtLineStart("---\r\n");
        }

        private void DoInsertMarkdownPictureCommand()
        {
            SurroundSelection("![", "](http://example.com/image.png)");
        }

        private void DoInsertMarkdownLinkCommand()
        {
            SurroundSelection("[", "](http://example.com)");
        }

        private void DoInsertMarkdownStrongCommand()
        {
            SurroundSelection("**", "**");
        }

        private void DoInsertMarkdownEmphasisCommand()
        {
            SurroundSelection("*", "*");
        }

        private void DoInsertMarkdownInlineCodeCommand()
        {
            SurroundSelection("`", "`");
        }

        private void DoInsertMarkdownUnorderedListCommand()
        {
            PrependLines("* ");
        }

        private void DoInsertMarkdownOrderedListCommand()
        {
            PrependLines("1. ");
        }

        private void DoInsertMarkdownBlockquoteCommand()
        {
            PrependLines(">");
        }

        private void DoInsertMarkdownBlockCodeCommand()
        {
            PrependLines("    ");
        }
    }
}