using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using static ModernFlyouts.Core.Interop.NativeMethods;

namespace ModernFlyouts.Core.Interop
{
    public enum ZBandID : int
    {
        Default = 0x0,
        Desktop = 0x1,
        UIAccess = 0x2,
        ImmersiveIHM = 0x3,
        ImmersiveNotification = 0x4,
        ImmersiveAppChrome = 0x5,
        ImmersiveMogo = 0x6,
        ImmersiveEDGY = 0x7,
        ImmersiveInActiveMOBODY = 0x8,
        ImmersiveInActiveDock = 0x9,
        ImmersiveActiveMOBODY = 0xA,
        ImmersiveActiveDock = 0xB,
        ImmersiveBackground = 0xC,
        ImmersiveSEARCH = 0xD,
        GenuineWindows = 0xE,
        ImmersiveRestricted = 0xF,
        SystemTools = 0x10,
        Lock = 0x11,
        AboveLockUX = 0x12,
    };

    public class BandWindow : DependencyObject
    {
        private delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        private readonly WndProc delegWndProc;

        private HwndSource hwndSource;

        private double dpiScale = 1.0;

        #region Properties

        protected double DpiScale => dpiScale;

        protected HwndSource HwndSource => hwndSource;

        #region Attached DPs

        public static readonly DependencyProperty BandWindowProperty = DependencyProperty.Register(
            "BandWindow",
            typeof(BandWindow),
            typeof(BandWindow),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

        public static BandWindow GetBandWindow(FrameworkElement element)
        {
            return (BandWindow)element.GetValue(BandWindowProperty);
        }

        public static void SetBandWindow(FrameworkElement element, BandWindow bandWindow)
        {
            element.SetValue(BandWindowProperty, bandWindow);
        }

        #endregion

        #region Activatable

        public static readonly DependencyProperty ActivatableProperty = DependencyProperty.Register(
            nameof(Activatable),
            typeof(bool),
            typeof(BandWindow),
            new PropertyMetadata(false, OnActivatablePropertyChanged));

        public bool Activatable
        {
            get => (bool)GetValue(ActivatableProperty);
            set => SetValue(ActivatableProperty, value);
        }

        private static void OnActivatablePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BandWindow bandWindow)
            {
                if (bandWindow.IsLoaded)
                {
                    if ((bool)e.NewValue)
                    {
                        ApplyWindowStyles(bandWindow.Handle, wsEXToRemove: ExtendedWindowStyles.WS_EX_NOACTIVATE);
                    }
                    else
                    {
                        ApplyWindowStyles(bandWindow.Handle, wsEXToAdd: ExtendedWindowStyles.WS_EX_NOACTIVATE);
                    }
                }
            }
        }

        #endregion

        #region ActualWidth

