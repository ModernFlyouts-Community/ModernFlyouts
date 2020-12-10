// TODO: Move this to ModernWpf community toolkit and reference it

using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ModernFlyouts.UI.Fluent.Media
{
    internal class RevealBorderBrushHelper
    {
        private const double DefaultRevealBorderBrushRadius = 80.0;

        public static Brush GetRevealBrush(UIElement target)
        {
            RadialGradientBrush brush = new(Colors.White, Colors.Transparent)
            {
                MappingMode = BrushMappingMode.Absolute,
                RadiusX = DefaultRevealBorderBrushRadius,
                RadiusY = DefaultRevealBorderBrushRadius
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
            bool isMouseOverRootVisual = RevealBrushHelper.GetIsMouseOverRootVisual(uiElement);

            if (RevealBrushHelper.GetRevealBrush(uiElement) is Brush brush)
            {
                brush.Opacity = (isMouseOverRootVisual && revealBrushState == RevealBrushState.Normal) ? 1.0 : 0.0;
            }
        }
    }
}
