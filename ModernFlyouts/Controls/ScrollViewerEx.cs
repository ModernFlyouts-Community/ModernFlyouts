using ModernFlyouts.Helpers;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ModernFlyouts.Controls
{
    public class ScrollViewerEx : ScrollViewer
    {
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            if (Style == null && ReadLocalValue(StyleProperty) == DependencyProperty.UnsetValue)
            {
                SetResourceReference(StyleProperty, typeof(ScrollViewer));
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (e.Handled) { return; }
            ScrollViewerHelperEx.OnMouseWheel(this, e);
            e.Handled = true;
        }
    }
}
