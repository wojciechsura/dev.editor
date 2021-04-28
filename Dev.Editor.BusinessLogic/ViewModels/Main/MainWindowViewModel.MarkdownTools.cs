using Dev.Editor.BusinessLogic.Models.Messages;
using Dev.Editor.BusinessLogic.ViewModels.Document;
using Dev.Editor.Resources;
using Markdig;
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
        private void DoMarkdownHtmlPreview()
        {
            var document = (TextDocumentViewModel)documentsManager.ActiveDocument;

            string html = Markdown.ToHtml(document.Document.Text);

            var webBrowserViewModel = dialogService.RequestWebBrowser(this);
            webBrowserViewModel.ShowWindow();
            webBrowserViewModel.DisplayHtml(html);
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