using Dev.Editor.BusinessLogic.Types.Main;
using Dev.Editor.BusinessLogic.Types.UI;
using Dev.Editor.BusinessLogic.ViewModels.Document;
using Dev.Editor.BusinessLogic.ViewModels.Main;
using Dev.Editor.BusinessLogic.ViewModels.Main.Documents;
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

        private bool handlingActivated = false;

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
            viewModel.Documents.PropertyChanged += HandleDocumentsManagerPropertyChanged;
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
        }

        private void HandleDocumentsManagerPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DocumentsManager.ShowSecondaryDocumentTab))
            {
                SetupSecondaryDocumentTabArea();
            }
            else if (e.PropertyName == nameof(DocumentsManager.ActiveDocumentTab))
            {
                if (viewModel.Documents.ActiveDocumentTab == DocumentTabKind.Primary)
                {
                    tcePrimary.FocusContent();
                }
                else if (viewModel.Documents.ActiveDocumentTab == DocumentTabKind.Secondary)
                {
                    tceSecondary.FocusContent();
                }
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
            // It may happen, that WindowActivated will be called during processing
            // of WindowActivated. Since we're using BeginInvoke, additional 
            // measures must be taken to prevent nested calls - until the first one
            // is processed properly.

            if (!handlingActivated)
            {
                // This event is called too before Loaded event, what may cause problems
                // on application startup. Deferring call until Loaded has been executed.
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    handlingActivated = true;

                    try
                    {
                        viewModel.NotifyActivated();
                    }
                    finally
                    {
                        handlingActivated = false;
                    }
                }), DispatcherPriority.ContextIdle);
            }
        }

        private void DocumentTabItemPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                var element = (FrameworkElement)sender;

                var data = element.DataContext as BaseDocumentViewModel;
                viewModel.RequestCloseDocument(data);

                e.Handled = true;
            }
        }


        private void DocumentTabItemPreviewMouseMove(object sender, MouseEventArgs e)
        {            
            if (e.LeftButton == MouseButtonState.Pressed && !(e.OriginalSource is System.Windows.Controls.Button) && !(e.OriginalSource is System.Windows.Controls.Primitives.ToggleButton))
            {
                var element = (FrameworkElement)sender;

                var data = element.DataContext as BaseDocumentViewModel;
                DragDrop.DoDragDrop(sender as DependencyObject, new DragData(data), DragDropEffects.Move);
            }
        }

        private void DocumentTabItemDrop(object sender, DragEventArgs e)
        {
            var element = (FrameworkElement)sender;

            var dragData = (DragData)e.Data.GetData(typeof(DragData));
            if (dragData != null)
            {
                var data = element.DataContext;
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
                    var itemsSource = (ITabDocumentCollection<BaseDocumentViewModel>)scrollViewer.GetValue(ParentData.ParentItemsSourceProperty);

                    viewModel.MoveDocumentTo(dragData.DocumentViewModel, itemsSource);
                }
            }
        }

        private void PrimaryDocumentTabGotFocus(object sender, RoutedEventArgs e)
        {
            if (viewModel.Documents.ActiveDocumentTab != DocumentTabKind.Primary)
                viewModel.Documents.ActiveDocumentTab = DocumentTabKind.Primary;
        }

        private void SecondaryDocumentTabGotFocus(object sender, RoutedEventArgs e)
        {
            if (viewModel.Documents.ActiveDocumentTab != DocumentTabKind.Secondary)
                viewModel.Documents.ActiveDocumentTab = DocumentTabKind.Secondary;
        }

        private void SetupSecondaryDocumentTabArea()
        {
            if (viewModel.Documents.ShowSecondaryDocumentTab)
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
            const int MIN_PIXELS_ON_SCREEN = 64;

            this.Left = point.X;
            this.Top = point.Y;

            // Ensure, that window is visible

            var converter = new ScreenBoundsConverter(this);

            foreach (var screen in System.Windows.Forms.Screen.AllScreens)
            {
                Rect bounds = converter.ConvertBounds(screen.Bounds);

                double commonX = Math.Min(bounds.Right, this.Left + this.Width) - Math.Max(bounds.Left, this.Left);
                double commonY = Math.Min(bounds.Bottom, this.Top + this.Height) - Math.Max(bounds.Top, this.Top);

                if (commonX > MIN_PIXELS_ON_SCREEN && commonY > MIN_PIXELS_ON_SCREEN)
                {
                    return;
                }
            }

            var mainScreen = System.Windows.Forms.Screen.PrimaryScreen;
            Rect mainBounds = converter.ConvertBounds(mainScreen.Bounds);

            Left = mainBounds.Left;
            Top = mainBounds.Top;
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

        void IMainWindowAccess.WhenUIReady(Action action)
        {
            Dispatcher.BeginInvoke(action, DispatcherPriority.Render);
        }

        // Public methods -----------------------------------------------------

        // private DispatcherTimer tmp;

        public MainWindow()
        {
            InitializeComponent();

            winAPIService = Dependencies.Container.Instance.Resolve<WinAPIService>();
            singleInstanceService = Dependencies.Container.Instance.Resolve<SingleInstanceService>();

            viewModel = Dependencies.Container.Instance.Resolve<MainWindowViewModel>(new ParameterOverride("access", this));
            DataContext = viewModel;

            navigationTimer = new Lazy<DispatcherTimer>(() => new DispatcherTimer(TimeSpan.FromMilliseconds(500), DispatcherPriority.Normal, NavigationSearch, this.Dispatcher));

            // tmp = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Normal, new EventHandler((s, a) => {
            //     var focusedElement = FocusManager.GetFocusedElement(this);
            //     System.Diagnostics.Debug.WriteLine($"Focused element: {(focusedElement == null ? "null" : focusedElement.ToString())}");
            // }), Dispatcher);
        }
    }
}
