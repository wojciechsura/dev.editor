using Dev.Editor.BusinessLogic.Types.UI;
using Dev.Editor.BusinessLogic.ViewModels.Document;
using Dev.Editor.BusinessLogic.ViewModels.Main;
using Dev.Editor.BusinessLogic.ViewModels.Search;
using Dev.Editor.Converters;
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

        private void SetupSidePanel()
        {
            if (viewModel.SidePanelPlacement == SidePanelPlacement.Left)
            {
                BindingOperations.SetBinding(leftPanelColumn, ColumnDefinition.WidthProperty, new Binding
                {
                    Path = new PropertyPath(nameof(MainWindowViewModel.SidePanelSize)),
                    Mode = BindingMode.TwoWay,
                    Converter = new DoubleToGridLengthConverter()
                });

                leftPanelColumn.MinWidth = 150.0;
            }
            else
            {
                leftPanelColumn.MinWidth = 0.0;
                BindingOperations.ClearBinding(leftPanelColumn, ColumnDefinition.WidthProperty);
                leftPanelColumn.Width = new GridLength(0.0, GridUnitType.Auto);
            }

            if (viewModel.SidePanelPlacement == SidePanelPlacement.Right)
            {
                BindingOperations.SetBinding(rightPanelColumn, ColumnDefinition.WidthProperty, new Binding
                {
                    Path = new PropertyPath(nameof(MainWindowViewModel.SidePanelSize)),
                    Mode = BindingMode.TwoWay,
                    Converter = new DoubleToGridLengthConverter()
                });
                rightPanelColumn.MinWidth = 150.0;
            }
            else
            {
                rightPanelColumn.MinWidth = 0.0;
                BindingOperations.ClearBinding(rightPanelColumn, ColumnDefinition.WidthProperty);
                rightPanelColumn.Width = new GridLength(0.0, GridUnitType.Auto);
            }
        }

        private void SetupBottomPanel()
        {
            if (viewModel.BottomPanelVisibility == BottomPanelVisibility.Visible)
            {
                BindingOperations.SetBinding(bottomPanelRow, RowDefinition.HeightProperty, new Binding
                {
                    Path = new PropertyPath(nameof(MainWindowViewModel.BottomPanelSize)),
                    Mode = BindingMode.TwoWay,
                    Converter = new DoubleToGridLengthConverter()
                });

                bottomPanelRow.MinHeight = 150.0;
            }
            else
            {
                bottomPanelRow.MinHeight = 0.0;
                BindingOperations.ClearBinding(bottomPanelRow, RowDefinition.HeightProperty);
                bottomPanelRow.Height = new GridLength(0.0, GridUnitType.Auto);
            }
        }

        private void HandleWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            bool canClose = viewModel.CanCloseApplication();
            
            e.Cancel = !canClose;

            if (canClose)
                viewModel.NotifyClosingWindow();
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

        private void HandleLoaded(object sender, RoutedEventArgs e)
        {
            viewModel.PropertyChanged += HandleViewModelPropertyChanged;
            SetupSidePanel();
            SetupBottomPanel();
        }

        private void HandleViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainWindowViewModel.SidePanelPlacement))
            {
                SetupSidePanel();
            }
            else if (e.PropertyName == nameof(MainWindowViewModel.BottomPanelVisibility))
            {
                SetupBottomPanel();
            }
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

        private void HandleDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                viewModel.NotifyFilesDropped(files);                
            }
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

        Size IMainWindowAccess.GetWindowSize()
        {
            return new Size(this.Width, this.Height);
        }

        Point IMainWindowAccess.GetWindowLocation()
        {
            return new Point(this.Left, this.Top);
        }

        bool IMainWindowAccess.GetMaximized()
        {
            return WindowState == WindowState.Maximized;
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
