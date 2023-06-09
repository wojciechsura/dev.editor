﻿using Autofac;
using Dev.Editor.BusinessLogic.ViewModels.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Dev.Editor
{
    /// <summary>
    /// Logika interakcji dla klasy SearchReplaceWindow.xaml
    /// </summary>
    public partial class SearchReplaceWindow : Window, ISearchReplaceWindowAccess
    {
        private readonly SearchReplaceWindowViewModel viewModel;

        private void HandleWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();

            // This window's focus and owner window's focus are independent
            // However, user expects, that focusing this window focuses
            // the application, so we have to ensure, that main window gains
            // focus at this point.
            if (Owner != null)
            {
                Owner.Focus();
            }

            viewModel.NotifyClosed();
        }

        private void HandleStoredSearchesDoubleClick(object sender, MouseButtonEventArgs e)
        {
            viewModel.StoredSearchSelected();
        }

        void ISearchReplaceWindowAccess.Close()
        {
            Close();
        }

        public SearchReplaceWindow(ISearchHost searchHost)
        {
            InitializeComponent();

            viewModel = Dependencies.Container.Instance.Resolve<SearchReplaceWindowViewModel>(new NamedParameter("searchHost", searchHost),
                new NamedParameter("access", this));

            DataContext = viewModel;
        }

        public SearchReplaceWindowViewModel ViewModel => viewModel;

        public void FocusReplace()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle,
                new Action(delegate()
                {
                    FocusManager.SetFocusedElement(this, cbReplaceSearch);
                    cbReplaceSearch.SelectAll();
                    cbReplaceSearch.FocusTextBox();
                }));
            
        }

        public void FocusSearch()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle,
                new Action(delegate ()
                {
                    FocusManager.SetFocusedElement(this, cbSearchSearch);
                    cbSearchSearch.SelectAll();
                    cbSearchSearch.FocusTextBox();
                }));
        }

        public void FocusFindInFiles()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle,
                new Action(delegate ()
                {
                    FocusManager.SetFocusedElement(this, cbSearchInFilesSearch);
                    cbSearchInFilesSearch.SelectAll();
                    cbSearchInFilesSearch.FocusTextBox();
                }));
        }

        public void FocusReplaceInFiles()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle,
                new Action(delegate ()
                {
                    FocusManager.SetFocusedElement(this, cbReplaceInFilesSearch);
                    cbReplaceInFilesSearch.SelectAll();
                    cbReplaceInFilesSearch.FocusTextBox();
                }));
        }

        public void ShowAndFocus()
        {
            Show();
            Focus();
        }

        private void HandleWindowPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                viewModel.NotifyEscapePressed();
            }
        }
    }
}
