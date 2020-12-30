using Microsoft.Xaml.Behaviors;
using ModernFlyouts.Core.UI;
using System.Windows;

namespace ModernFlyouts.Behaviors
{
    public class DragFlyoutBehavior : Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            AssociatedObject.MouseLeftButtonDown += AssociatedObject_MouseLeftButtonDown;
        }

        private void AssociatedObject_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var flyoutWindow = FlyoutWindow.GetFlyoutWindow(AssociatedObject);
            if (flyoutWindow != null)
            {
                flyoutWindow.DragMove();
            }
        }
    }
}
