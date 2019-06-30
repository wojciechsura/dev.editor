﻿using Dev.Editor.BusinessLogic.ViewModels.Document;
using Dev.Editor.BusinessLogic.ViewModels.Main;
using Dev.Editor.BusinessLogic.ViewModels.Search;
using Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Unity;
using Unity.Resolution;

namespace Dev.Editor
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow, IMainWindowAccess
    {
        // Private fields -----------------------------------------------------

        private MainWindowViewModel viewModel;

        private readonly Lazy<DispatcherTimer> navigationTimer;

        // Private methods ----------------------------------------------------

        private void HandleDocumentClosing(object sender, Xceed.Wpf.AvalonDock.DocumentClosingEventArgs e)
        {
            e.Cancel = !viewModel.CanCloseDocument(e.Document.Content as DocumentViewModel);
        }

        private void HandleDocumentClosed(object sender, Xceed.Wpf.AvalonDock.DocumentClosedEventArgs e)
        {
            viewModel.NotifyClosedDocument(e.Document.Content as DocumentViewModel);
        }

        private void HandleWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !viewModel.CanCloseApplication();
        }

        private void ShowNavigationPopup()
        {
            pNavigation.IsOpen = true;
            tbNavigation.Focus();
        }

        private void HideNavigationPopup()
        {
            pNavigation.IsOpen = false;
        }

        private void HandleNavigationPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                HideNavigationPopup();
                e.Handled = true;
            }
            else if (e.Key == Key.Enter)
            {
                viewModel.NavigationItemChosen();
            }
            else if (e.Key == Key.Up)
            {
                viewModel.SelectPreviousNavigationItem();
                e.Handled = true;
            }
            else if (e.Key == Key.Down)
            {
                viewModel.SelectNextNavigationItem();
                e.Handled = true;
            }
        }

        private void HandleNavigationTextboxTextChanged(object sender, TextChangedEventArgs e)
        {
            navigationTimer.Value.Start();
        }

        private void NavigationSearch(object sender, EventArgs e)
        {
            navigationTimer.Value.Stop();
            viewModel.PerformNavigationSearch();
        }

        private void HandleNavigationListMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            viewModel.NavigationItemChosen();
        }

        // IMainWindowAccess implementation -----------------------------------

        void IMainWindowAccess.ShowNavigationPopup()
        {
            ShowNavigationPopup();
        }

        void IMainWindowAccess.HideNavigationPopup()
        {
            HideNavigationPopup();
        }

        void IMainWindowAccess.EnsureSelectedNavigationItemVisible()
        {
            if (viewModel.SelectedNavigationItem != null)
                lbNavigation.ScrollIntoView(viewModel.SelectedNavigationItem);
        }

        // Public methods -----------------------------------------------------

        public MainWindow()
        {
            InitializeComponent();

            viewModel = Dependencies.Container.Instance.Resolve<MainWindowViewModel>(new ParameterOverride("access", this));
            DataContext = viewModel;

            navigationTimer = new Lazy<DispatcherTimer>(() => new DispatcherTimer(TimeSpan.FromMilliseconds(500), DispatcherPriority.Normal, NavigationSearch, this.Dispatcher));
        }
    }
}
