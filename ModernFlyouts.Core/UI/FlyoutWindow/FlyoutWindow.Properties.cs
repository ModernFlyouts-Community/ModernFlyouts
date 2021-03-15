using ModernFlyouts.Core.Display;
using System.Windows;
using System.Windows.Controls;

namespace ModernFlyouts.Core.UI
{
    public partial class FlyoutWindow
    {
        #region Attached DPs

        public static readonly DependencyProperty FlyoutWindowProperty =
            DependencyProperty.RegisterAttached(
                "FlyoutWindow",
                typeof(FlyoutWindow),
                typeof(FlyoutWindow),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

        public static FlyoutWindow GetFlyoutWindow(DependencyObject obj)
        {
            return (FlyoutWindow)obj?.GetValue(FlyoutWindowProperty);
        }

        public static void SetFlyoutWindow(DependencyObject obj, FlyoutWindow flyoutWindow)
        {
            obj?.SetValue(FlyoutWindowProperty, flyoutWindow);
        }

        #endregion

        #region Alignment

        public static readonly DependencyProperty AlignmentProperty =
            DependencyProperty.Register(
                nameof(Alignment),
                typeof(FlyoutWindowAlignments),
                typeof(FlyoutWindow),
                new PropertyMetadata(FlyoutWindowAlignments.Top | FlyoutWindowAlignments.Left, OnAlignmentPropertyChanged));

        public FlyoutWindowAlignments Alignment
        {
            get => (FlyoutWindowAlignments)GetValue(AlignmentProperty);
            set => SetValue(AlignmentProperty, value);
        }

        private static void OnAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FlyoutWindow flyoutWindow)
            {
                flyoutWindow.PositionFlyout();
            }
        }

        #endregion

        #region ActualExpandDirection

