using ModernFlyouts.Core.Interop;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ModernFlyouts.Core.UI
{
    public partial class FlyoutWindow : BandWindow
    {
        #region Properties

        #region Alignment

        public static readonly DependencyProperty AlignmentProperty = DependencyProperty.Register(
            nameof(Alignment),
            typeof(FlyoutWindowAlignment),
            typeof(FlyoutWindow),
            new PropertyMetadata(FlyoutWindowAlignment.Top | FlyoutWindowAlignment.Left, OnAlignmentPropertyChanged));

        public FlyoutWindowAlignment Alignment
        {
            get => (FlyoutWindowAlignment)GetValue(AlignmentProperty);
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

        private static readonly DependencyPropertyKey ActualExpandDirectionPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(ActualExpandDirection),
            typeof(FlyoutWindowExpandDirection),
            typeof(FlyoutWindow),
            new PropertyMetadata(FlyoutWindowExpandDirection.Down));

        public static readonly DependencyProperty ActualExpandDirectionProperty = ActualExpandDirectionPropertyKey.DependencyProperty;

        public FlyoutWindowExpandDirection ActualExpandDirection
        {
            get => (FlyoutWindowExpandDirection)GetValue(ActualExpandDirectionProperty);
            private set => SetValue(ActualExpandDirectionPropertyKey, value);
        }

        #endregion

        #region ExpandDirection

        public static readonly DependencyProperty ExpandDirectionProperty = DependencyProperty.Register(
            nameof(ExpandDirection),
            typeof(FlyoutWindowExpandDirection),
            typeof(FlyoutWindow),
            new PropertyMetadata(FlyoutWindowExpandDirection.Auto));

        public FlyoutWindowExpandDirection ExpandDirection
        {
            get => (FlyoutWindowExpandDirection)GetValue(ExpandDirectionProperty);
            set => SetValue(ExpandDirectionProperty, value);
        }

        #endregion

        #region FlyoutWindowType

        public static readonly DependencyProperty FlyoutWindowTypeProperty = DependencyProperty.Register(
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

        public static readonly DependencyProperty MarginProperty = FrameworkElement.MarginProperty.AddOwner(
            typeof(FlyoutWindow), new PropertyMetadata(OnMarginPropertyChanged));

        public Thickness Margin
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

        public static readonly DependencyProperty OffsetProperty = DependencyProperty.RegisterAttached(
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

        public static readonly DependencyProperty PlacementModeProperty = DependencyProperty.Register(
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

        #endregion

        protected override void OnDpiChanged(double newDpiScale)
        {
            base.OnDpiChanged(newDpiScale);
            CalculateEffectiveMargin();
        }

        protected override void OnIsVisibleChanged()
        {
            base.OnIsVisibleChanged();

            if (IsVisible)
            {
                PositionFlyout();
            }
        }

        protected override void OnSizeChanged()
        {
            base.OnSizeChanged();

            PositionFlyout();
        }

        protected override void OnDragMoved()
        {
            base.OnDragMoved();

            Left = ActualLeft + Offset.Left;
            Top = ActualTop + Offset.Top;

            if (PlacementMode != FlyoutWindowPlacementMode.Manual)
            {
                PlacementMode = FlyoutWindowPlacementMode.Manual;
            }
        }

        private double effectiveMarginLeft = 0;
        private double effectiveMarginRight = 0;
        private double effectiveMarginTop = 0;
        private double effectiveMarginBottom = 0;

        private void CalculateEffectiveMargin()
        {
            effectiveMarginLeft = Margin.Left - (DpiScale * Offset.Left);
            effectiveMarginRight = Margin.Right - (DpiScale * Offset.Right);
            effectiveMarginTop = Margin.Top - (DpiScale * Offset.Top);
            effectiveMarginBottom = Margin.Bottom - (DpiScale * Offset.Bottom);
        }

        private void RePositionFlyout()
        {
            CalculateEffectiveMargin();
            PositionFlyout();
        }

        private void PositionFlyout()
        {
            if (!IsLoaded)
                return;

            if (PlacementMode == FlyoutWindowPlacementMode.Auto)
            {
                double x = 0;
                double y = 0;
                double width = ActualWidth;
                double height = ActualHeight;

                if (FlyoutWindowType == FlyoutWindowType.OnScreen)
                {
                    PositionOnScreenFlyout(ref x, ref y, width, height);
                }
                else if (FlyoutWindowType == FlyoutWindowType.Tray)
                {
                    PositionTrayFlyout(ref x, ref y, width, height);
                }

                SetPosition(x, y);
            }
            else if (PlacementMode == FlyoutWindowPlacementMode.Manual)
            {
                AlignToPosition();
            }
        }

        public void AlignToPosition()
        {
            if (PlacementMode != FlyoutWindowPlacementMode.Manual)
            {
                throw new InvalidOperationException(nameof(AlignToPosition)
                    + " should be called only if the flyout window's FlyoutWindowPlacementMode is "
                    + FlyoutWindowPlacementMode.Manual);
            }

            if (FlyoutWindowType != FlyoutWindowType.OnScreen)
            {
                throw new InvalidOperationException("Manual placement should only be set for a flyout window of FlyoutWindowType "
                    + FlyoutWindowType.OnScreen);
            }

            double x = Left - (DpiScale * Offset.Left);
            double y = Top - (DpiScale * Offset.Top);

            SetPosition(x, y);
        }
    }
}
