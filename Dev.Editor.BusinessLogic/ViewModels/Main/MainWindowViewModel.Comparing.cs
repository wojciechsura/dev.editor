using Dev.Editor.BusinessLogic.Models.Dialogs;
using Dev.Editor.BusinessLogic.ViewModels.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dev.Editor.BusinessLogic.Models.Documents.Text;
using Dev.Editor.BusinessLogic.Types.Document.Text;
using Dev.Editor.BusinessLogic.Types.Main;

namespace Dev.Editor.BusinessLogic.ViewModels.Main
{
    public partial class MainWindowViewModel
    {
        private void DoCompare()
        {
            // Collecting all text documents
            var textDocuments = documentsManager.AllDocuments
                .OfType<TextDocumentViewModel>()
                .ToList();

            // Cast will set them to null if they are not text documents
            var preferredFirst = documentsManager.SelectedPrimaryDocument as TextDocumentViewModel;
            var preferredSecond = documentsManager.SelectedSecondaryDocument as TextDocumentViewModel;

            if (preferredFirst == null && textDocuments.Count == 2)
            {
                if (textDocuments[0] == preferredSecond)
                    preferredFirst = textDocuments[1];
                else
                    preferredFirst = textDocuments[0];
            }

            if (preferredSecond == null && textDocuments.Count == 2)
            {
                if (textDocuments[0] == preferredFirst)
                    preferredSecond = textDocuments[1];
                else
                    preferredSecond = textDocuments[0];
            }

            var model = new DiffConfigDialogModel(textDocuments, preferredFirst, preferredSecond);
            (bool result, DiffConfigDialogResult config) = dialogService.ShowDiffConfigDialog(model);

            if (result)
            {
                ClearAllDiffs();

                var diffResult = textComparisonService.FindChanges(config.FirstDocument.Document, config.SecondDocument.Document, config.IgnoreCase, config.IgnoreWhitespace);

                if (diffResult.ChangesA.All(c => c == false) && diffResult.ChangesB.All(c => c == false))
                {
                    messagingService.Inform(Resources.Strings.Message_DocumentsAreSame);
                }
                else
                {
                    // Organizing view

                    // Turn on split view
                    documentsManager.ShowSecondaryDocumentTab = true;

                    // Show first document on primary pane
                    if (!documentsManager.PrimaryDocuments.Contains(config.FirstDocument))
                        documentsManager.MoveDocumentTo(config.FirstDocument, DocumentTabKind.Primary);
                    documentsManager.SelectedPrimaryDocument = config.FirstDocument;

                    // Show second document on secondary pane
                    if (!documentsManager.SecondaryDocuments.Contains(config.SecondDocument))
                        documentsManager.MoveDocumentTo(config.SecondDocument, DocumentTabKind.Secondary);
                    documentsManager.SelectedSecondaryDocument = config.SecondDocument;

                    // Show diffs in documents
                    config.FirstDocument.DiffResult = new DiffInfo(diffResult.ChangesA, DiffDisplayMode.Delete);
                    config.SecondDocument.DiffResult = new DiffInfo(diffResult.ChangesB, DiffDisplayMode.Insert);
                }
            }
        }
    }
}
