﻿using Dev.Editor.Resources;
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
            return documents.FirstOrDefault(d => string.Equals(d.FileName.ToLower(), filename.ToLower()));
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

        private void LoadTextDocument(string filename, int? index = null)
        {
            var document = FindDocument(filename);
                
            if (document == null)
            {
                var textDocument = new TextDocumentViewModel(this);
                
                InternalLoadTextDocument(textDocument, filename);

                documents.Insert(index ?? documents.Count, textDocument);

                document = textDocument;
            }

            ActiveDocument = document;
        }

		private bool SaveTextDocument(TextDocumentViewModel document)
        {
            if (document.FilenameVirtual)
                throw new InvalidOperationException("Cannot save document with virtual filename!");

            return InternalSaveTextDocument(document, document.FileName);
        }

		private bool SaveTextDocumentAs(TextDocumentViewModel document)
        {
            var fileDialogResult = dialogService.ShowSaveDialog();
            if (fileDialogResult.Result && InternalSaveTextDocument(document, fileDialogResult.FileName))
            {
                document.SetFilename(fileDialogResult.FileName, fileIconProvider.GetImageForFile(fileDialogResult.FileName));
                document.FilenameVirtual = false;
                return true;
            }

            return false;
        }

        private void DoNewTextDocument(string initialText = null, int? index = null)
        {
            var newDocument = new TextDocumentViewModel(this);
            
            int i = 1;
            while (documents.Any(d => d.FileName.Equals(GenerateBlankFileName(i))))
                i++;

            string newFilename = GenerateBlankFileName(i);
            newDocument.SetFilename(newFilename, fileIconProvider.GetImageForFile(newFilename));
            newDocument.Highlighting = highlightingProvider.EmptyHighlighting;

            if (initialText != null)
                newDocument.Document.Text = initialText;

            documents.Insert(index ?? documents.Count, newDocument);

            ActiveDocument = newDocument;
        }

        private void DoOpenTextDocument()
        {
            var dialogResult = dialogService.ShowOpenDialog();
            if (dialogResult.Result)
            {
                try
                {
                    LoadTextDocument(dialogResult.FileName);                    
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

        private void LoadHexDocument(string filename, int? index = null)
        {
            var document = FindDocument(filename);

            if (document == null)
            {
                var hexDocument = new HexDocumentViewModel(this);

                InternalLoadHexDocument(hexDocument, filename);

                documents.Insert(index ?? documents.Count, hexDocument);

                document = hexDocument;
            }

            ActiveDocument = document;            
        }

        private bool SaveHexDocument(HexDocumentViewModel document)
        {
            if (document.FilenameVirtual)
                throw new InvalidOperationException("Cannot save document with virtual filename!");

            return InternalSaveHexDocument(document, document.FileName);
        }

        private bool SaveHexDocumentAs(HexDocumentViewModel document)
        {
            var fileDialogResult = dialogService.ShowSaveDialog();
            if (fileDialogResult.Result && InternalSaveHexDocument(document, fileDialogResult.FileName))
            {
                document.SetFilename(fileDialogResult.FileName, fileIconProvider.GetImageForFile(fileDialogResult.FileName));
                document.FilenameVirtual = false;
                return true;
            }

            return false;
        }

        private void DoNewHexDocument(int? index = null)
        {
            var newDocument = new HexDocumentViewModel(this);

            int i = 1;
            while (documents.Any(d => d.FileName.Equals(GenerateBlankFileName(i))))
                i++;

            string newFilename = GenerateBlankFileName(i);
            newDocument.SetFilename(newFilename, fileIconProvider.GetImageForFile(newFilename));

            documents.Insert(index ?? documents.Count, newDocument);

            ActiveDocument = newDocument;
        }

        private void DoOpenHexDocument()
        {
            var dialogResult = dialogService.ShowOpenDialog();
            if (dialogResult.Result)
            {
                try
                {
                    LoadHexDocument(dialogResult.FileName);
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

            #warning TODO!
            // SelectedTool = binDefinitionsToolViewModel;

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

        private void LoadBinDocument(string filename, BinDefinition binDefinition, int? index = null)
        {
            var document = FindDocument(filename);
                
            if (document == null)
            {
                var binDocument = new BinDocumentViewModel(this);

                InternalLoadBinDocument(binDocument, filename, binDefinition);

                documents.Insert(index ?? documents.Count, binDocument);

                document = binDocument;
            }

            ActiveDocument = document;
        }

        private void DoOpenBinDocument(BinDefinition binDefinition)
        {
            var dialogResult = dialogService.ShowOpenDialog(Strings.DefaultFilter, string.Format(Strings.OpenBinaryFile_Title, binDefinition.DefinitionName.Value));
            if (dialogResult.Result)
            {
                messagesBottomToolViewModel.ClearMessages();

                try
                {
                    LoadBinDocument(dialogResult.FileName, binDefinition);
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

                    #warning TODO!
                    // SelectedBottomTool = messagesBottomToolViewModel;
                }
                catch (Exception e)
                {
                    messagesBottomToolViewModel.AddMessage(new MessageModel(e.Message,
                        Types.Messages.MessageSeverity.Critical,
                        $"{pathService.BinDefinitionsPath}\\{binDefinition.Filename.Value}"));

                    messagingService.ShowError(string.Format(Strings.Message_CannotOpenBinFile));

                    BottomPanelVisibility = Types.UI.BottomPanelVisibility.Visible;
                    
                    #warning TODO!
                    // SelectedBottomTool = messagesBottomToolViewModel;
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
            if (activeDocument.FilenameVirtual)
            {
                DoSaveDocumentAs();
            }
            else
            {
                switch (activeDocument)
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
            switch (activeDocument)
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

            var modificationDate = System.IO.File.GetLastWriteTimeUtc(activeDocument.FileName);
            activeDocument.LastModificationDate = modificationDate;
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

        private void ReplaceReloadDocument(BaseDocumentViewModel document)
        {
            int index = documents.IndexOf(document);
            
            if (index < 0)
                throw new ArgumentException("Invalid document!");

            RemoveDocument(document);

            switch (document)
            {
                case TextDocumentViewModel textDocumentViewModel:
                    {
                        LoadTextDocument(textDocumentViewModel.FileName, index);
                        break;
                    }
                case HexDocumentViewModel hexDocumentViewModel:
                    {
                        LoadHexDocument(hexDocumentViewModel.FileName, index);
                        break;
                    }
                case BinDocumentViewModel binDocumentViewModel:
                    {
                        LoadBinDocument(binDocumentViewModel.FileName, binDocumentViewModel.Definition, index);
                        break;
                    }
                default:
                    throw new InvalidOperationException("Unsupported document type!");
            }
        }
    }
}
