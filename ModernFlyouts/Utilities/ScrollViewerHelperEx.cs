using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace ModernFlyouts.Utilities
{
    internal static class ScrollViewerHelperEx
    {
        #region IsSmoothScrollingEnabled

        public static readonly DependencyProperty IsSmoothScrollingEnabledProperty =
            DependencyProperty.RegisterAttached(
                "IsSmoothScrollingEnabled",
                typeof(bool),
                typeof(ScrollViewerHelperEx),
                new PropertyMetadata(false, OnIsSmoothScrollingEnabledChanged));

        public static bool GetIsSmoothScrollingEnabled(ScrollViewer scrollViewer)
        {
            return (bool)scrollViewer.GetValue(IsSmoothScrollingEnabledProperty);
        }

        public static void SetIsSmoothScrollingEnabled(ScrollViewer scrollViewer, bool value)
        {
            scrollViewer.SetValue(IsSmoothScrollingEnabledProperty, value);
        }

        private static void OnIsSmoothScrollingEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var scrollViewer = (ScrollViewer)d;
            var newValue = (bool)e.NewValue;

            if (newValue)
            {
                scrollViewer.PreviewMouseWheel += OnMouseWheel;
                SetCurrentVerticalOffset(scrollViewer, scrollViewer.VerticalOffset);
            }
            else
            {
                scrollViewer.PreviewMouseWheel -= OnMouseWheel;
            }
        }

        #endregion

        #region IsAnimating

        internal static readonly DependencyProperty IsAnimatingProperty =
            DependencyProperty.RegisterAttached(
                "IsAnimating",
                typeof(bool),
                typeof(ScrollViewerHelperEx),
                new PropertyMetadata(false));

        internal static bool GetIsAnimating(ScrollViewer scrollViewer)
        {
            return (bool)scrollViewer.GetValue(IsAnimatingProperty);
        }

        internal static void SetIsAnimating(ScrollViewer scrollViewer, bool value)
        {
            scrollViewer.SetValue(IsAnimatingProperty, value);
        }

        #endregion

        #region CurrentVerticalOffset

        internal static readonly DependencyProperty CurrentVerticalOffsetProperty =
            DependencyProperty.RegisterAttached("CurrentVerticalOffset",
                typeof(double),
                typeof(ScrollViewerHelperEx),
                new PropertyMetadata(0.0, OnCurrentVerticalOffsetChanged));

        private static double GetCurrentVerticalOffset(ScrollViewer scrollViewer)
        {
            return (double)scrollViewer.GetValue(CurrentVerticalOffsetProperty);
        }

        private static void SetCurrentVerticalOffset(ScrollViewer scrollViewer, double value)
        {
            scrollViewer.SetValue(CurrentVerticalOffsetProperty, value);
        }

        private static void OnCurrentVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScrollViewer ctl && e.NewValue is double v)
            {
                ctl.ScrollToVerticalOffset(v);
            }
        }

        #endregion

        #region CurrentHorizontalOffset

        internal static readonly DependencyProperty CurrentHorizontalOffsetProperty =
            DependencyProperty.RegisterAttached("CurrentHorizontalOffset",
                typeof(double),
                typeof(ScrollViewerHelperEx),
                new PropertyMetadata(0.0, OnCurrentHorizontalOffsetChanged));

        private static double GetCurrentHorizontalOffset(ScrollViewer scrollViewer)
        {
            return (double)scrollViewer.GetValue(CurrentHorizontalOffsetProperty);
        }

        private static void SetCurrentHorizontalOffset(ScrollViewer scrollViewer, double value)
        {
            scrollViewer.SetValue(CurrentHorizontalOffsetProperty, value);
        }

        private static void OnCurrentHorizontalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScrollViewer ctl && e.NewValue is double v)
            {
                ctl.ScrollToHorizontalOffset(v);
            }
        }

        #endregion

        private static void OnMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;
            e.Handled = true;

            if (!GetIsAnimating(scrollViewer))
            {
                SetCurrentVerticalOffset(scrollViewer, scrollViewer.VerticalOffset);
            }

            double _totalVerticalOffset = Math.Min(Math.Max(0, scrollViewer.VerticalOffset - e.Delta), scrollViewer.ScrollableHeight);
            ScrollToVerticalOffset(scrollViewer, _totalVerticalOffset);
        }

        public static void ScrollToOffset(ScrollViewer scrollViewer, Orientation orientation, double offset, double duration = 500, IEasingFunction easingFunction = null)
        {
            var animation = new DoubleAnimation(offset, TimeSpan.FromMilliseconds(duration));
            easingFunction ??= new CubicEase
            {
                EasingMode = EasingMode.EaseOut
            };
            animation.EasingFunction = easingFunction;
            animation.FillBehavior = FillBehavior.Stop;
            animation.Completed += (s, e1) =>
            {
                if (orientation == Orientation.Vertical)
                {
                    SetCurrentVerticalOffset(scrollViewer, offset);
                }
                else
                {
                    SetCurrentHorizontalOffset(scrollViewer, offset);
                }
                SetIsAnimating(scrollViewer, false);
            };
            SetIsAnimating(scrollViewer, true);

            scrollViewer.BeginAnimation(orientation == Orientation.Vertical ? CurrentVerticalOffsetProperty : CurrentHorizontalOffsetProperty, animation, HandoffBehavior.Compose);
        }

        public static void ScrollToVerticalOffset(ScrollViewer scrollViewer, double offset, double duration = 500, IEasingFunction easingFunction = null)
        {
            ScrollToOffset(scrollViewer, Orientation.Vertical, offset, duration, easingFunction);
        }

        public static void ScrollToHorizontalOffset(ScrollViewer scrollViewer, double offset, double duration = 500, IEasingFunction easingFunction = null)
        {
            ScrollToOffset(scrollViewer, Orientation.Horizontal, offset, duration, easingFunction);
        }
    }
}
