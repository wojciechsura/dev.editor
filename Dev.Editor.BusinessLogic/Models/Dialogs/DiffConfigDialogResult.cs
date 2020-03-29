using Dev.Editor.BusinessLogic.ViewModels.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.Dialogs
{
    public class DiffConfigDialogResult
    {
        public DiffConfigDialogResult(TextDocumentViewModel firstDocument, TextDocumentViewModel secondDocument, bool ignoreCase, bool ignoreWhitespace)
        {
            FirstDocument = firstDocument;
            SecondDocument = secondDocument;
            IgnoreCase = ignoreCase;
            IgnoreWhitespace = ignoreWhitespace;
        }

        public TextDocumentViewModel FirstDocument { get; }
        public TextDocumentViewModel SecondDocument { get; }
        public bool IgnoreCase { get; }
        public bool IgnoreWhitespace { get; }
    }
}
