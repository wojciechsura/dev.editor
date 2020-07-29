using Dev.Editor.Resources;
using Dev.Editor.BusinessLogic.ViewModels.Document;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dev.Editor.BinAnalyzer.Data;
using Dev.Editor.BusinessLogic.Models.Configuration.BinDefinitions;
using Dev.Editor.BinAnalyzer.Exceptions;
using Dev.Editor.BusinessLogic.Models.Messages;
using Dev.Editor.BusinessLogic.Types.Main;
using Dev.Editor.BusinessLogic.Types.Tools;
using Dev.Editor.BusinessLogic.Types.BottomTools;

namespace Dev.Editor.BusinessLogic.ViewModels.Main
{
    public partial class MainWindowViewModel
    {
        // Private methods ----------------------------------------------------------

        private static string GenerateBlankFileName(int i)
        {
            return $"{Strings.BlankDocumentName}{i}.txt";
        }

        private BaseDocumentViewModel FindDocument(string filename)
        {
            return documentsManager.AllDocuments.FirstOrDefault(d => string.Equals(d.FileName.ToLower(), filename.ToLower()));
        }

        private (string filename, string path) GetSuggestedFilenamePath(BaseDocumentViewModel document)
        {
            string filename = document.FileName;
            string path = null;

            if (document.FilenameVirtual)
            {
                if (explorerToolViewModel.SelectedFolder != null)
                    path = explorerToolViewModel.SelectedFolder.GetFullPath();
            }
            else
            {
                path = System.IO.Path.GetDirectoryName(document.FileName);
            }

            return (filename, path);
        }

        // *** Text document ***

        private void InternalWriteTextDocument(TextDocumentViewModel document, string filename)
        {
            using (var fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                using (var writer = new StreamWriter(fs))
                {
                    document.Document.WriteTextTo(writer);
                }
            }
        }

