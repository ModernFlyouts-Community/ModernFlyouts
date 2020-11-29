// TODO: Move this to ModernWpf community toolkit and reference it

using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ModernFlyouts.UI.Fluent.Media
{
    public class RevealBorderBrushHelper
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

            Binding opacityBinding = new("Opacity")
            {
                Source = target,
                Path = new PropertyPath(RevealBrushHelper.IsMouseOverRootVisualProperty),
                Converter = new OpacityConverter(),
                ConverterParameter = 1.0 // opacity has been reduced by 5% as per WinUI design guidelines
            };
            BindingOperations.SetBinding(brush, RadialGradientBrush.OpacityProperty, opacityBinding);

            var binding = new MultiBinding
            {
                Converter = new RelativePositionConverter()
            };
            binding.Bindings.Add(new Binding() { Source = target, Path = new PropertyPath(RevealBrushHelper.RootObjectProperty) });
            binding.Bindings.Add(new Binding() { Source = target });
            binding.Bindings.Add(new Binding() { Source = target, Path = new PropertyPath(RevealBrushHelper.MousePositionProperty) });

            BindingOperations.SetBinding(brush, RadialGradientBrush.CenterProperty, binding);
            BindingOperations.SetBinding(brush, RadialGradientBrush.GradientOriginProperty, binding);

            return brush;
        }
    }
}
