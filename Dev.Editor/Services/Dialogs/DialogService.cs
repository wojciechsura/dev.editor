using Dev.Editor.BusinessLogic.Models.Dialogs;
using Dev.Editor.Resources;
using Dev.Editor.BusinessLogic.Services.Dialogs;
using Dev.Editor.BusinessLogic.ViewModels.Search;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Dev.Editor.BusinessLogic.Models.Configuration.BinDefinitions;

namespace Dev.Editor.Services.Dialogs
{
    class DialogService : IDialogService
    {
        private readonly Dictionary<ISearchHost, SearchReplaceWindow> searchWindows = new Dictionary<ISearchHost, SearchReplaceWindow>();

        private void SetupFileDialog(FileDialog dialog, string filter, string title, string filename, string path)
        {
            if (filename != null)
                dialog.FileName = filename;

            if (filter != null)
                dialog.Filter = filter;
            else
                dialog.Filter = Strings.DefaultFilter;

            if (title != null)
                dialog.Title = title;
            else
                dialog.Title = Strings.DefaultDialogTitle;

            if (path != null)
                dialog.InitialDirectory = path;
        }

        public OpenDialogResult ShowOpenDialog(string filter = null, string title = null, string filename = null, string path = null)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            SetupFileDialog(dialog, filter, title, filename, path);

            if (dialog.ShowDialog() == true)
                return new OpenDialogResult(true, dialog.FileName);
            else
                return new OpenDialogResult(false, null);
        }

        public SearchReplaceWindowViewModel RequestSearchReplace(ISearchHost searchHost)
        {
            if (!searchWindows.ContainsKey(searchHost))
            {
                SearchReplaceWindow searchReplaceWindow = new SearchReplaceWindow(searchHost);
                searchReplaceWindow.Owner = Application.Current.MainWindow;
                searchWindows.Add(searchHost, searchReplaceWindow);
            }

            return searchWindows[searchHost].ViewModel;
        }

        public SaveDialogResult ShowSaveDialog(string filter = null, string title = null, string filename = null, string path = null)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            SetupFileDialog(dialog, filter, title, filename, path);           

            if (dialog.ShowDialog() == true)
                return new SaveDialogResult(true, dialog.FileName);
            else
                return new SaveDialogResult(false, null);
        }

        public void ShowConfigurationDialog()
        {
            ConfigurationWindow configurationWindow = new ConfigurationWindow();
            configurationWindow.ShowDialog();
        }

        public (bool, NameDialogResult) ShowChooseNameDialog(NameDialogModel model)
        {
            var dialog = new NameDialog(model);
            if (dialog.ShowDialog() == true)
                return (true, dialog.Result);
            else
                return (false, null);
        }

    }
}
