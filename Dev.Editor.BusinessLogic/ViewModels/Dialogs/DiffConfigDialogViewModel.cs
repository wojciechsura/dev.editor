using Dev.Editor.BusinessLogic.Models.Dialogs;
using Dev.Editor.BusinessLogic.ViewModels.Base;
using Dev.Editor.BusinessLogic.ViewModels.Document;
using Dev.Editor.Common.Commands;
using Dev.Editor.Common.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dev.Editor.BusinessLogic.ViewModels.Dialogs
{
    public class DiffConfigDialogViewModel : BaseViewModel
    {
        private readonly List<TextDocumentViewModel> documents;
        private readonly IDiffConfigDialogAccess access;
        private TextDocumentViewModel firstDocument;
        private TextDocumentViewModel secondDocument;

        private Condition documentsSelectedCondition;
        private Condition documentsAreEqualCondition;
        private BaseCondition canConfirmCondition;

        private void DoOk()
        {
            access.Close(new DiffConfigDialogResult(firstDocument, secondDocument, IgnoreCase, IgnoreWhitespace), true);
        }

        private void DoCancel()
        {
            access.Close(null, false);
        }

        private void HandleDocumentsChanged()
        {
            documentsSelectedCondition.Value = firstDocument != null && secondDocument != null;
            documentsAreEqualCondition.Value = firstDocument == secondDocument;
        }

        public DiffConfigDialogViewModel(IDiffConfigDialogAccess access, DiffConfigDialogModel model)
        {
            this.documents = model.Documents;
            firstDocument = model.PreferredFirst;
            secondDocument = model.PreferredSecond;

            documentsSelectedCondition = new Condition(firstDocument != null && secondDocument != null);
            documentsAreEqualCondition = new Condition(firstDocument == secondDocument);
            canConfirmCondition = documentsSelectedCondition & !documentsAreEqualCondition;

            OkCommand = new AppCommand(obj => DoOk(), canConfirmCondition);
            CancelCommand = new AppCommand(obj => DoCancel());
            this.access = access;
        }

        public TextDocumentViewModel FirstDocument
        {
            get => firstDocument;
            set => Set(ref firstDocument, () => FirstDocument, value, HandleDocumentsChanged);
        }

        public TextDocumentViewModel SecondDocument
        {
            get => secondDocument;
            set => Set(ref secondDocument, () => SecondDocument, value, HandleDocumentsChanged);
        }

        public bool IgnoreWhitespace { get; set; }
        public bool IgnoreCase { get; set; }

        public List<TextDocumentViewModel> Documents => documents;

        public ICommand OkCommand { get; }

        public ICommand CancelCommand { get; }
    }
}
