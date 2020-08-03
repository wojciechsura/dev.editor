using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit.Document;

namespace Dev.Editor.BusinessLogic.ViewModels.Document
{
    public interface ITextEditorAccess
    {
        void Copy();
        void Cut();
        void Paste();

        (int selStart, int selLength) GetSelection();
        void SetSelection(int selStart, int selLength);
        void ScrollTo(int line, int column);
        string GetSelectedText();
        void RunAsSingleHistoryOperation(Action action);
        void FocusDocument();
        void FocusQuickSearch();

        bool QuickSearchFocused { get; }
    }
}
