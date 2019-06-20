﻿using Dev.Editor.Resources;
using Dev.Editor.BusinessLogic.ViewModels.Document;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.ViewModels.Main
{
    public partial class MainWindowViewModel
    {
        // Private methods ----------------------------------------------------------

        private static string GenerateBlankFileName(int i)
        {
            return $"{Strings.BlankDocumentName}{i}.txt";
        }

        private void InternalAddDocument(Action<DocumentViewModel> initAction)
        {
            var document = new DocumentViewModel(this);

            initAction(document);

            documents.Add(document);

            ActiveDocument = document;
        }

        private void InternalWriteDocument(DocumentViewModel document, string filename)
        {
            using (var fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (var writer = new StreamWriter(fs))
                {
                    document.Document.WriteTextTo(writer);
                }
            }
        }
       
        private void InternalReadDocument(DocumentViewModel document, string filename)
        {
            using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(fs))
                {
                    document.Document.Text = reader.ReadToEnd();
                }
            }
        }

        private bool InternalSaveDocument(DocumentViewModel document, string filename)
        {
            try
            {
                InternalWriteDocument(document, filename);

                document.Document.UndoStack.MarkAsOriginalFile();
                return true;
            }
			catch (Exception e)
            {
                messagingService.ShowError(String.Format(Strings.Message_CannotSaveFile, activeDocument.FileName, e.Message));
                return false;
            }
        }        

        private void InternalLoadDocument(DocumentViewModel document, string filename)
        {
            InternalReadDocument(document, filename);

            document.Document.FileName = filename;
            document.Document.UndoStack.ClearAll();
            document.Document.UndoStack.MarkAsOriginalFile();
            document.FilenameVirtual = false;
            document.Highlighting = highlightingProvider.GetDefinitionByExtension(Path.GetExtension(filename));
        }

        private void LoadDocument(string filename)
        {
            foreach (var document in documents)
            {
                if (string.Equals(document.FileName.ToLower(), filename.ToLower()))
                {
                    ActiveDocument = document;
                    return;
                }
            }

            InternalAddDocument(document =>
            {
                InternalLoadDocument(document, filename);
            });
        }

		private bool SaveDocument(DocumentViewModel document)
        {
            if (document.FilenameVirtual)
                throw new InvalidOperationException("Cannot save document with virtual filename!");

            return InternalSaveDocument(document, document.FileName);
        }

		private bool SaveDocumentAs(DocumentViewModel document)
        {
            var fileDialogResult = dialogService.SaveDialog();
            if (fileDialogResult.Result && InternalSaveDocument(activeDocument, fileDialogResult.FileName))
            {
                activeDocument.FileName = fileDialogResult.FileName;
                return true;
            }

            return false;
        }

        private void DoNew()
        {
            InternalAddDocument(newDocument =>
            {
                int i = 1;
                while (documents.Any(d => d.FileName.Equals(GenerateBlankFileName(i))))
                    i++;

                newDocument.Document.FileName = GenerateBlankFileName(i);
                newDocument.Highlighting = highlightingProvider.EmptyHighlighting;
            });            
        }

        private void DoOpen()
        {
            var dialogResult = dialogService.OpenDialog();
            if (dialogResult.Result)
            {
                try
                {
                    LoadDocument(dialogResult.FileName);                    
                }
                catch (Exception e)
                {
                    messagingService.ShowError(string.Format(Strings.Message_CannotOpenFile, dialogResult.FileName, e.Message));
                }
            }
        }

        private void DoSave()
        {
            if (activeDocument.FilenameVirtual)
                DoSaveAs();
            else
                SaveDocument(activeDocument);
        }

        private void DoSaveAs()
        {
            SaveDocumentAs(activeDocument);
        }
    }
}
