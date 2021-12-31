using Dev.Editor.BusinessLogic.Models.Dialogs;
using Dev.Editor.Resources;
using Dev.Editor.BusinessLogic.Services.Dialogs;
using Dev.Editor.BusinessLogic.ViewModels.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Dev.Editor.BusinessLogic.Models.Configuration.BinDefinitions;
using System.ComponentModel;
using Dev.Editor.BusinessLogic.Models.DuplicatedLines;
using Microsoft.Win32;
using Dev.Editor.BusinessLogic.ViewModels.SubstitutionCipher;
using Dev.Editor.BusinessLogic.ViewModels.WebBrowserWindow;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Dev.Editor.Services.Dialogs
{
    class DialogService : IDialogService
    {
        private readonly Dictionary<IWebBrowserHost, WebBrowserWindow> webBrowserWindows = new Dictionary<IWebBrowserHost, WebBrowserWindow>();
        private readonly Dictionary<ISearchHost, SearchReplaceWindow> searchWindows = new Dictionary<ISearchHost, SearchReplaceWindow>();
        private readonly Dictionary<ISubstitutionCipherHost, SubstitutionCipherWindow> substitutionCipherWindows = new Dictionary<ISubstitutionCipherHost, SubstitutionCipherWindow>();

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

        public (bool result, List<string> files) ShowOpenFilesDialog(string filter = null, string title = null, string path = null)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            SetupFileDialog(dialog, filter, title, null, path);
            dialog.Multiselect = true;

            if (dialog.ShowDialog() == true)
                return (true, dialog.FileNames.ToList());
            else
                return (false, null);
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

        public (bool, DiffConfigDialogResult) ShowDiffConfigDialog(DiffConfigDialogModel model)
        {
            var dialog = new DiffConfigDialog(model);
            if (dialog.ShowDialog() == true)
                return (true, dialog.Result);
            else
                return (false, null);
        }

        public (bool, EscapeConfigResult) ShowEscapeConfigDialog(EscapeConfigModel model)
        {
            var dialog = new EscapeConfigDialog(model);
            if (dialog.ShowDialog() == true)
                return (true, dialog.Result);
            else
                return (false, null);
        }

        public void ShowExceptionDialog(Exception e)
        {
            var dialog = new ExceptionWindow(e);
            dialog.ShowDialog();
        }

        public (bool result, string newLocation) ShowChooseFolderDialog(string location)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.InitialDirectory = location;
            
            CommonFileDialogResult result = dialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
                return (true, dialog.FileName);
            else
                return (false, null);

            /*
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.SelectedPath = location;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                return (true, dialog.SelectedPath);
            else
                return (false, null);            
            */
        }

        public void ShowProgressDialog(string operationTitle, BackgroundWorker worker)
        {
            var progressWindow = new ProgressWindow(operationTitle, worker, null);
            progressWindow.ShowDialog();
        }

        public void ShowProgressDialog(string operationTitle, BackgroundWorker worker, object workerParameter)
        {
            var progressWindow = new ProgressWindow(operationTitle, worker, workerParameter);
            progressWindow.ShowDialog();
        }

        public (bool, DuplicatedLinesFinderConfig) ShowDuplicatedLinesFinderConfigDialog(DuplicatedLinesFinderConfigModel model)
        {
            var dialog = new DuplicatedLinesFinderConfigDialog(model);
            if (dialog.ShowDialog() == true)
                return (true, dialog.Result);
            else
                return (false, null);
        }

        public void OpenSubstitutionCipherWindow(ISubstitutionCipherHost host)
        {
            if (!substitutionCipherWindows.ContainsKey(host))
            {
                SubstitutionCipherWindow substitutionCipherWindow = new SubstitutionCipherWindow(host);
                substitutionCipherWindow.Owner = Application.Current.MainWindow;
                substitutionCipherWindows.Add(host, substitutionCipherWindow);
            }

            substitutionCipherWindows[host].Show();
        }

        public (bool, string) ShowAlphabetDialog(string message, string previousAlphabet = null)
        {
            var dialog = new AlphabetDialogWindow(message, previousAlphabet);
            if (dialog.ShowDialog() == true)
                return (true, dialog.Result);
            else
                return (false, null);
        }

        public WebBrowserWindowViewModel RequestWebBrowser(IWebBrowserHost webBrowserHost)
        {
            if (!webBrowserWindows.ContainsKey(webBrowserHost))
            {
                WebBrowserWindow webBrowserWindow = new WebBrowserWindow(webBrowserHost);
                webBrowserWindow.Owner = Application.Current.MainWindow;
                webBrowserWindows.Add(webBrowserHost, webBrowserWindow);
            }

            return webBrowserWindows[webBrowserHost].ViewModel;

        }
    }
}
