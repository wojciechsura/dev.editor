using Dev.Editor.BusinessLogic.ViewModels.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.Dialogs
{
    public class DiffConfigDialogModel
    {
        public DiffConfigDialogModel(List<TextDocumentViewModel> documents, TextDocumentViewModel preferredFirst, TextDocumentViewModel preferredSecond)
        {
            Documents = documents;
            PreferredFirst = preferredFirst;
            PreferredSecond = preferredSecond;
        }

        public List<TextDocumentViewModel> Documents { get; }
        public TextDocumentViewModel PreferredFirst { get; }
        public TextDocumentViewModel PreferredSecond { get; }
    }
}
