using System.Windows;

namespace ModernFlyouts.Core.UI
{
    public partial class FlyoutWindow
    {
        public static readonly RoutedEvent OpenedEvent = EventManager.RegisterRoutedEvent(
            nameof(Opened),
            RoutingStrategy.Direct,
            typeof(RoutedEventHandler),
            typeof(FlyoutWindow));

        public event RoutedEventHandler Opened
        {
            add { AddHandler(OpenedEvent, value); }
            remove { RemoveHandler(OpenedEvent, value); }
        }

        public static readonly RoutedEvent ClosedEvent = EventManager.RegisterRoutedEvent(
            nameof(Closed),
            RoutingStrategy.Direct,
            typeof(RoutedEventHandler),
            typeof(FlyoutWindow));

        public event RoutedEventHandler Closed
        {
            add { AddHandler(ClosedEvent, value); }
            remove { RemoveHandler(ClosedEvent, value); }
        }

        public static readonly RoutedEvent ClosingEvent = EventManager.RegisterRoutedEvent(
            nameof(Closing),
            RoutingStrategy.Direct,
            typeof(RoutedEventHandler),
            typeof(FlyoutWindow));

        public event RoutedEventHandler Closing
        {
            add { AddHandler(ClosingEvent, value); }
            remove { RemoveHandler(ClosingEvent, value); }
        }
    }
}
