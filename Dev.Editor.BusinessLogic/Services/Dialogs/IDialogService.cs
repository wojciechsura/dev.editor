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
        OpenDialogResult OpenDialog(string filter = null, string title = null, string filename = null);
        SaveDialogResult SaveDialog(string filter = null, string title = null, string filename = null);

        SearchReplaceWindowViewModel RequestSearchReplace(ISearchHost searchHost);
    }
}
