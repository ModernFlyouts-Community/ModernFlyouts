using ModernFlyouts.Core.Interop;
using ModernFlyouts.Core.UI;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ModernFlyouts.Internal.Playground
{
    /// <summary>
    /// Interaction logic for MyView.xaml
    /// </summary>
    public partial class MyView : UserControl
    {
        public MyView()
        {
            InitializeComponent();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                BandWindow.GetBandWindow(this).DragMove();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var bandWindow = BandWindow.GetBandWindow(this);

            if (sender is ToggleButton tg && bandWindow is FlyoutWindow flyoutWindow)
            {
                if (tg.IsChecked.HasValue)
                {
                    if (tg.IsChecked.Value)
                    {
                        Thickness m = new(20, 1, 20, 30);
                        flyoutWindow.Offset = m;
                        Border.Margin = m;
                    }
                    else
                    {
                        Thickness m = new();
                        flyoutWindow.Offset = m;
                        Border.Margin = m;
                    }
                }
            }
        }
    }
}
