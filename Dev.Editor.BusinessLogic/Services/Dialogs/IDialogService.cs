using Dev.Editor.BusinessLogic.Models.Dialogs;
using Dev.Editor.BusinessLogic.Models.DuplicatedLines;
using Dev.Editor.BusinessLogic.ViewModels.Search;
using Dev.Editor.BusinessLogic.ViewModels.SubstitutionCipher;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Services.Dialogs
{
    public interface IDialogService
    {
        // TODO refactor to multiple returned values
        OpenDialogResult ShowOpenDialog(string filter = null, string title = null, string filename = null, string path = null);
        SaveDialogResult ShowSaveDialog(string filter = null, string title = null, string filename = null, string path = null);
        (bool result, List<string> files) ShowOpenFilesDialog(string filter = null, string title = null, string path = null);

        void ShowConfigurationDialog();

        SearchReplaceWindowViewModel RequestSearchReplace(ISearchHost searchHost);
        (bool, NameDialogResult) ShowChooseNameDialog(NameDialogModel model);
        (bool, DiffConfigDialogResult) ShowDiffConfigDialog(DiffConfigDialogModel model);
        (bool, EscapeConfigResult) ShowEscapeConfigDialog(EscapeConfigModel escapeConfigModel);
        void ShowExceptionDialog(Exception e);
        (bool result, string newLocation) ShowChooseFolderDialog(string location);
        void ShowProgressDialog(string operationTitle, BackgroundWorker worker);
        void ShowProgressDialog(string operationTiele, BackgroundWorker worker, object workerParameter);
        (bool, DuplicatedLinesFinderConfig) ShowDuplicatedLinesFinderConfigDialog(DuplicatedLinesFinderConfigModel model);
        void OpenSubstitutionCipherWindow(ISubstitutionCipherHost host);
        (bool, string) ShowAlphabetDialog(string message, string previousAlphabet = null);
    }
}
