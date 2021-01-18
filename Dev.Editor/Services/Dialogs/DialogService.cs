﻿using Dev.Editor.BusinessLogic.Models.Dialogs;
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
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                return (true, dialog.SelectedPath);
            else
                return (false, null);            

            /*
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.InitialDirectory = location;
            
            CommonFileDialogResult result = dialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
                return (true, dialog.FileName);
            else
                return (false, null);
            */
        }

        public void ShowProgressDialog(string operationTitle, BackgroundWorker worker)
        {
            var progressWindow = new ProgressWindow(operationTitle, worker);
            progressWindow.ShowDialog();
        }

        public (bool, DuplicatedLinesFinderConfig) ShowDuplicatedLinesFinderConfigDialog()
        {
            var dialog = new DuplicatedLinesFinderConfigDialog();
            if (dialog.ShowDialog() == true)
                return (true, dialog.Result);
            else
                return (false, null);
        }
    }
}