        private void InternalReadTextDocument(TextDocumentViewModel document, string filename)
        {
            using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(fs))
                {
                    document.Document.Text = reader.ReadToEnd();
                }
            }
        }

        private bool InternalSaveTextDocument(TextDocumentViewModel document, string filename)
        {
            try
            {
                InternalWriteTextDocument(document, filename);

                document.Document.UndoStack.MarkAsOriginalFile();

                var modificationDate = System.IO.File.GetLastWriteTimeUtc(document.FileName);
                document.LastModificationDate = modificationDate;

                return true;
            }
            catch (Exception e)
            {
                messagingService.ShowError(String.Format(Strings.Message_CannotSaveFile, document.FileName, e.Message));
                return false;
            }
        }

        private void InternalLoadTextDocument(TextDocumentViewModel document, string filename)
        {
            InternalReadTextDocument(document, filename);

            document.SetFilename(filename, fileIconProvider.GetImageForFile(filename));
            document.Document.UndoStack.ClearAll();
            document.Document.UndoStack.MarkAsOriginalFile();
            document.FilenameVirtual = false;
            document.Highlighting = highlightingProvider.GetDefinitionByExtension(Path.GetExtension(filename));

            var modificationDate = System.IO.File.GetLastWriteTimeUtc(document.FileName);
            document.LastModificationDate = modificationDate;

        }

        private BaseDocumentViewModel LoadTextDocument(DocumentTabKind documentTabKind, string filename, int? index = null)
        {
            var document = FindDocument(filename);
                
            if (document == null)
            {
                var textDocument = new TextDocumentViewModel(this, Guid.NewGuid());
                
                InternalLoadTextDocument(textDocument, filename);

                if (index == null)
                    documentsManager.AddDocument(textDocument, documentTabKind);
                else
                    documentsManager.InsertDocument(index.Value, textDocument, documentTabKind);

                document = textDocument;
            }

            documentsManager.ActiveDocument = document;

            return document;
        }

		private bool SaveTextDocument(TextDocumentViewModel document)
        {
            if (document.FilenameVirtual)
                throw new InvalidOperationException("Cannot save document with virtual filename!");

            return InternalSaveTextDocument(document, document.FileName);
        }

		private bool SaveTextDocumentAs(TextDocumentViewModel document)
        {
            (string filename, string path) = GetSuggestedFilenamePath(document);

            var fileDialogResult = dialogService.ShowSaveDialog(null, null, filename, path);
            if (fileDialogResult.Result && InternalSaveTextDocument(document, fileDialogResult.FileName))
            {
                document.SetFilename(fileDialogResult.FileName, fileIconProvider.GetImageForFile(fileDialogResult.FileName));
                document.FilenameVirtual = false;
                return true;
            }

            return false;
        }

        private void DoNewTextDocument(DocumentTabKind documentTabKind, string initialText = null, int? index = null)
        {
            var newDocument = new TextDocumentViewModel(this, Guid.NewGuid());
            
            int i = 1;
            while (documentsManager.AllDocuments.Any(d => d.FileName.Equals(GenerateBlankFileName(i))))
                i++;

            string newFilename = GenerateBlankFileName(i);
            newDocument.SetFilename(newFilename, fileIconProvider.GetImageForFile(newFilename));
            newDocument.Highlighting = highlightingProvider.EmptyHighlighting;

            if (initialText != null)
                newDocument.Document.Text = initialText;

            if (index == null)
                documentsManager.AddDocument(newDocument, documentTabKind);
            else
                documentsManager.InsertDocument(index.Value, newDocument, documentTabKind);

            documentsManager.ActiveDocument = newDocument;
        }

        private void DoOpenTextDocument(DocumentTabKind documents)
        {
            var dialogResult = dialogService.ShowOpenDialog();
            if (dialogResult.Result)
            {
                try
                {
                    LoadTextDocument(documents, dialogResult.FileName);                    
                }
                catch (Exception e)
                {
                    messagingService.ShowError(string.Format(Strings.Message_CannotOpenFile, dialogResult.FileName, e.Message));
                }
            }
        }
       
        // *** Hex document ***

        private void InternalWriteHexDocument(HexDocumentViewModel document, string filename)
        {
            using (var fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                document.Document.SaveToStream(fs);
            }
        }

        private void InternalReadHexDocument(HexDocumentViewModel document, string filename)
        {
            using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                document.Document.LoadFromStream(fs);
            }
        }

        private bool InternalSaveHexDocument(HexDocumentViewModel document, string filename)
        {
            try
            {
                InternalWriteHexDocument(document, filename);

                document.Changed = false;

                var modificationDate = System.IO.File.GetLastWriteTimeUtc(document.FileName);
                document.LastModificationDate = modificationDate;

                return true;
            }
            catch (Exception e)
            {
                messagingService.ShowError(String.Format(Strings.Message_CannotSaveFile, document.FileName, e.Message));
                return false;
            }
        }

        private void InternalLoadHexDocument(HexDocumentViewModel document, string filename)
        {
            InternalReadHexDocument(document, filename);

            document.SetFilename(filename, fileIconProvider.GetImageForFile(filename));
            document.Document.ClearUndoHistory();
            document.Changed = false;
            document.FilenameVirtual = false;

            var modificationDate = System.IO.File.GetLastWriteTimeUtc(document.FileName);
            document.LastModificationDate = modificationDate;
        }

        private BaseDocumentViewModel LoadHexDocument(DocumentTabKind documentTabKind, string filename, int? index = null)
        {
            var document = FindDocument(filename);

            if (document == null)
            {
                var hexDocument = new HexDocumentViewModel(this, Guid.NewGuid());

                InternalLoadHexDocument(hexDocument, filename);

                if (index == null)
                    documentsManager.AddDocument(hexDocument, documentTabKind);
                else
                    documentsManager.InsertDocument(index.Value, hexDocument, documentTabKind);
                
                document = hexDocument;
            }

            documentsManager.ActiveDocument = document;

            return document;
        }

        private bool SaveHexDocument(HexDocumentViewModel document)
        {
            if (document.FilenameVirtual)
                throw new InvalidOperationException("Cannot save document with virtual filename!");

            return InternalSaveHexDocument(document, document.FileName);
        }

        private bool SaveHexDocumentAs(HexDocumentViewModel document)
        {
            (string filename, string path) = GetSuggestedFilenamePath(document);

            var fileDialogResult = dialogService.ShowSaveDialog(null, null, filename, path);

            if (fileDialogResult.Result && InternalSaveHexDocument(document, fileDialogResult.FileName))
            {
                document.SetFilename(fileDialogResult.FileName, fileIconProvider.GetImageForFile(fileDialogResult.FileName));
                document.FilenameVirtual = false;
                return true;
            }

            return false;
        }

        private void DoNewHexDocument(DocumentTabKind documentTabKind, int? index = null)
        {
            var newDocument = new HexDocumentViewModel(this, Guid.NewGuid());

            int i = 1;
            while (documentsManager.AllDocuments.Any(d => d.FileName.Equals(GenerateBlankFileName(i))))
                i++;

            string newFilename = GenerateBlankFileName(i);
            newDocument.SetFilename(newFilename, fileIconProvider.GetImageForFile(newFilename));

            if (index == null)
                documentsManager.AddDocument(newDocument, documentTabKind);
            else
                documentsManager.InsertDocument(index.Value, newDocument, documentTabKind);

            documentsManager.ActiveDocument = newDocument;
        }

        private void DoOpenHexDocument(DocumentTabKind documents)
        {
            var dialogResult = dialogService.ShowOpenDialog();
            if (dialogResult.Result)
            {
                try
                {
                    LoadHexDocument(documents, dialogResult.FileName);
                }
                catch (Exception e)
                {
                    messagingService.ShowError(string.Format(Strings.Message_CannotOpenFile, dialogResult.FileName, e.Message));
                }
            }
        }

        private void DoOpenBinDocument()
        {
            // Show binary definitions pane
            if (sidePanelPlacement == Types.UI.SidePanelPlacement.Hidden)
            {
                SidePanelPlacement = Types.UI.SidePanelPlacement.Right;
            }
            
            SelectedSideTool = SideTool.BinaryDefinitions;

            messagingService.Inform(Strings.Message_UseSidePanelToOpenBinFile);
        }

        // *** Binary Document ***

        private void InternalReadBinDocument(BinDocumentViewModel document, string filename, BinDefinition binDefinition)
        {
            // Try compile binary definition
            string defSource = File.ReadAllText($"{pathService.BinDefinitionsPath}\\{binDefinition.Filename.Value}");

            var analyzer = BinAnalyzer.Compiler.Compile(defSource);

            using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                var result = analyzer.Analyze(fs);

                document.Document = result;
                document.Definition = binDefinition;
            }
        }

        private void InternalLoadBinDocument(BinDocumentViewModel document, string filename, BinDefinition binDefinition)
        {
            InternalReadBinDocument(document, filename, binDefinition);

            document.SetFilename(filename, fileIconProvider.GetImageForFile(filename));
            document.Changed = false;
            document.FilenameVirtual = false;

            var modificationDate = System.IO.File.GetLastWriteTimeUtc(document.FileName);
            document.LastModificationDate = modificationDate;
        }

        private BaseDocumentViewModel LoadBinDocument(DocumentTabKind documentTabKind, string filename, BinDefinition binDefinition, int? index = null)
        {
            var document = FindDocument(filename);
                
            if (document == null)
            {
                var binDocument = new BinDocumentViewModel(this, Guid.NewGuid());

                InternalLoadBinDocument(binDocument, filename, binDefinition);

                if (index == null)
                    documentsManager.AddDocument(binDocument, documentTabKind);
                else
                    documentsManager.InsertDocument(index.Value, binDocument, documentTabKind);

                document = binDocument;
            }

            documentsManager.ActiveDocument = document;

            return document;
        }

        private void DoOpenBinDocument(DocumentTabKind documents, BinDefinition binDefinition)
        {
            var dialogResult = dialogService.ShowOpenDialog(Strings.DefaultFilter, 
                string.Format(Strings.OpenBinaryFile_Title, binDefinition.DefinitionName.Value));
            if (dialogResult.Result)
            {
                messagesBottomToolViewModel.ClearMessages();

                try
                {
                    LoadBinDocument(documents, dialogResult.FileName, binDefinition);
                }
                catch (BaseSourceReferenceException e)
                {
                    messagesBottomToolViewModel.AddMessage(new MessageModel(e.LocalizedErrorMessage,
                        Types.Messages.MessageSeverity.Error,
                        $"{pathService.BinDefinitionsPath}\\{binDefinition.Filename.Value}",
                        e.Line,
                        e.Column));

                    messagingService.ShowError(string.Format(Strings.Message_CannotOpenBinFile));

                    BottomPanelVisibility = Types.UI.BottomPanelVisibility.Visible;

                    SelectedBottomTool = BottomTool.Messages;
                }
                catch (Exception e)
                {
                    messagesBottomToolViewModel.AddMessage(new MessageModel(e.Message,
                        Types.Messages.MessageSeverity.Critical,
                        $"{pathService.BinDefinitionsPath}\\{binDefinition.Filename.Value}"));

                    messagingService.ShowError(string.Format(Strings.Message_CannotOpenBinFile));

                    BottomPanelVisibility = Types.UI.BottomPanelVisibility.Visible;

                    SelectedBottomTool = BottomTool.Messages;
                }
            }
        }

        // *** General ***

        private void InternalWriteDocument(BaseDocumentViewModel document, string filename)
        {
            switch (document)
            {
                case TextDocumentViewModel textDocument:
                    {
                        InternalWriteTextDocument(textDocument, filename);
                        break;
                    }
                case HexDocumentViewModel hexDocument:
                    {
                        InternalWriteHexDocument(hexDocument, filename);
                        break;
                    }
                case BinDocumentViewModel binDocument:
                    throw new InvalidOperationException("BinDocument doesn't support saving!");
                default:
                    throw new InvalidOperationException("Unsupported document type!");
            }
        }

        private void DoSaveDocument()
        {
            if (documentsManager.ActiveDocument.FilenameVirtual)
            {
                DoSaveDocumentAs();
            }
            else
            {
                switch (documentsManager.ActiveDocument)
                {
                    case TextDocumentViewModel textDocument:
                        SaveTextDocument(textDocument);
                        break;
                    case HexDocumentViewModel hexDocument:
                        SaveHexDocument(hexDocument);
                        break;
                    case BinDocumentViewModel binDocument:
                        throw new InvalidOperationException("Bin document doesn't support saving!");
                    default:
                        throw new InvalidOperationException("Unsupported document type!");
                }

            }
        }

        private void DoSaveDocumentAs()
        {
            switch (documentsManager.ActiveDocument)
            {
                case TextDocumentViewModel textDocument:
                    {
                        if (!SaveTextDocumentAs(textDocument))
                            return;

                        break;
                    }
                case HexDocumentViewModel hexDocument:
                    {
                        if (!SaveHexDocumentAs(hexDocument))
                            return;

                        break;
                    }
                case BinDocumentViewModel binDocument:
                    {
                        throw new InvalidOperationException("Bin document doesn't support saving!");
                    }
                default:
                    {
                        throw new InvalidOperationException("Unsupported document type!");
                    }
            }

            // File might have been saved in location chosen in explorer tool
            explorerToolViewModel.Refresh();

            var modificationDate = System.IO.File.GetLastWriteTimeUtc(documentsManager.ActiveDocument.FileName);
            documentsManager.ActiveDocument.LastModificationDate = modificationDate;
        }

        private bool SaveDocument(BaseDocumentViewModel document)
        {
            switch (document)
            {
                case TextDocumentViewModel textDocument:
                    return SaveTextDocument(textDocument);
                case HexDocumentViewModel hexDocument:
                    return SaveHexDocument(hexDocument);
                case BinDocumentViewModel binDocument:
                    throw new InvalidOperationException("Bin document doesn't support saving!");
                default:
                    throw new InvalidOperationException("Unsupported document type!");
            }
        }

        private bool SaveDocumentAs(BaseDocumentViewModel document)
        {
            switch (document)
            {
                case TextDocumentViewModel textDocument:
                    return SaveTextDocumentAs(textDocument);
                case HexDocumentViewModel hexDocument:
                    return SaveHexDocumentAs(hexDocument);
                case BinDocumentViewModel binDocument:
                    throw new InvalidOperationException("Bin document doesn't support saving!");
                default:
                    throw new InvalidOperationException("Unsupported document type!");
            }
        }        

        private void ReplaceReloadDocument(DocumentTabKind documentTabKind, BaseDocumentViewModel document)
        {
            int index = documentsManager[documentTabKind].IndexOf(document);
            
            if (index < 0)
                throw new ArgumentException("Invalid document!");

            documentsManager.RemoveDocumentAt(documentTabKind, index);

            switch (document)
            {
                case TextDocumentViewModel textDocumentViewModel:
                    {
                        LoadTextDocument(documentTabKind, textDocumentViewModel.FileName, index);
                        break;
                    }
                case HexDocumentViewModel hexDocumentViewModel:
                    {
                        LoadHexDocument(documentTabKind, hexDocumentViewModel.FileName, index);
                        break;
                    }
                case BinDocumentViewModel binDocumentViewModel:
                    {
                        LoadBinDocument(documentTabKind, binDocumentViewModel.FileName, binDocumentViewModel.Definition, index);
                        break;
                    }
                default:
                    throw new InvalidOperationException("Unsupported document type!");
            }
        }
    }
}
