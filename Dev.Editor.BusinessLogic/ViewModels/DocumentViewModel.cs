using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;

namespace Dev.Editor.BusinessLogic.ViewModels
{
    public class DocumentViewModel
    {
        private TextDocument document;

        public DocumentViewModel()
        {
            document = new TextDocument();
        }

        public TextDocument Document => document;
    }
}
