using ModernFlyouts.Core.Interop;
using System;
using System.Windows;

namespace ModernFlyouts.Core.UI
{
    public partial class FlyoutWindow : BandWindow
    {
        static FlyoutWindow()
        {
            ContentProperty.OverrideMetadata(typeof(BandWindow), new FrameworkPropertyMetadata(OnContentPropertyChanged));
        }

        public FlyoutWindow()
        {
            PrepareAnimations();
            SizeChanged += FlyoutWindow_SizeChanged;
        }

        private static void OnContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FlyoutWindow flyoutWindow)
            {
                if (e.OldValue is FrameworkElement oldContent)
                {
                    SetFlyoutWindow(oldContent, null);
                }
                if (e.NewValue is FrameworkElement newContent)
                {
                    SetFlyoutWindow(newContent, flyoutWindow);
                }
            }
        }

        protected override void OnShown()
        {
            base.OnShown();

            PositionFlyout();
        }

        protected override void OnDpiChanged()
        {
            base.OnDpiChanged();

            RePositionFlyout();
        }

        private void FlyoutWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            PositionFlyout();
        }

        protected override void OnDragMoved()
        {
            StartCloseTimer();

            var offset = Offset;

            Left = ActualLeft + (DpiScale * offset.Left);
            Top = ActualTop + (DpiScale * Offset.Top);

            if (PlacementMode != FlyoutWindowPlacementMode.Manual)
            {
                PlacementMode = FlyoutWindowPlacementMode.Manual;
            }

            base.OnDragMoved();
        }

        private double effectiveMarginLeft = 0;
        private double effectiveMarginRight = 0;
        private double effectiveMarginTop = 0;
        private double effectiveMarginBottom = 0;

        private void CalculateEffectiveMargin()
        {
            var margin = Margin;
            var offset = Offset;
            effectiveMarginLeft = margin.Left - (DpiScale * offset.Left);
            effectiveMarginRight = margin.Right - (DpiScale * offset.Right);
            effectiveMarginTop = margin.Top - (DpiScale * offset.Top);
            effectiveMarginBottom = margin.Bottom - (DpiScale * offset.Bottom);
        }

        private void RePositionFlyout()
        {
            CalculateEffectiveMargin();
            PositionFlyout();
        }

        private void PositionFlyout()
        {
            if (!HasSourceCreated)
                return;

            if (PlacementMode == FlyoutWindowPlacementMode.Auto)
            {
                double x = 0;
                double y = 0;
                double width = ActualWidth * DpiScale;
                double height = ActualHeight * DpiScale;

                if (FlyoutWindowType == FlyoutWindowType.OnScreen)
                {
                    PositionOnScreenFlyout(ref x, ref y, width, height);
                }
                else if (FlyoutWindowType == FlyoutWindowType.Tray)
                {
                    PositionTrayFlyout(ref x, ref y, width, height);
                }

                SetPosition(x, y);

                CalculateActualExpandDirection();
            }
            else if (PlacementMode == FlyoutWindowPlacementMode.Manual)
            {
                AlignToPosition();

                if (ExpandDirection == FlyoutWindowExpandDirection.Auto)
                {
                    ActualExpandDirection = FlyoutWindowExpandDirection.Down;
                }
            }
        }

        public void AlignToPosition()
        {
            if (IsDragMoving)
                return;

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

            var offset = Offset;
            double x = Left - (DpiScale * offset.Left);
            double y = Top - (DpiScale * offset.Top);

            SetPosition(x, y);
        }

        private void CalculateActualExpandDirection()
        {
            var expandDirection = ExpandDirection;
            if (expandDirection == FlyoutWindowExpandDirection.Auto)
            {
                if (FlyoutWindowType == FlyoutWindowType.OnScreen)
                {
                    expandDirection = CalculateActualExpandDirectionOnScreen();
                }
                else if (FlyoutWindowType == FlyoutWindowType.Tray)
                {
                    expandDirection = CalculateActualExpandDirectionTray();
                }
            }

            ActualExpandDirection = expandDirection;
        }
    }
}
