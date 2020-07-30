using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dev.Editor.BusinessLogic.ViewModels.Document
{
    public interface IDocumentHandler : INotifyPropertyChanged
    {
        bool WordWrap { get; }
        bool LineNumbers { get; }

        void RequestClose(BaseDocumentViewModel documentViewModel);
        void RequestCloseOthers(BaseDocumentViewModel baseDocumentViewModel);
        void RequestCloseAllButPinned();
        void RequestCloseAll();

        ICommand CopyCommand { get; }
        ICommand CutCommand { get; }
        ICommand PasteCommand { get; }

        void ChildActivated(BaseDocumentViewModel baseDocumentViewModel);
        void MoveToOtherView(BaseDocumentViewModel baseDocumentViewModel);
        void RequestClearAllDiffs();
        bool PerformQuickSearch(string quickSearchText, bool next);
    }
}
