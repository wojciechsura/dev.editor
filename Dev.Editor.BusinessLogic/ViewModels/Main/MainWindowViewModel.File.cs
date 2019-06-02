using Dev.Editor.BusinessLogic.Properties;
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
            return $"{Resources.BlankDocumentName}{i}.txt";
        }

        private bool InternalSaveDocument(DocumentViewModel document, string filename)
        {
            try
            {
                using (var fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    using (var writer = new StreamWriter(fs))
                    {
                        document.Document.WriteTextTo(writer);
                    }
                }

                document.Changed = false;
                return true;
            }
			catch (Exception e)
            {
                messagingService.ShowError(String.Format(Resources.Message_CannotSaveFile, activeDocument.FileName, e.Message));
                return false;
            }
        }

		private bool DoSaveDocument(DocumentViewModel document)
        {
            if (document.FilenameVirtual)
                throw new InvalidOperationException("Cannot save document with virtual filename!");

            return InternalSaveDocument(document, document.FileName);
        }

		private bool DoSaveDocumentAs(DocumentViewModel document)
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
            int i = 1;
            while (documents.Any(d => d.FileName.Equals(GenerateBlankFileName(i))))
                i++;

            var newDocument = new DocumentViewModel();
            newDocument.Document.FileName = GenerateBlankFileName(i);

            documents.Add(newDocument);

            ActiveDocument = newDocument;
        }

        private void DoOpen()
        {
            var dialogResult = dialogService.OpenDialog();
            if (dialogResult.Result)
            {
                try
                {
                    using (FileStream fs = new FileStream(dialogResult.FileName, FileMode.Open, FileAccess.Read))
                    {
                        var newDocument = DocumentViewModel.CreateFromFile(fs, dialogResult.FileName);

                        documents.Add(newDocument);

                        ActiveDocument = newDocument;
                    }
                }
                catch (Exception e)
                {
                    messagingService.ShowError(string.Format(Resources.Message_CannotOpenFile, dialogResult.FileName, e.Message));
                }
            }
        }

        private void DoSave()
        {
            if (activeDocument.FilenameVirtual)
                DoSaveAs();
            else
                DoSaveDocument(activeDocument);
        }

        private void DoSaveAs()
        {
            DoSaveDocumentAs(activeDocument);
        }
    }
}