        private static readonly DependencyPropertyKey ActualWidthPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(ActualWidth),
            typeof(double),
            typeof(BandWindow),
            new PropertyMetadata(0.0));

        public static readonly DependencyProperty ActualWidthProperty = ActualWidthPropertyKey.DependencyProperty;

        public double ActualWidth
        {
            get => (double)GetValue(ActualWidthProperty);
            private set => SetValue(ActualWidthPropertyKey, value);
        }

        #endregion

        #region ActualHeight

        private static readonly DependencyPropertyKey ActualHeightPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(ActualHeight),
            typeof(double),
            typeof(BandWindow),
            new PropertyMetadata(0.0));

        public static readonly DependencyProperty ActualHeightProperty = ActualHeightPropertyKey.DependencyProperty;

        public double ActualHeight
        {
            get => (double)GetValue(ActualHeightProperty);
            private set => SetValue(ActualHeightPropertyKey, value);
        }

        #endregion

        #region ActualLeft

        private static readonly DependencyPropertyKey ActualLeftPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(ActualLeft),
            typeof(double),
            typeof(BandWindow),
            new PropertyMetadata(0.0));

        public static readonly DependencyProperty ActualLeftProperty = ActualLeftPropertyKey.DependencyProperty;

        public double ActualLeft
        {
            get => (double)GetValue(ActualLeftProperty);
            private set => SetValue(ActualLeftPropertyKey, value);
        }

        #endregion

        #region ActualTop

        private static readonly DependencyPropertyKey ActualTopPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(ActualTop),
            typeof(double),
            typeof(BandWindow),
            new PropertyMetadata(0.0));

        public static readonly DependencyProperty ActualTopProperty = ActualTopPropertyKey.DependencyProperty;

        public double ActualTop
        {
            get => (double)GetValue(ActualTopProperty);
            private set => SetValue(ActualTopPropertyKey, value);
        }

        #endregion

        #region Content

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
            nameof(Content),
            typeof(FrameworkElement),
            typeof(BandWindow),
            new PropertyMetadata(null, OnContentPropertyChanged));

        public FrameworkElement Content
        {
            get => (FrameworkElement)GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        private static void OnContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BandWindow bandWindow)
            {
                if (e.OldValue is FrameworkElement oldContent)
                {
                    oldContent.ClearValue(BandWindowProperty);
                    oldContent.SizeChanged -= OnContentSizeChanged;
                }
                if (e.NewValue is FrameworkElement newContent)
                {
                    newContent.SizeChanged += OnContentSizeChanged;
                    newContent.SetValue(BandWindowProperty, bandWindow);

                    if (bandWindow.hwndSource != null)
                    {
                        bandWindow.hwndSource.RootVisual = newContent;
                    }
                }
            }
        }

        private static void OnContentSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                var bandWindow = GetBandWindow(element);
                if (bandWindow.IsLoaded)
                {
                    bandWindow.UpdateSize();
                }
            }
        }

        #endregion

        #region Handle

        private static readonly DependencyPropertyKey HandlePropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(Handle),
            typeof(IntPtr),
            typeof(BandWindow),
            new PropertyMetadata(IntPtr.Zero));

        public static readonly DependencyProperty HandleProperty = HandlePropertyKey.DependencyProperty;

        public IntPtr Handle
        {
            get => (IntPtr)GetValue(HandleProperty);
            private set => SetValue(HandlePropertyKey, value);
        }

        #endregion

        #region IsActive

        private static readonly DependencyPropertyKey IsActivePropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(IsActive),
            typeof(bool),
            typeof(BandWindow),
            new PropertyMetadata(false));

        public static readonly DependencyProperty IsActiveProperty = IsActivePropertyKey.DependencyProperty;

        public bool IsActive
        {
            get => (bool)GetValue(IsActiveProperty);
            private set => SetValue(IsActivePropertyKey, value);
        }

        #endregion

        #region IsLoaded

        private static readonly DependencyPropertyKey IsLoadedPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(IsLoaded),
            typeof(bool),
            typeof(BandWindow),
            new PropertyMetadata(false));

        public static readonly DependencyProperty IsLoadedProperty = IsLoadedPropertyKey.DependencyProperty;

        public bool IsLoaded
        {
            get => (bool)GetValue(IsLoadedProperty);
            private set => SetValue(IsLoadedPropertyKey, value);
        }

        #endregion

        #region IsVisible

        private static readonly DependencyPropertyKey IsVisiblePropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(IsVisible),
            typeof(bool),
            typeof(BandWindow),
            new PropertyMetadata(false));

        public static readonly DependencyProperty IsVisibleProperty = IsVisiblePropertyKey.DependencyProperty;

        public bool IsVisible
        {
            get => (bool)GetValue(IsVisibleProperty);
            private set => SetValue(IsVisiblePropertyKey, value);
        }

        #endregion

        #region TopMost

        public static readonly DependencyProperty TopMostProperty = DependencyProperty.Register(
            nameof(TopMost),
            typeof(bool),
            typeof(BandWindow),
            new PropertyMetadata(true, OnTopMostPropertyChanged));

        public bool TopMost
        {
            get => (bool)GetValue(TopMostProperty);
            set => SetValue(TopMostProperty, value);
        }

        private static void OnTopMostPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BandWindow bandWindow)
            {
                if (bandWindow.IsLoaded)
                {
                    if ((bool)e.NewValue)
                    {
                        ApplyWindowStyles(bandWindow.Handle, wsEXToAdd: ExtendedWindowStyles.WS_EX_TOPMOST);
                        ShowWindow(bandWindow.Handle, (int)ShowWindowCommands.ShowNoActivate);
                    }
                    else
                    {
                        ApplyWindowStyles(bandWindow.Handle, wsEXToRemove: ExtendedWindowStyles.WS_EX_TOPMOST);
                    }
                }
            }
        }

        #endregion

        #region ZBandID

        public static readonly DependencyProperty ZBandIDProperty = DependencyProperty.Register(
            nameof(ZBandID),
            typeof(ZBandID),
            typeof(BandWindow),
            new PropertyMetadata(ZBandID.Default, OnZBandIDPropertyChanged));

        public ZBandID ZBandID
        {
            get => (ZBandID)GetValue(ZBandIDProperty);
            set => SetValue(ZBandIDProperty, value);
        }

        private static void OnZBandIDPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BandWindow bandWindow && bandWindow.IsLoaded)
                throw new AccessViolationException("ZBandID should not be changed after the window creation");
        }

        #endregion

        #endregion

        public BandWindow()
        {
            delegWndProc = myWndProc;
        }

        #region Actual window handling

        public void CreateWindow()
        {
            if (IsLoaded)
                return;

            WNDCLASSEX wind_class = new()
            {
                cbSize = Marshal.SizeOf(typeof(WNDCLASSEX)),
                hbrBackground = (IntPtr)1 + 1,
                cbClsExtra = 0,
                cbWndExtra = 0,
                hInstance = Process.GetCurrentProcess().Handle,
                hIcon = IntPtr.Zero,
                lpszMenuName = string.Empty,
                lpszClassName = "WIB_" + Guid.NewGuid(),
                lpfnWndProc = Marshal.GetFunctionPointerForDelegate(delegWndProc),
                hIconSm = IntPtr.Zero
            };
            ushort regResult = RegisterClassEx(ref wind_class);

            if (regResult == 0)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            IntPtr hWnd = CreateWindowInBand(
                    (int)(ExtendedWindowStyles.WS_EX_TOOLWINDOW
                    | (Activatable ? 0 : ExtendedWindowStyles.WS_EX_NOACTIVATE)
                    | (TopMost ? ExtendedWindowStyles.WS_EX_TOPMOST : 0)),
                    regResult,
                    string.Empty,
                    (uint)WindowStyles.WS_POPUP & ~(uint)WindowStyles.WS_SYSMENU,
                    0,
                    0,
                    0,
                    0,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    wind_class.hInstance,
                    IntPtr.Zero,
                    (int)ZBandID);

            if (hWnd == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            Handle = hWnd;

            WindowCompositionHelper.MakeWindowTransparent(hWnd);

            HwndSourceParameters param = new()
            {
                WindowStyle = 0x10000000 | 0x40000000,
                ParentWindow = hWnd,
                UsesPerPixelOpacity = true
            };

            hwndSource = new(param)
            {
                RootVisual = Content
            };

            hwndSource.CompositionTarget.BackgroundColor = Colors.Transparent;
            hwndSource.SizeToContent = SizeToContent.WidthAndHeight;
            hwndSource.DpiChanged += HwndSource_DpiChanged;
            hwndSource.ContentRendered += HwndSource_ContentRendered;

            UpdateWindow(hWnd);

            IsLoaded = true;
        }

        private void HwndSource_DpiChanged(object sender, HwndDpiChangedEventArgs e)
        {
            OnDpiChanged(e.NewDpi.DpiScaleX);
            UpdateSize();
        }

        private void HwndSource_ContentRendered(object sender, EventArgs e)
        {
            OnDpiChanged(GetDpiForWindow(Handle) / 96.0);
            UpdateSize();
        }

        private IntPtr myWndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            switch (msg)
            {
                case WM_ACTIVATE:
                    HandleWindowActivation(wParam);
                    break;

                case WM_EXITSIZEMOVE:
                    OnDragMoved();
                    break;

                case WM_DESTROY:
                    DestroyWindow(hWnd);
                    break;

                case WM_DPICHANGED:
                    OnDpiChanged((wParam.ToInt32() & 0xFFFF) / 96.0);
                    UpdateSize();
                    break;

                default:
                    break;
            }

            return DefWindowProc(hWnd, msg, wParam, lParam);
        }

        private void HandleWindowActivation(IntPtr wParam)
        {
            bool isActive = wParam.ToInt32() != 0;
            IsActive = isActive;

            if (isActive)
            {
                OnActivated();
            }
            else
            {
                OnDeactivated();
            }
        }

        private void UpdateSize()
        {
            var width = Content?.ActualWidth * dpiScale ?? 0.0;
            var height = Content?.ActualHeight * dpiScale ?? 0.0;
            SetWindowPos(Handle, IntPtr.Zero, 0, 0,
                (int)Math.Round(width),
                (int)Math.Round(height),
                SWP.NOZORDER | SWP.NOMOVE);

            UpdateWindow(Handle);

            ActualWidth = width;
            ActualHeight = height;

            OnSizeChanged();
        }

        #endregion

        #region Protected Methods

        protected void SetPosition(double x, double y)
        {
            if (!IsLoaded)
                return;

            SetWindowPos(Handle, IntPtr.Zero,
                (int)Math.Round(x),
                (int)Math.Round(y),
                0, 0, SWP.NOZORDER | SWP.NOSIZE);

            UpdateWindow(Handle);

            ActualLeft = x;
            ActualTop = y;
        }

        protected virtual void OnActivated()
        {
            Activated?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnDeactivated()
        {
            Deactivated?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnDpiChanged(double newDpiScale)
        {
            dpiScale = newDpiScale;
            DpiChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnIsVisibleChanged()
        {
            IsVisibleChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnDragMoved()
        {
            GetWindowRect(Handle, out var rect);
            ActualLeft = rect.Left;
            ActualTop = rect.Top;

            DragMoved?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnSizeChanged()
        {
            SizeChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Public Methods

        public void DragMove()
        {
            if (!IsLoaded)
                return;

            SendMessage(Handle, WM_SYSCOMMAND, SC_MOUSEMOVE, 0);
            SendMessage(Handle, WM_LBUTTONUP, 0, 0);
        }

        public void Activate()
        {
            if (!IsLoaded)
                return;

            SetForegroundWindow(Handle);
        }

        public void Show()
        {
            if (!IsLoaded)
            {
                CreateWindow();
            }

            ShowWindowAsync(Handle, Activatable ?
                (int)ShowWindowCommands.ShowNoActivate : (int)ShowWindowCommands.ShowDefault);

            if (Activatable) SetForegroundWindow(Handle);

            IsVisible = true;
            OnIsVisibleChanged();
        }

        public void Hide()
        {
            if (!IsLoaded)
                return;

            ShowWindowAsync(Handle, (int)ShowWindowCommands.Hide);
            IsVisible = false;
            OnIsVisibleChanged();
        }

        #endregion

        public event EventHandler Activated;

        public event EventHandler Deactivated;

        public event EventHandler DpiChanged;

        public event EventHandler IsVisibleChanged;

        public event EventHandler DragMoved;

        public event EventHandler SizeChanged;
    }
}
