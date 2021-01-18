using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModernFlyouts.Controls
{
    /// <summary>
    /// This control does everything a <see cref="Canvas"/> does
    /// plus auto sizes its <strong>children</strong> to fit its own size.
    /// This is similar to placing a <see cref="Canvas"/> in a <see cref="ViewBox"/>.
    /// But unlike a <see cref="ViewBox"/>, this actual modifies the dimensions of the content.
    /// </summary>
    /// <remarks>
    /// <strong>Whewwwwwwwwwwwwwwwwwwwwwwwwww!</strong>, creating this was a pain in the a**.
    /// Don't make your own control kids. Use available open source libraries instead.
    /// </remarks>
    public class AutoScaleCanvas : Panel
    {
        #region Properties

        #region Left

        public static readonly DependencyProperty LeftProperty =
            DependencyProperty.RegisterAttached(
                "Left",
                typeof(double),
                typeof(AutoScaleCanvas),
                new FrameworkPropertyMetadata(0.0, OnLayoutParameterChanged));

        public static double GetLeft(DependencyObject obj)
        {
            return (double)obj.GetValue(LeftProperty);
        }

        public static void SetLeft(DependencyObject obj, double value)
        {
            obj.SetValue(LeftProperty, value);
        }

        #endregion

        #region Top

        public static readonly DependencyProperty TopProperty =
            DependencyProperty.RegisterAttached(
                "Top",
                typeof(double),
                typeof(AutoScaleCanvas),
                new FrameworkPropertyMetadata(0.0, OnLayoutParameterChanged));

        public static double GetTop(DependencyObject obj)
        {
            return (double)obj.GetValue(TopProperty);
        }

        public static void SetTop(DependencyObject obj, double value)
        {
            obj.SetValue(TopProperty, value);
        }

        #endregion

        #region RequestedWidth

        public static readonly DependencyProperty RequestedWidthProperty =
            DependencyProperty.RegisterAttached(
                "RequestedWidth",
                typeof(double),
                typeof(AutoScaleCanvas),
                new FrameworkPropertyMetadata(0.0, OnLayoutParameterChanged));

        public static double GetRequestedWidth(DependencyObject obj)
        {
            return (double)obj.GetValue(RequestedWidthProperty);
        }

        public static void SetRequestedWidth(DependencyObject obj, double value)
        {
            obj.SetValue(RequestedWidthProperty, value);
        }

        #endregion

        #region RequestedHeight

        public static readonly DependencyProperty RequestedHeightProperty =
            DependencyProperty.RegisterAttached(
                "RequestedHeight",
                typeof(double),
                typeof(AutoScaleCanvas),
                new FrameworkPropertyMetadata(0.0, OnLayoutParameterChanged));

        public static double GetRequestedHeight(DependencyObject obj)
        {
            return (double)obj.GetValue(RequestedHeightProperty);
        }

        public static void SetRequestedHeight(DependencyObject obj, double value)
        {
            obj.SetValue(RequestedHeightProperty, value);
        }

        #endregion

        #region RequiredSize

        public static readonly DependencyProperty RequiredSizeProperty =
            DependencyProperty.Register(
                nameof(RequiredSize),
                typeof(Size),
                typeof(AutoScaleCanvas),
                new FrameworkPropertyMetadata(new Size(), FrameworkPropertyMetadataOptions.AffectsMeasure));

        public Size RequiredSize
        {
            get => (Size)GetValue(RequiredSizeProperty);
            set => SetValue(RequiredSizeProperty, value);
        }

        #endregion

        #endregion

        private static void OnLayoutParameterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement element)
            {
                if (VisualTreeHelper.GetParent(element) is AutoScaleCanvas canvas)
                    canvas.InvalidateArrange();
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Size childConstraint = new(double.PositiveInfinity, double.PositiveInfinity);
            // Whoa! That's huge af! But not as big as yo mama!

            foreach (UIElement child in InternalChildren)
            {
                if (child == null) { continue; }

                child.Measure(childConstraint);
            }

            var requiredSize = RequiredSize;

            if (!double.IsFinite(constraint.Width))
                constraint.Width = 0;

            if (!double.IsFinite(constraint.Height))
                constraint.Height = 0;

            var scale = GetScale(constraint, requiredSize);

            if (scale == 0)
                return new(); // "UgHhHh, why? Thats's quite schtewpid" you may say.
            // But I did this to prevent the next line from being called
            // in-order to reduce the CPU usage by 0.00000000069% (nice)

            return new Size(requiredSize.Width * scale, requiredSize.Height * scale);
        }

        private static double GetScale(Size availableSize, Size requiredSize)
        {
            double scaleHeight = requiredSize.Width == 0 ? 0 : availableSize.Height / requiredSize.Height;
            double scaleWidth = requiredSize.Height == 0 ? 0 : availableSize.Width / requiredSize.Width;
            return Math.Min(scaleHeight, scaleWidth);
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            var requiredSize = RequiredSize;
            var scale = GetScale(arrangeSize, requiredSize);

            foreach (UIElement child in InternalChildren)
            {
                if (child == null) { continue; }

                double left = GetLeft(child);
                if (double.IsNaN(left))
                    left = 0;

                double x = left * scale;

                double top = GetTop(child);
                if (double.IsNaN(top))
                    top = 0;

                double y = top * scale;

                double width = GetRequestedWidth(child);
                if (double.IsNaN(width))
                    width = 0;

                width *= scale;

                double height = GetRequestedHeight(child);
                if (double.IsNaN(height))
                    height = 0;

                height *= scale;

                child.Arrange(new Rect(x, y, width, height));
            }

            return new Size(requiredSize.Width * scale, requiredSize.Height * scale);
        }
    }
}
