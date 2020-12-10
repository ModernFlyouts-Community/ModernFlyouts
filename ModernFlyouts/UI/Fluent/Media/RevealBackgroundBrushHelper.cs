// TODO: Move this to ModernWpf community toolkit and reference it

using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ModernFlyouts.UI.Fluent.Media
{
    internal class RevealBackgroundBrushHelper
    {
        private const double DefaultRevealBackgroudBrushRadius = 450.0;

        public static Brush GetHoverRevealBrush(UIElement target)
        {
            RadialGradientBrush brush = new(Colors.White, Colors.Transparent)
            {
                MappingMode = BrushMappingMode.Absolute,
                RadiusX = DefaultRevealBackgroudBrushRadius,
                RadiusY = DefaultRevealBackgroudBrushRadius,
                Opacity = 0.0
            };

            var binding = new MultiBinding
            {
                Converter = new RelativePositionConverter()
            };
            binding.Bindings.Add(new Binding() { Source = target, Path = new PropertyPath(RevealBrushHelper.RootObjectProperty) });
            binding.Bindings.Add(new Binding() { Source = target });
            binding.Bindings.Add(new Binding() { Source = target, Path = new PropertyPath(RevealBrushHelper.MousePositionProperty) });

            BindingOperations.SetBinding(brush, RadialGradientBrush.CenterProperty, binding);
            BindingOperations.SetBinding(brush, RadialGradientBrush.GradientOriginProperty, binding);

            RevealBrushHelper.SetRevealBrush(target, brush);

            return brush;
        }

        public static Brush GetPressedRevealBrush(UIElement target)
        {
            var gradientStops = new GradientStopCollection(
                new []
                {
                    new GradientStop(Color.FromArgb(30, 255, 255, 255), 0),
                    new GradientStop(Color.FromArgb(40, 255, 255, 255), 0),
                    new GradientStop(Colors.Transparent, 1)
                });

            RadialGradientBrush brush = new(gradientStops)
            {
                MappingMode = BrushMappingMode.Absolute,
                RadiusX = 200.0,
                RadiusY = 200.0,
                Opacity = 0.0
            };

            var binding = new MultiBinding
            {
                Converter = new RelativePositionConverter()
            };
            binding.Bindings.Add(new Binding() { Source = target, Path = new PropertyPath(RevealBrushHelper.RootObjectProperty) });
            binding.Bindings.Add(new Binding() { Source = target });
            binding.Bindings.Add(new Binding() { Source = target, Path = new PropertyPath(RevealBrushHelper.MousePositionProperty) });

            BindingOperations.SetBinding(brush, RadialGradientBrush.CenterProperty, binding);
            BindingOperations.SetBinding(brush, RadialGradientBrush.GradientOriginProperty, binding);

            RevealBrushHelper.SetRevealBrush(target, brush);

            return brush;
        }

        public static void UpdateBrush(UIElement uiElement)
        {
            RevealBrushState revealBrushState = RevealBrushHelper.GetState(uiElement);
            RevealBrushMode revealBrushMode = RevealBrushHelper.GetRevealBrushMode(uiElement);

            if (RevealBrushHelper.GetRevealBrush(uiElement) is RadialGradientBrush brush)
            {
                if (revealBrushMode == RevealBrushMode.Background)
                {
                    brush.Opacity = (revealBrushState != RevealBrushState.Normal) ? 0.1 : 0.0;
                }
                else if (revealBrushMode == RevealBrushMode.BackgroundPressed)
                {
                    if (revealBrushState == RevealBrushState.Pressed)
                    {
                        var time = TimeSpan.FromMilliseconds(1500);
                        var easingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };

                        var opacityAnimation = new DoubleAnimation(1.0, 0.0, time)
                        {
                            EasingFunction = easingFunction
                        };

                        brush.BeginAnimation(Brush.OpacityProperty, opacityAnimation);

                        var radiusAnimation = new DoubleAnimation(50.0, 200.0, time)
                        {
                            EasingFunction = easingFunction
                        };

                        brush.BeginAnimation(RadialGradientBrush.RadiusXProperty, radiusAnimation);
                        brush.BeginAnimation(RadialGradientBrush.RadiusYProperty, radiusAnimation);

                        //var gradientStop = brush.GradientStops[1];

                        //var offsetAnimation = new DoubleAnimation(0, 0.8, time)
                        //{
                        //    EasingFunction = easingFunction
                        //};

                        //gradientStop.BeginAnimation(GradientStop.OffsetProperty, offsetAnimation);
                    }
                    else
                    {
                        var time = TimeSpan.FromMilliseconds(200);
                        var easingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };

                        var opacityAnimation = new DoubleAnimation(0.0, time)
                        {
                            EasingFunction = easingFunction
                        };

                        brush.BeginAnimation(Brush.OpacityProperty, opacityAnimation);

                        var radiusAnimation = new DoubleAnimation(200.0, time)
                        {
                            EasingFunction = easingFunction
                        };

                        brush.BeginAnimation(RadialGradientBrush.RadiusXProperty, radiusAnimation);
                        brush.BeginAnimation(RadialGradientBrush.RadiusYProperty, radiusAnimation);
                    }
                }
            }
        }
    }
}
