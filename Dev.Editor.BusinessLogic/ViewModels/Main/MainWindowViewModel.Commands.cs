using Dev.Editor.BusinessLogic.Models.Highlighting;
using Dev.Editor.BusinessLogic.ViewModels.Document;
using Dev.Editor.BusinessLogic.ViewModels.Main.Documents;
using Dev.Editor.Common.Conditions;
using ICSharpCode.AvalonEdit.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dev.Editor.BusinessLogic.ViewModels.Main
{
    public partial class MainWindowViewModel
    {
        // Private fields -----------------------------------------------------

        private readonly MutablePropertyNotNullCondition<DocumentsManager, BaseDocumentViewModel> documentExistsCondition;
        private readonly MutablePropertyFuncCondition<DocumentsManager, BaseDocumentViewModel> documentIsTextCondition;
        private readonly MutableSourcePropertyWatchCondition<DocumentsManager, BaseDocumentViewModel> canUndoCondition;
        private readonly MutableSourcePropertyWatchCondition<DocumentsManager, BaseDocumentViewModel> canRedoCondition;
        private readonly MutableSourcePropertyWatchCondition<DocumentsManager, BaseDocumentViewModel> canSaveCondition;
        private readonly MutableSourcePropertyWatchCondition<DocumentsManager, BaseDocumentViewModel> selectionAvailableCondition;
        private readonly TextDocumentPropertyNotNullCondition<AnchorSegment> searchAreaAvailableCondition;
        private readonly BaseCondition selectionAvailableForSearchCondition;
        private readonly MutableSourcePropertyWatchCondition<DocumentsManager, BaseDocumentViewModel> regularSelectionAvailableCondition;
        private readonly MutableSourcePropertyNotNullCondition<DocumentsManager, BaseDocumentViewModel> searchPerformedCondition;
        private readonly MutableSourcePropertyFuncCondition<DocumentsManager, BaseDocumentViewModel, HighlightingInfo> xmlToolsetAvailableCondition;
        private readonly MutableSourcePropertyFuncCondition<DocumentsManager, BaseDocumentViewModel, HighlightingInfo> markdownToolsetAvailableCondition;
        private readonly MutableSourcePropertyWatchCondition<DocumentsManager, BaseDocumentViewModel> documentPathVirtualCondition;
        private readonly BaseCondition documentHasPathCondition;

        // Public properties --------------------------------------------------

        // General

        public ICommand ConfigCommand { get; }

        public ICommand RunStoredSearchCommand { get; }

        public ICommand NewPrimaryTextCommand { get; }

        public ICommand NewSecondaryTextCommand { get; }

        // File

        public ICommand NewTextCommand { get; }
        public ICommand NewHexCommand { get; }
        public ICommand OpenTextCommand { get; }
        public ICommand OpenHexCommand { get; }
        public ICommand OpenBinCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand SaveAsCommand { get; }

        // Edit

        public ICommand UndoCommand { get; }
        public ICommand RedoCommand { get; }
        public ICommand CopyCommand { get; }
        public ICommand CutCommand { get; }
        public ICommand PasteCommand { get; }

        // Search

        public ICommand SearchCommand { get; }
        public ICommand ReplaceCommand { get; }
        public ICommand FindNextCommand { get; }

        // Comparing

        public ICommand CompareCommand { get; }

        // Navigation

        public ICommand NavigateCommand { get; }

        // Lines

        public ICommand SortLinesAscendingCommand { get; }
        public ICommand SortLinesDescendingCommand { get; }
        public ICommand RemoveEmptyLinesCommand { get; }
        public ICommand RemoveWhitespaceLinesCommand { get; }

        // Text

        public ICommand LowercaseCommand { get; }
        public ICommand UppercaseCommand { get; }
        public ICommand NamingCaseCommand { get; }
        public ICommand SentenceCaseCommand { get; }
        public ICommand InvertCaseCommand { get; }

        public ICommand Base64EncodeCommand { get; }
        public ICommand Base64DecodeCommand { get; }

        public ICommand HtmlEntitiesEncodeCommand { get; }
        public ICommand HtmlEntitiesDecodeCommand { get; }

        public ICommand EscapeCommand { get; }
        public ICommand UnescapeCommand { get; }

        // XmlTools

        public ICommand FormatXmlCommand { get; }
        public ICommand TransformXsltCommand { get; }

        // MarkdownTools

        public ICommand InsertMarkdownHeader1Command { get; }
        public ICommand InsertMarkdownHeader2Command { get; }
        public ICommand InsertMarkdownHeader3Command { get; }
        public ICommand InsertMarkdownHeader4Command { get; }
        public ICommand InsertMarkdownHeader5Command { get; }
        public ICommand InsertMarkdownHeader6Command { get; }

        public ICommand InsertMarkdownBlockCodeCommand { get; }
        public ICommand InsertMarkdownBlockquoteCommand { get; }
        public ICommand InsertMarkdownOrderedListCommand { get; }
        public ICommand InsertMarkdownUnorderedListCommand { get; }
        public ICommand InsertMarkdownInlineCodeCommand { get; }
        public ICommand InsertMarkdownEmphasisCommand { get; }
        public ICommand InsertMarkdownStrongCommand { get; }
        public ICommand InsertMarkdownLinkCommand { get; }
        public ICommand InsertMarkdownPictureCommand { get; }
        public ICommand InsertMarkdownHorizontalRuleCommand { get; }
    }   
}
