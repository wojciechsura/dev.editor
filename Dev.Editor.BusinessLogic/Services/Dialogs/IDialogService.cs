using Dev.Editor.BusinessLogic.Models.Dialogs;
using Dev.Editor.BusinessLogic.ViewModels.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Services.Dialogs
{
    public interface IDialogService
    {
        OpenDialogResult ShowOpenDialog(string filter = null, string title = null, string filename = null);
        SaveDialogResult ShowSaveDialog(string filter = null, string title = null, string filename = null);
        void ShowConfigurationDialog();

        SearchReplaceWindowViewModel RequestSearchReplace(ISearchHost searchHost);
        (bool, NameDialogResult) ShowBinDefinitionDialog(NameDialogModel model);
    }
}
