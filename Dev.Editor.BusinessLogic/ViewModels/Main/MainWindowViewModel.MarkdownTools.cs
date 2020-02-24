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
        public void DoInsertMarkdownHeader1()
        {
            InsertAtLineStart("# ");
        }

        public void DoInsertMarkdownHeader2()
        {
            InsertAtLineStart("## ");
        }

        public void DoInsertMarkdownHeader3()
        {
            InsertAtLineStart("### ");
        }

        public void DoInsertMarkdownHeader4()
        {
            InsertAtLineStart("#### ");
        }

        public void DoInsertMarkdownHeader5()
        {
            InsertAtLineStart("##### ");
        }

        public void DoInsertMarkdownHeader6()
        {
            InsertAtLineStart("###### ");
        }

    }
}