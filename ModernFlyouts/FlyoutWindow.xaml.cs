using System;
using System.Windows;
using System.Windows.Threading;

namespace ModernFlyouts
{
    public partial class FlyoutWindow : Window
    {
        private DispatcherTimer _elapsedTimer;

        #region Properties

        public static readonly DependencyProperty VisibleProperty = DependencyProperty.Register("Visible", typeof(bool), typeof(FlyoutWindow), new PropertyMetadata(false, OnVisiblePropertyChanged));

        private static void OnVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var vlFly = d as FlyoutWindow;

            if ((bool)e.NewValue) { vlFly.ShowFlyout(); }
            else { vlFly.HideFlyout(); }
        }

        public bool Visible
        {
            get => (bool)GetValue(VisibleProperty);
            set => SetValue(VisibleProperty, value);
        }

        #endregion

        #region Events

        // Showing Flyout
        public static readonly RoutedEvent FlyoutShownEvent = EventManager.RegisterRoutedEvent(nameof(FlyoutShown), RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(FlyoutWindow));

        public event RoutedEventHandler FlyoutShown
        {
            add { AddHandler(FlyoutShownEvent, value); }
            remove { RemoveHandler(FlyoutShownEvent, value); }
        }

        private void ShowFlyout()
        {
            Show();
            _elapsedTimer.Stop();
            RoutedEventArgs args = new RoutedEventArgs(FlyoutShownEvent);
            RaiseEvent(args);
            _elapsedTimer.Start();
        }

        // Hiding Flyout
        public static readonly RoutedEvent FlyoutHiddenEvent = EventManager.RegisterRoutedEvent(nameof(FlyoutHidden), RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(FlyoutWindow));

        public event RoutedEventHandler FlyoutHidden
        {
            add { AddHandler(FlyoutHiddenEvent, value); }
            remove { RemoveHandler(FlyoutHiddenEvent, value); }
        }

        private void HideFlyout()
        {
            RoutedEventArgs args = new RoutedEventArgs(FlyoutHiddenEvent);
            RaiseEvent(args);
        }

        #endregion

        public FlyoutWindow()
        {
            InitializeComponent();
            
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) { return; }

            AlignFlyout();
            MovableAreaBorder.MouseLeftButtonDown += (_, __) => DragMove();
            HideFlyoutButton.Click += (_, __) => Visible = false;
            AlignButton.Click += (_, __) => AlignFlyout();
            OtherPreps();
            SetUpAnimPreps();
        }

        private void SetupHideTimer()
        {
            _elapsedTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2.75) };

            _elapsedTimer.Tick += (_, __) =>
            {
                _elapsedTimer.Stop();
                if (!IsMouseOver)
                {
                    Visible = false;
                }
            };
        }

        public void StopHideTimer()
        {
            _elapsedTimer.Stop();
        }

        public void StartHideTimer()
        {
           if (!_elapsedTimer.IsEnabled) { _elapsedTimer.Start(); }
        }

        private void OtherPreps()
        {
            SetupHideTimer();
            MouseEnter += (_, __) => _elapsedTimer.Stop();
            MouseLeave += (_, __) => _elapsedTimer.Start();
            PreviewMouseDown += (_, __) => _elapsedTimer.Stop();
            PreviewStylusDown += (_, __) => _elapsedTimer.Stop();
            PreviewMouseUp += (_, __) => { _elapsedTimer.Stop(); _elapsedTimer.Start(); };
            PreviewStylusUp += (_, __) => { _elapsedTimer.Stop(); _elapsedTimer.Start(); };
        }

        internal void SetUpAnimPreps()
        {
            T1.Y = -40;
            T2.Y = 40;
            MainBorder.Opacity = 0;
            SecondaryHost.Opacity = 0;
        }

        private Point DefaultPosition;

        private void AlignFlyout()
        {
            if (DefaultPosition == default)
            {
                DefaultPosition = DUIHandler.GetCoordinates();
            }

            Left = DefaultPosition.X; Top = DefaultPosition.Y;
        }

        #region Tray Menu Methods

        private void exitItem_Click(object sender, RoutedEventArgs e)
        {
            FlyoutHandler.SafelyExitApplication();
        }

        private void settingsItem_Click(object sender, RoutedEventArgs e)
        {
            FlyoutHandler.ShowSettingsWindow();
        }

        #endregion
    }
}