        private static readonly DependencyPropertyKey ActualExpandDirectionPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(ActualExpandDirection),
                typeof(FlyoutWindowExpandDirection),
                typeof(FlyoutWindow),
                new PropertyMetadata(FlyoutWindowExpandDirection.Down, OnActualExpandDirectionPropertyChanged));

        public static readonly DependencyProperty ActualExpandDirectionProperty = ActualExpandDirectionPropertyKey.DependencyProperty;

        public FlyoutWindowExpandDirection ActualExpandDirection
        {
            get => (FlyoutWindowExpandDirection)GetValue(ActualExpandDirectionProperty);
            private set => SetValue(ActualExpandDirectionPropertyKey, value);
        }

        private static void OnActualExpandDirectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FlyoutWindow flyoutWindow)
            {
                flyoutWindow.UpdateAnimations((FlyoutWindowExpandDirection)e.NewValue);
            }
        }

        #endregion

        #region ExpandDirection

        public static readonly DependencyProperty ExpandDirectionProperty =
            DependencyProperty.Register(
                nameof(ExpandDirection),
                typeof(FlyoutWindowExpandDirection),
                typeof(FlyoutWindow),
                new PropertyMetadata(FlyoutWindowExpandDirection.Auto, OnExpandDirectionPropertyChanged));

        public FlyoutWindowExpandDirection ExpandDirection
        {
            get => (FlyoutWindowExpandDirection)GetValue(ExpandDirectionProperty);
            set => SetValue(ExpandDirectionProperty, value);
        }

        private static void OnExpandDirectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FlyoutWindow flyoutWindow)
            {
                flyoutWindow.CalculateActualExpandDirection();
            }
        }

        #endregion

        #region FlyoutWindowType

        public static readonly DependencyProperty FlyoutWindowTypeProperty =
            DependencyProperty.Register(
                nameof(FlyoutWindowType),
                typeof(FlyoutWindowType),
                typeof(FlyoutWindow),
                new PropertyMetadata(FlyoutWindowType.OnScreen, OnFlyoutWindowTypePropertyChanged));

        public FlyoutWindowType FlyoutWindowType
        {
            get => (FlyoutWindowType)GetValue(FlyoutWindowTypeProperty);
            set => SetValue(FlyoutWindowTypeProperty, value);
        }

        private static void OnFlyoutWindowTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FlyoutWindow flyoutWindow)
            {
                flyoutWindow.PositionFlyout();
            }
        }

        #endregion

        #region Margin

        public static readonly new DependencyProperty MarginProperty =
            DependencyProperty.Register(
                nameof(Margin),
                typeof(Thickness),
                typeof(FlyoutWindow),
                new PropertyMetadata(new Thickness(), OnMarginPropertyChanged));

        public new Thickness Margin
        {
            get => (Thickness)GetValue(MarginProperty);
            set => SetValue(MarginProperty, value);
        }

        private static void OnMarginPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FlyoutWindow flyoutWindow)
            {
                flyoutWindow.RePositionFlyout();
            }
        }

        #endregion

        #region Offset

        public static readonly DependencyProperty OffsetProperty =
            DependencyProperty.Register(
                nameof(Offset),
                typeof(Thickness),
                typeof(FlyoutWindow),
                new PropertyMetadata(new Thickness(), OnOffsetPropertyChanged));

        public Thickness Offset
        {
            get => (Thickness)GetValue(OffsetProperty);
            set => SetValue(OffsetProperty, value);
        }

        private static void OnOffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FlyoutWindow flyoutWindow)
            {
                flyoutWindow.RePositionFlyout();
            }
        }

        #endregion

        #region PlacementMode

        public static readonly DependencyProperty PlacementModeProperty =
            DependencyProperty.Register(
                nameof(PlacementMode),
                typeof(FlyoutWindowPlacementMode),
                typeof(FlyoutWindow),
                new PropertyMetadata(FlyoutWindowPlacementMode.Auto, OnPlacementModePropertyChanged));

        public FlyoutWindowPlacementMode PlacementMode
        {
            get => (FlyoutWindowPlacementMode)GetValue(PlacementModeProperty);
            set => SetValue(PlacementModeProperty, value);
        }

        private static void OnPlacementModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FlyoutWindow flyoutWindow)
            {
                flyoutWindow.PositionFlyout();
            }
        }

        #endregion

        #region PreferredDisplayMonitor

        public static readonly DependencyProperty PreferredDisplayMonitorProperty =
            DependencyProperty.Register(
                nameof(PreferredDisplayMonitor),
                typeof(DisplayMonitor),
                typeof(FlyoutWindow),
                new PropertyMetadata(OnPreferredDisplayMonitorPropertyChanged));

        public DisplayMonitor PreferredDisplayMonitor
        {
            get => (DisplayMonitor)GetValue(PreferredDisplayMonitorProperty);
            set => SetValue(PreferredDisplayMonitorProperty, value);
        }

        private static void OnPreferredDisplayMonitorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FlyoutWindow flyoutWindow)
            {
                if (flyoutWindow.PlacementMode == FlyoutWindowPlacementMode.Auto
                    && flyoutWindow.FlyoutWindowType == FlyoutWindowType.OnScreen)
                {
                    flyoutWindow.PositionFlyout();
                }
            }
        }

        #endregion

        #region Left

        public static readonly DependencyProperty LeftProperty = Canvas.LeftProperty.AddOwner(typeof(FlyoutWindow));

        public double Left
        {
            get => (double)GetValue(LeftProperty);
            set => SetValue(LeftProperty, value);
        }

        #endregion

        #region Top

        public static readonly DependencyProperty TopProperty = Canvas.TopProperty.AddOwner(typeof(FlyoutWindow));

        public double Top
        {
            get => (double)GetValue(TopProperty);
            set => SetValue(TopProperty, value);
        }

        #endregion

        #region IsTimeoutEnabled

        public static readonly DependencyProperty IsTimeoutEnabledProperty =
            DependencyProperty.Register(
                nameof(IsTimeoutEnabled),
                typeof(bool),
                typeof(FlyoutWindow),
                new PropertyMetadata(false, OnIsTimeoutEnabledPropertyChanged));

        public bool IsTimeoutEnabled
        {
            get => (bool)GetValue(IsTimeoutEnabledProperty);
            set => SetValue(IsTimeoutEnabledProperty, value);
        }

        private static void OnIsTimeoutEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FlyoutWindow flyoutWindow)
            {
                if ((bool)e.NewValue)
                {
                    flyoutWindow.SetupCloseTimer();
                }
                else
                {
                    flyoutWindow.StopCloseTimer();
                }
            }
        }

        #endregion

        #region Timeout

        public static readonly DependencyProperty TimeoutProperty =
            DependencyProperty.Register(
                nameof(Timeout),
                typeof(double),
                typeof(FlyoutWindow),
                new PropertyMetadata(1.0, OnTimeoutPropertyChanged));

        public double Timeout
        {
            get => (double)GetValue(TimeoutProperty);
            set => SetValue(TimeoutProperty, value);
        }

        private static void OnTimeoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FlyoutWindow flyoutWindow)
            {
                flyoutWindow.UpdateCloseTimerInterval((double)e.NewValue);
            }
        }

        #endregion

        #region IsOpen

        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register(
                nameof(IsOpen),
                typeof(bool),
                typeof(FlyoutWindow),
                new PropertyMetadata(false, OnIsOpenPropertyChanged));

        public bool IsOpen
        {
            get => (bool)GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }

        private static void OnIsOpenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FlyoutWindow flyoutWindow)
            {
                if ((bool)e.NewValue)
                {
                    flyoutWindow.OpenFlyout();
                }
                else
                {
                    flyoutWindow.CloseFlyout();
                }
            }
        }

        #endregion

        #region FlyoutAnimationEnabled

        public static readonly DependencyProperty FlyoutAnimationEnabledProperty =
            DependencyProperty.Register(
                nameof(FlyoutAnimationEnabled),
                typeof(bool),
                typeof(FlyoutWindow),
                new PropertyMetadata(false));

        public bool FlyoutAnimationEnabled
        {
            get => (bool)GetValue(FlyoutAnimationEnabledProperty);
            set => SetValue(FlyoutAnimationEnabledProperty, value);
        }

        #endregion
    }
}
