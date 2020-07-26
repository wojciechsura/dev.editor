using Dev.Editor.BusinessLogic.Models.Dialogs;
using Dev.Editor.BusinessLogic.ViewModels.Search;
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
        OpenDialogResult ShowOpenDialog(string filter = null, string title = null, string filename = null, string path = null);
        SaveDialogResult ShowSaveDialog(string filter = null, string title = null, string filename = null, string path = null);
        void ShowConfigurationDialog();

        SearchReplaceWindowViewModel RequestSearchReplace(ISearchHost searchHost);
        (bool, NameDialogResult) ShowChooseNameDialog(NameDialogModel model);
        (bool, DiffConfigDialogResult) ShowDiffConfigDialog(DiffConfigDialogModel model);
        (bool, EscapeConfigResult) ShowEscapeConfigDialog(EscapeConfigModel escapeConfigModel);
        void ShowExceptionDialog(Exception e);
        (bool result, string newLocation) ShowChooseFolderDialog(string location);
        void ShowProgressDialog(BackgroundWorker worker);
    }
}
