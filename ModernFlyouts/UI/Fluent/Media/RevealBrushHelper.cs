using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace ModernFlyouts.UI.Fluent.Media
{
    public enum RevealBrushState
    {
        Normal = 0,
        PointerOver = 1,
        Pressed = 2
    }

    public enum RevealBrushMode
    {
        None = 0,
        Border = 1,
        Background = 2,
        BackgroundPressed = 3
    }

    public static class RevealBrushHelper
    {
        #region State

        public static readonly DependencyProperty StateProperty =
            DependencyProperty.RegisterAttached(
                "State",
                typeof(RevealBrushState),
                typeof(RevealBrushHelper),
                new FrameworkPropertyMetadata(RevealBrushState.Normal, FrameworkPropertyMetadataOptions.Inherits, OnStatePropertyChanged));

        public static RevealBrushState GetState(DependencyObject obj)
        {
            return (RevealBrushState)obj.GetValue(StateProperty);
        }

        public static void SetState(DependencyObject obj, RevealBrushState value)
        {
            obj.SetValue(StateProperty, value);
        }

        private static void OnStatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement uiElement)
            {
                var revealBrushMode = GetRevealBrushMode(uiElement);
                if (revealBrushMode != RevealBrushMode.None)
                {
                    switch (revealBrushMode)
                    {
                        case RevealBrushMode.Border:
                            RevealBorderBrushHelper.UpdateBrush(uiElement);
                            break;

                        case RevealBrushMode.Background:
                        case RevealBrushMode.BackgroundPressed:
                            RevealBackgroundBrushHelper.UpdateBrush(uiElement);
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        #endregion

        #region MouseX

        internal static readonly DependencyProperty MouseXProperty =
            DependencyProperty.RegisterAttached(
                "MouseX",
                typeof(double),
                typeof(RevealBrushHelper),
                new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.Inherits));

        internal static double GetMouseX(DependencyObject obj)
        {
            return (double)obj.GetValue(MouseXProperty);
        }

        internal static void SetMouseX(DependencyObject obj, double value)
        {
            obj.SetValue(MouseXProperty, value);
        }

        #endregion

        #region MouseY

        internal static readonly DependencyProperty MouseYProperty =
            DependencyProperty.RegisterAttached(
                "MouseY",
                typeof(double),
                typeof(RevealBrushHelper),
                new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.Inherits));

        internal static double GetMouseY(DependencyObject obj)
        {
            return (double)obj.GetValue(MouseYProperty);
        }

        internal static void SetMouseY(DependencyObject obj, double value)
        {
            obj.SetValue(MouseYProperty, value);
        }

        #endregion

        #region MousePosition

        internal static readonly DependencyProperty MousePositionProperty =
            DependencyProperty.RegisterAttached(
                "MousePosition",
                typeof(Point),
                typeof(RevealBrushHelper),
                new FrameworkPropertyMetadata(default(Point), FrameworkPropertyMetadataOptions.Inherits));

        internal static Point GetMousePosition(DependencyObject obj)
        {
            return (Point)obj.GetValue(MousePositionProperty);
        }

        internal static void SetMousePosition(DependencyObject obj, Point value)
        {
            obj.SetValue(MousePositionProperty, value);
        }

        #endregion

        #region IsMouseOverRootVisual

        internal static readonly DependencyProperty IsMouseOverRootVisualProperty =
            DependencyProperty.RegisterAttached(
                "IsMouseOverRootVisual",
                typeof(bool),
                typeof(RevealBrushHelper),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits, OnIsMouseOverRootVisualPropertyChanged));

        internal static bool GetIsMouseOverRootVisual(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsMouseOverRootVisualProperty);
        }

        internal static void SetIsMouseOverRootVisual(DependencyObject obj, bool value)
        {
            obj.SetValue(IsMouseOverRootVisualProperty, value);
        }

        private static void OnIsMouseOverRootVisualPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement uiElement)
            {
                if (GetRevealBrushMode(uiElement) == RevealBrushMode.Border)
                {
                    RevealBorderBrushHelper.UpdateBrush(uiElement);
                }
            }
        }

        #endregion

        #region RootObject

        internal static readonly DependencyProperty RootObjectProperty =
            DependencyProperty.RegisterAttached(
                "RootObject",
                typeof(UIElement),
                typeof(RevealBrushHelper),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

        internal static UIElement GetRootObject(DependencyObject obj)
        {
            return (UIElement)obj.GetValue(RootObjectProperty);
        }

        internal static void SetRootObject(DependencyObject obj, UIElement value)
        {
            obj.SetValue(RootObjectProperty, value);
        }

        #endregion

        #region TrackThisElement

        public static readonly DependencyProperty TrackThisElementProperty =
            DependencyProperty.RegisterAttached(
                "TrackThisElement",
                typeof(bool),
                typeof(RevealBrushHelper),
                new PropertyMetadata(false, OnTrackThisElementPropertyChanged));

        public static bool GetTrackThisElement(DependencyObject obj)
        {
            return (bool)obj.GetValue(TrackThisElementProperty);
        }

        public static void SetTrackThisElement(DependencyObject obj, bool value)
        {
            obj.SetValue(TrackThisElementProperty, value);
        }

        private static void OnTrackThisElementPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not UIElement ctrl) return;

            if ((bool)e.NewValue)
            {
                ctrl.MouseEnter += OnMouseEnter;
                ctrl.PreviewMouseMove += OnPreviewMouseMove;
                ctrl.MouseLeave += OnMouseLeave;

                SetRootObject(ctrl, ctrl);
            }
            else
            {
                ctrl.MouseEnter -= OnMouseEnter;
                ctrl.PreviewMouseMove -= OnPreviewMouseMove;
                ctrl.MouseLeave -= OnMouseLeave;

                ctrl.ClearValue(RootObjectProperty);
            }
        }

        #endregion

        #region RevealBrushMode

        public static readonly DependencyProperty RevealBrushModeProperty =
            DependencyProperty.RegisterAttached(
                "RevealBrushMode",
                typeof(RevealBrushMode),
                typeof(RevealBrushHelper),
                new PropertyMetadata(RevealBrushMode.None, OnRevealBrushModePropertyChanged));

        public static RevealBrushMode GetRevealBrushMode(DependencyObject obj)
        {
            return (RevealBrushMode)obj.GetValue(RevealBrushModeProperty);
        }

        public static void SetRevealBrushMode(DependencyObject obj, RevealBrushMode value)
        {
            obj.SetValue(RevealBrushModeProperty, value);
        }

        private static void OnRevealBrushModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement uiElement && e.NewValue is RevealBrushMode revealBrushMode)
            {
                uiElement.OpacityMask = revealBrushMode switch
                {
                    RevealBrushMode.Border => RevealBorderBrushHelper.GetRevealBrush(uiElement),
                    RevealBrushMode.Background => RevealBackgroundBrushHelper.GetHoverRevealBrush(uiElement),
                    RevealBrushMode.BackgroundPressed => RevealBackgroundBrushHelper.GetPressedRevealBrush(uiElement),
                    // Could be set to any color given that its alpha is 100%. I just use YellowGreen because it's my favorite color.
                    _ => Brushes.YellowGreen,
                };
            }
        }

        #endregion

        #region IsBorderRevealBrush

        public static readonly DependencyProperty IsBorderRevealBrushProperty =
            DependencyProperty.RegisterAttached(
                "IsBorderRevealBrush",
                typeof(bool),
                typeof(RevealBrushHelper),
                new PropertyMetadata(false));

        public static bool GetIsBorderRevealBrush(Brush obj)
        {
            return (bool)obj.GetValue(IsBorderRevealBrushProperty);
        }

        public static void SetIsBorderRevealBrush(Brush obj, bool value)
        {
            obj.SetValue(IsBorderRevealBrushProperty, value);
        }

        #endregion

        #region IsBackgroundRevealBrush

        public static readonly DependencyProperty IsBackgroundRevealBrushProperty =
            DependencyProperty.RegisterAttached(
                "IsBackgroundRevealBrush",
                typeof(bool),
                typeof(RevealBrushHelper),
                new PropertyMetadata(false));

        public static bool GetIsBackgroundRevealBrush(Brush obj)
        {
            return (bool)obj.GetValue(IsBackgroundRevealBrushProperty);
        }

        public static void SetIsBackgroundRevealBrush(Brush obj, bool value)
        {
            obj.SetValue(IsBackgroundRevealBrushProperty, value);
        }

        #endregion

        #region RevealBrush

        internal static readonly DependencyProperty RevealBrushProperty =
            DependencyProperty.RegisterAttached(
                "RevealBrush",
                typeof(Brush),
                typeof(RevealBrushHelper),
                new PropertyMetadata(null));

        internal static Brush GetRevealBrush(UIElement obj)
        {
            return (Brush)obj.GetValue(RevealBrushProperty);
        }

        internal static void SetRevealBrush(UIElement obj, Brush value)
        {
            obj.SetValue(RevealBrushProperty, value);
        }

        #endregion

        private static void OnMouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is UIElement ctrl)
            {
                SetIsMouseOverRootVisual(ctrl, true);
            }
        }

        private static void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (sender is UIElement ctrl && GetIsMouseOverRootVisual(ctrl))
            {
                var pos = e.GetPosition(ctrl);

                SetMouseX(ctrl, pos.X);
                SetMouseY(ctrl, pos.Y);
                SetMousePosition(ctrl, pos);
            }
        }

        private static void OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is UIElement ctrl)
            {
                SetIsMouseOverRootVisual(ctrl, false);
            }
        }
    }

    internal class RelativePositionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Any(o => o == DependencyProperty.UnsetValue || o == null)) return new Point(0, 0);

            var parent = values[0] as UIElement;
            var ctrl = values[1] as UIElement;
            var pointerPos = (Point)values[2];
            var relativePos = parent.TranslatePoint(pointerPos, ctrl);

            return relativePos;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
