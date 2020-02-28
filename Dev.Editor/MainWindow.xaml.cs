using Dev.Editor.BusinessLogic.Types.Main;
using Dev.Editor.BusinessLogic.Types.UI;
using Dev.Editor.BusinessLogic.ViewModels.Document;
using Dev.Editor.BusinessLogic.ViewModels.Main;
using Dev.Editor.BusinessLogic.ViewModels.Search;
using Dev.Editor.Converters;
using Dev.Editor.Models;
using Dev.Editor.Services;
using Dev.Editor.Services.SingleInstance;
using Dev.Editor.Services.WinAPI;
using Dev.Editor.Wpf;
using Fluent;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
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
        // Private classes ----------------------------------------------------

        private class DragData
        {
            public DragData(BaseDocumentViewModel documentViewModel)
            {
                DocumentViewModel = documentViewModel;
            }

            public BaseDocumentViewModel DocumentViewModel { get; }
        }

        // Private fields -----------------------------------------------------

        private readonly WinAPIService winAPIService;
        private readonly SingleInstanceService singleInstanceService;

        private WindowInteropHelper interopHelper;

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
                    Converter = new DoubleToPixelGridLengthConverter()
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
                    Converter = new DoubleToPixelGridLengthConverter()
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
                    Converter = new DoubleToPixelGridLengthConverter()
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
            interopHelper = new WindowInteropHelper(this);
            singleInstanceService.StoreMainWindowHandle(interopHelper.Handle);
            HwndSource.FromHwnd(interopHelper.Handle).AddHook(HandleWindowMessage);

            viewModel.PropertyChanged += HandleViewModelPropertyChanged;
            SetupSidePanel();
            SetupBottomPanel();
            SetupSecondaryDocumentTabArea();

            viewModel.NotifyLoaded();
        }

        private IntPtr HandleWindowMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WinAPIService.WM_COPYDATA)
            {
                (bool result, string data) = winAPIService.ProcessCopyData(lParam);
                if (result)
                {
                    var args = JsonConvert.DeserializeObject<ArgumentInfo>(data);
                    viewModel.NotifyArgsReceived(args.Args);
                }

                handled = true;
            }

            return IntPtr.Zero;
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
            else if (e.PropertyName == nameof(MainWindowViewModel.ShowSecondaryDocumentTab))
            {
                SetupSecondaryDocumentTabArea();
            }
            else if (e.PropertyName == nameof(MainWindowViewModel.ActiveDocumentTab))
            {
                if (viewModel.ActiveDocumentTab == DocumentTabKind.Primary && tceSecondary.IsFocused)
                    tcePrimary.Focus();
                else if (viewModel.ActiveDocumentTab == DocumentTabKind.Secondary && tcePrimary.IsFocused)
                    tceSecondary.Focus();
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

        private void HandleWindowActivated(object sender, EventArgs e)
        {
            viewModel.NotifyActivated();
        }

        private void DocumentTabItemPreviewMouseMove(object sender, MouseEventArgs e)
        {            
            if (e.LeftButton == MouseButtonState.Pressed && !(e.OriginalSource is System.Windows.Controls.Button))
            {
                var stackPanel = (StackPanel)sender;

                var data = stackPanel.DataContext as BaseDocumentViewModel;
                DragDrop.DoDragDrop(sender as DependencyObject, new DragData(data), DragDropEffects.Move);
            }
        }

        private void DocumentTabItemDrop(object sender, DragEventArgs e)
        {
            var stackPanel = (StackPanel)sender;

            var dragData = (DragData)e.Data.GetData(typeof(DragData));
            if (dragData != null)
            {
                var data = stackPanel.DataContext;
                viewModel.ReorderDocument(dragData.DocumentViewModel, data as BaseDocumentViewModel);
            }
        }

        private void TabHeaderDrop(object sender, DragEventArgs e)
        {
            // Handle only direct drops
            if (e.OriginalSource == sender)
            {
                var dragData = (DragData)e.Data.GetData(typeof(DragData));
                if (dragData != null)
                {
                    var scrollViewer = (ScrollViewer)sender;
                    var itemsSource = (ObservableCollection<BaseDocumentViewModel>)(scrollViewer.GetValue(ParentData.ParentItemsSourceProperty));

                    viewModel.MoveDocumentTo(dragData.DocumentViewModel, itemsSource);
                }
            }
        }

        private void PrimaryDocumentTabGotFocus(object sender, RoutedEventArgs e)
        {
            if (viewModel.ActiveDocumentTab != DocumentTabKind.Primary)
                viewModel.ActiveDocumentTab = DocumentTabKind.Primary;
        }

        private void SecondaryDocumentTabGotFocus(object sender, RoutedEventArgs e)
        {
            if (viewModel.ActiveDocumentTab != DocumentTabKind.Secondary)
                viewModel.ActiveDocumentTab = DocumentTabKind.Secondary;
        }

        private void SetupSecondaryDocumentTabArea()
        {
            if (viewModel.ShowSecondaryDocumentTab)
            {
                cdPrimary.MinWidth = 50;
                cdSecondary.MinWidth = 50;
                cdPrimary.Width = new GridLength(1, GridUnitType.Star);
                cdSecondary.Width = new GridLength(1, GridUnitType.Star);
            }
            else
            {
                cdPrimary.MinWidth = 0;
                cdSecondary.MinWidth = 0;
                cdPrimary.Width = new GridLength(1, GridUnitType.Star);
                cdSecondary.Width = new GridLength(0, GridUnitType.Auto);
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

        void IMainWindowAccess.SetWindowSize(Size size)
        {
            this.Width = size.Width;
            this.Height = size.Height;
        }

        void IMainWindowAccess.SetWindowLocation(Point point)
        {
            this.Left = point.X;
            this.Top = point.Y;
        }

        void IMainWindowAccess.SetWindowMaximized(bool maximized)
        {
            this.WindowState = maximized ? WindowState.Maximized : WindowState.Normal;
        }

        void IMainWindowAccess.BringToFront()
        {
            // Restore if needed
            if (this.WindowState == WindowState.Minimized)
                this.WindowState = WindowState.Normal;

            // Focus
            this.Activate();
        }

        // Public methods -----------------------------------------------------

        public MainWindow()
        {
            InitializeComponent();

            winAPIService = Dependencies.Container.Instance.Resolve<WinAPIService>();
            singleInstanceService = Dependencies.Container.Instance.Resolve<SingleInstanceService>();

            viewModel = Dependencies.Container.Instance.Resolve<MainWindowViewModel>(new ParameterOverride("access", this));
            DataContext = viewModel;

            navigationTimer = new Lazy<DispatcherTimer>(() => new DispatcherTimer(TimeSpan.FromMilliseconds(500), DispatcherPriority.Normal, NavigationSearch, this.Dispatcher));
        }
    }
}
