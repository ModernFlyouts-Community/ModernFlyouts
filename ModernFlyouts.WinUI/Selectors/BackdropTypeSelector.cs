using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ModernFlyouts.Core.Models;

namespace ModernFlyouts.UI.Selectors
{
    internal class BackdropTypeSelector : DataTemplateSelector
    {
        public DataTemplate Solid { get; set; }
        public DataTemplate Mica { get; set; }
        public DataTemplate Acrylic { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            switch (item as BackdropType?)
            {
                case BackdropType.Solid:
                    return Solid;
                case BackdropType.Mica:
                    return Mica;
                case BackdropType.Acrylic:
                    return Acrylic;
                default:
                    return null;
            }
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return SelectTemplateCore(item);
        }
    }
}
