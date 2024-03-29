﻿using Dev.Editor.BusinessLogic.Models.Dialogs;
using Dev.Editor.BusinessLogic.ViewModels.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dev.Editor.BusinessLogic.Models.Documents.Text;
using Dev.Editor.BusinessLogic.Types.Document.Text;
using Dev.Editor.BusinessLogic.Types.Main;
using Dev.Editor.BusinessLogic.Services.TextComparison;

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

                BaseDocumentDiffInfo firstDiff = null;
                BaseDocumentDiffInfo secondDiff = null;

                if (config.CharByChar)
                {
                    char[] docA = config.FirstDocument.Document.Text.ToCharArray();
                    char[] docB = config.SecondDocument.Document.Text.ToCharArray();

                    var diffResult = textComparisonService.FindChanges(docA, docB);
                    var lineResult = textComparisonService.ChangesToLineChanges(diffResult, docA, docB);

                    if (lineResult.ChangesA.All(x => x == null) && lineResult.ChangesB.All(x => x == null))
                    {
                        messagingService.Inform(Resources.Strings.Message_DocumentsAreSame);
                        return;
                    }

                    firstDiff = new DocumentLineDiffInfo(lineResult.ChangesA, DiffDisplayMode.Delete);
                    secondDiff = new DocumentLineDiffInfo(lineResult.ChangesB, DiffDisplayMode.Insert);
                }
                else
                {
                    var diffResult = textComparisonService.FindChanges(new TextDocumentAsList(config.FirstDocument.Document),
                        new TextDocumentAsList(config.SecondDocument.Document),
                        config.IgnoreCase,
                        config.IgnoreWhitespace);

                    if (diffResult.ChangesA.All(c => c == false) && diffResult.ChangesB.All(c => c == false))
                    {
                        messagingService.Inform(Resources.Strings.Message_DocumentsAreSame);
                        return;
                    }

                    firstDiff = new DocumentDiffInfo(diffResult.ChangesA, DiffDisplayMode.Delete);
                    secondDiff = new DocumentDiffInfo(diffResult.ChangesB, DiffDisplayMode.Insert);
                }

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
                config.FirstDocument.DiffResult = firstDiff;
                config.SecondDocument.DiffResult = secondDiff;
            }
        }

        private bool HasChange(BaseDocumentDiffInfo docDiff, int line)
        {
            switch (docDiff)
            {
                case DocumentDiffInfo diffInfo:
                    return diffInfo.Changes[line];
                case DocumentLineDiffInfo lineDiffInfo:
                    return lineDiffInfo.Changes[line] != null && lineDiffInfo.Changes[line].Any();
                default:
                    throw new InvalidOperationException("Unsupported document diff info!");
            }
        }

        private bool WithinChanges(BaseDocumentDiffInfo docDiff, int line)
        {
            switch (docDiff)
            {
                case DocumentDiffInfo diffInfo:
                    return line >= 0 && line < diffInfo.Changes.Length;
                case DocumentLineDiffInfo lineDiffInfo:
                    return line >= 0 && line < lineDiffInfo.Changes.Count;
                default: throw new InvalidOperationException("Unsupported diff info!");
            }
        }

        private void DoPreviousChange()
        {
            var doc = documentsManager.ActiveDocument as TextDocumentViewModel;

            (var start, _) = doc.GetSelection();

            var line = doc.Document.GetLineByOffset(start).LineNumber - 1;

            var diff = doc.DiffResult;

            do
            {
                line--;
                if (line >= 0 && HasChange(diff, line))
                    break;
            }
            while (line >= 0);

            if (line >= 0)
            {
                var offset = doc.Document.GetOffset(line + 1, 0);
                doc.SetSelection(offset, 0, true);
            }
        }

        private void DoNextChange()
        {
            var doc = documentsManager.ActiveDocument as TextDocumentViewModel;

            (var start, _) = doc.GetSelection();

            var line = doc.Document.GetLineByOffset(start).LineNumber - 1;

            var diff = doc.DiffResult;

            do
            {
                line++;
                if (WithinChanges(diff, line) && HasChange(diff, line))
                    break;
            }
            while (WithinChanges(diff, line));

            if (WithinChanges(diff, line))
            {
                var offset = doc.Document.GetOffset(line + 1, 0);
                doc.SetSelection(offset, 0, true);
            }
        }
    }
}
