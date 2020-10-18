using ModernFlyouts.Utilities;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace ModernFlyouts
{
    public partial class FlyoutWindow : Window
    {
        private DispatcherTimer _elapsedTimer;

        #region Properties

        public static readonly DependencyProperty VisibleProperty =
            DependencyProperty.Register(
                nameof(Visible),
                typeof(bool),
                typeof(FlyoutWindow),
                new PropertyMetadata(false, OnVisiblePropertyChanged));

        public bool Visible
        {
            get => (bool)GetValue(VisibleProperty);
            set => SetValue(VisibleProperty, value);
        }

        private static void OnVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var flyout = d as FlyoutWindow;

            if ((bool)e.NewValue)
            {
                flyout.ShowFlyout();
            }
            else
            {
                flyout.HideFlyout();
            }
        }

        public static readonly DependencyProperty FlyoutHelperProperty =
            DependencyProperty.Register(
                nameof(FlyoutHelper),
                typeof(FlyoutHelperBase),
                typeof(FlyoutWindow),
                new PropertyMetadata(null));


        public FlyoutHelperBase FlyoutHelper
        {
            get => (FlyoutHelperBase)GetValue(FlyoutHelperProperty);
            set => SetValue(FlyoutHelperProperty, value);
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

        public static readonly RoutedEvent FlyoutTimedHidingEvent = EventManager.RegisterRoutedEvent(nameof(FlyoutTimedHiding), RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(FlyoutWindow));

        public event RoutedEventHandler FlyoutTimedHiding
        {
            add { AddHandler(FlyoutTimedHidingEvent, value); }
            remove { RemoveHandler(FlyoutTimedHidingEvent, value); }
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

            DataContext = this;

            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) { return; }

            this.IsVisibleChanged += (s, e) =>
            {
                if (!(bool)e.NewValue)
                {
                    FlyoutHelper = null;
                }
            };

            AlignFlyout(false);
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

                RoutedEventArgs args = new RoutedEventArgs(FlyoutTimedHidingEvent);
                RaiseEvent(args);

                if (!IsMouseOver && !args.Handled)
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

            MouseEnter += (_, e) =>
            {
                _elapsedTimer.Stop();

                if (!_topBarOverlay && !FlyoutHandler.Instance.TopBarEnabled && IsMousePointerWithinBounds(TopBarGrid, e))
                {
                    _topBarOverlay = true;
                    UpdateTopBar(true);
                }
            };
            MouseLeave += (_, e) =>
            {
                _elapsedTimer.Start();

                if (_topBarOverlay && !FlyoutHandler.Instance.TopBarEnabled)
                {
                    _topBarOverlay = false;
                    UpdateTopBar(false);
                }
            };
            MouseMove += (_, e) =>
            {
                if (_topBarOverlay && !FlyoutHandler.Instance.TopBarEnabled && !IsMousePointerWithinBounds(TopBarGrid, e))
                {
                    _topBarOverlay = false;
                    UpdateTopBar(false);
                }

                if (!_topBarOverlay && !FlyoutHandler.Instance.TopBarEnabled && IsMousePointerWithinBounds(TopBarGrid, e))
                {
                    _topBarOverlay = true;
                    UpdateTopBar(true);
                }
            };

            PreviewMouseDown += (_, __) => _elapsedTimer.Stop();
            PreviewStylusDown += (_, __) => _elapsedTimer.Stop();
            PreviewMouseUp += (_, __) => { _elapsedTimer.Stop(); _elapsedTimer.Start(); };
            PreviewStylusUp += (_, __) => { _elapsedTimer.Stop(); _elapsedTimer.Start(); };
        }

        internal void SetUpAnimPreps()
        {
            T1.Y = -40;
            ContentGrid.Opacity = 0;
        }

        private void AlignFlyout(bool toDefault = true)
        {
            var defaultPosition = toDefault ? FlyoutHandler.Instance.DefaultFlyoutPosition : AppDataHelper.FlyoutPosition;

            Left = defaultPosition.X - leftShadowMargin; Top = defaultPosition.Y;

            if (toDefault)
            {
                SaveFlyoutPosition();
            }
        }

        public void SaveFlyoutPosition()
        {
            AppDataHelper.FlyoutPosition = new Point(Left + leftShadowMargin, Top);
        }

        public void OnTopBarEnabledChanged(bool value)
        {
            TopBarPinButtonIcon.Glyph = value ? CommonGlyphs.UnPin : CommonGlyphs.Pin;
            TopBarPinButton.ToolTip = value ? Properties.Strings.UnpinTopBar : Properties.Strings.PinTopBar;
            _topBarOverlay = false;
            var pos = Mouse.GetPosition(TopBarGrid);
            if (pos.Y > 0 && pos.Y < 32)
            {
                _topBarOverlay = true;
                value = true;
            }

            UpdateTopBar(value);
        }

        private void UpdateTopBar(bool showTopBar)
        {
            var R1 = this.TopBarGrid.RowDefinitions[0];
            var R2 = this.TopBarGrid.RowDefinitions[1];
            if (showTopBar)
            {
                this.TopBarGrid.Margin = new Thickness(0);
                R2.Height = new GridLength(0);
                var glAnim = new GridLengthAnimation()
                {
                    From = R1.Height,
                    To = new GridLength(32),
                    Duration = TimeSpan.FromMilliseconds(167),
                    EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut }
                };
                R1.BeginAnimation(RowDefinition.HeightProperty, glAnim);
            }
            else
            {
                this.TopBarGrid.Margin = new Thickness(0, 0, 0, -10);
                R2.Height = new GridLength(10);
                var glAnim = new GridLengthAnimation()
                {
                    From = R1.Height,
                    To = new GridLength(0),
                    Duration = TimeSpan.FromMilliseconds(167),
                    EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut }
                };
                R1.BeginAnimation(RowDefinition.HeightProperty, glAnim);
            }
        }

        private void TopBarPinButton_Click(object sender, RoutedEventArgs e)
        {
            FlyoutHandler.Instance.TopBarEnabled = !FlyoutHandler.Instance.TopBarEnabled;
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            FlyoutHandler.ShowSettingsWindow();
        }

        private static bool IsMousePointerWithinBounds(FrameworkElement frameworkElement, MouseEventArgs e)
        {
            var point = e.GetPosition(frameworkElement);
            return point.X >= 0 && point.Y >= 0 && point.X <= frameworkElement.ActualWidth && point.Y <= frameworkElement.ActualHeight;
        }

        private int leftShadowMargin = 20;
        private bool _topBarOverlay = false;
    }
}