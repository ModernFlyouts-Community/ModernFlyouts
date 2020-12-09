using ModernWpf;
using System;
using System.Windows;

namespace ModernFlyouts.UI.Media
{
    public class BindablePoint : DependencyObject
    {
        public static double MaxXValue = int.MaxValue;
        public static double MaxYValue = int.MaxValue;

        #region Properties

        public static readonly DependencyProperty XProperty =
            DependencyProperty.Register(
                nameof(X),
                typeof(double),
                typeof(BindablePoint),
                new PropertyMetadata(0.0, OnXPropertyChanged));

        public double X
        {
            get => (double)GetValue(XProperty);
            set => SetValue(XProperty, value);
        }

        private static void OnXPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BindablePoint point)
            {
                point.OnValueChanged();
            }
        }

        public static readonly DependencyProperty YProperty =
            DependencyProperty.Register(
                nameof(Y),
                typeof(double),
                typeof(BindablePoint),
                new PropertyMetadata(0.0, OnYPropertyChanged));

        public double Y
        {
            get => (double)GetValue(YProperty);
            set => SetValue(YProperty, value);
        }

        private static void OnYPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BindablePoint point)
            {
                point.OnValueChanged();
            }
        }

        #endregion

        public event TypedEventHandler<BindablePoint, EventArgs> ValueChanged;

        public BindablePoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        public BindablePoint() : this(0.0, 0.0)
        {

        }

        private void OnValueChanged()
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        public static bool TryParse(string value, out BindablePoint result)
        {
            result = new();
            if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value))
            {
                var values = value.Split(",");
                if (values.Length == 2)
                {
                    var vX = values[0].Trim();
                    var vY = values[1].Trim();

                    if (double.TryParse(vX, out double x) && double.TryParse(vY, out double y))
                    {
                        result.X = x;
                        result.Y = y;

                        return true;
                    }
                }
            }

            return false;
        }

        public override string ToString()
        {
            return $"{X}, {Y}";
        }
    }
}
