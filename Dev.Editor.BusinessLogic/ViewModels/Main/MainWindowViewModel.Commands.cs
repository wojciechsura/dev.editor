﻿using Dev.Editor.BusinessLogic.Models.Documents.Text;
using Dev.Editor.BusinessLogic.Models.Highlighting;
using Dev.Editor.BusinessLogic.ViewModels.Document;
using Dev.Editor.BusinessLogic.ViewModels.Main.Documents;
using Spooksoft.VisualStateManager.Conditions;
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

        private readonly BaseCondition documentExistsCondition;
        private readonly BaseCondition documentIsTextCondition;
        private readonly BaseCondition canUndoCondition;
        private readonly BaseCondition canRedoCondition;
        private readonly BaseCondition canSaveCondition;
        private readonly BaseCondition selectionAvailableCondition;
        private readonly BaseCondition searchAreaAvailableCondition;
        private readonly BaseCondition selectionAvailableForSearchCondition;
        private readonly BaseCondition regularSelectionAvailableCondition;
        private readonly BaseCondition searchPerformedCondition;
        private readonly BaseCondition xmlToolsetAvailableCondition;
        private readonly BaseCondition markdownToolsetAvailableCondition;
        private readonly BaseCondition jsonToolsetAvailableCondition;
        private readonly BaseCondition documentPathVirtualCondition;
        private readonly BaseCondition documentHasPathCondition;
        private readonly BaseCondition diffDataAvailableCondition;

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
        public ICommand CloseCurrentDocumentCommand { get; }

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
        public ICommand FindInFilesCommand { get; }
        public ICommand ReplaceInFilesCommand { get; }

        // Project

        public ICommand OpenProjectCommand { get; }

        // Comparing

        public ICommand CompareCommand { get; }
        public ICommand PreviousChangeCommand { get; }
        public ICommand NextChangeCommand { get; }

        // Navigation

        public ICommand NavigateCommand { get; }

        // Lines

        public ICommand SortLinesAscendingCommand { get; }
        public ICommand SortLinesDescendingCommand { get; }
        public ICommand RemoveEmptyLinesCommand { get; }
        public ICommand RemoveWhitespaceLinesCommand { get; }
        public ICommand RemoveDuplicatedLinesCommand { get; }
        public ICommand RemoveLinesWithRegexCommand { get; }
        public ICommand RemoveDuplicatedNeighboringLinesCommand { get; }
        public ICommand FindDuplicatedLinesCommand { get; }

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

        public ICommand OpenSubstitutionCipherCommand { get; }

        // XmlTools

        public ICommand FormatXmlCommand { get; }
        public ICommand TransformXsltCommand { get; }

        // MarkdownTools

        public ICommand MarkdownHtmlPreviewCommand { get; }
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

        // JsonTools

        public ICommand FormatJsonCommand { get; }
    }   
}
