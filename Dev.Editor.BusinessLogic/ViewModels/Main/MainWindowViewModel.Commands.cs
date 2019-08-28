using Dev.Editor.BusinessLogic.ViewModels.Document;
using Dev.Editor.Common.Conditions;
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

        private readonly Condition documentExistsCondition;
        private readonly Condition documentIsTextCondition;
        private readonly MutableSourcePropertyWatchCondition<MainWindowViewModel, BaseDocumentViewModel> canUndoCondition;
        private readonly MutableSourcePropertyWatchCondition<MainWindowViewModel, BaseDocumentViewModel> canRedoCondition;
        private readonly MutableSourcePropertyWatchCondition<MainWindowViewModel, BaseDocumentViewModel> selectionAvailableCondition;
        private readonly MutableSourcePropertyWatchCondition<MainWindowViewModel, BaseDocumentViewModel> regularSelectionAvailableCondition;
        private readonly MutableSourcePropertyNotNullWatchCondition<MainWindowViewModel, BaseDocumentViewModel> searchPerformedCondition;

        // Public properties --------------------------------------------------

        // File

        public ICommand NewTextCommand { get; }
        public ICommand NewHexCommand { get; }
        public ICommand OpenTextCommand { get; }
        public ICommand OpenHexCommand { get; }
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

        // Navigation

        public ICommand NavigateCommand { get; }

        // Lines

        public ICommand SortLinesAscendingCommand { get; }
        public ICommand SortLinesDescendingCommand { get; }
        public ICommand RemoveEmptyLinesCommand { get; }
        public ICommand RemoveWhitespaceLinesCommand { get; }
    }
}
