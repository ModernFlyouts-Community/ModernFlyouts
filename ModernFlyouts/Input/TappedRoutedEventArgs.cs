using System.Windows;

namespace ModernFlyouts.Input
{
    internal sealed class TappedRoutedEventArgs : RoutedEventArgs
    {
        public TappedRoutedEventArgs()
        {
        }

        internal int Timestamp { get; set; }
    }
}
