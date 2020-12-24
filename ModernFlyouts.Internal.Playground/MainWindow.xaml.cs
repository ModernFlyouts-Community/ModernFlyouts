using ModernFlyouts.Core.Interop;
using ModernFlyouts.Core.UI;
using System.Windows;

namespace ModernFlyouts.Internal.Playground
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        FlyoutWindow flyoutWindow;

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var myView = new MyView();
            flyoutWindow = new FlyoutWindow()
            {
                Activatable = true,
                ZBandID = ZBandID.Default,
                Content = myView,
                Alignment = FlyoutWindowAlignment.Bottom | FlyoutWindowAlignment.Right,
                Margin = new(10),
                FlyoutWindowType = FlyoutWindowType.Tray,
                Offset = new(20, 1, 20, 30)
            };
            flyoutWindow.Show();

            myView.DataContext = flyoutWindow;

            flyoutWindow.Deactivated += (s, e) => flyoutWindow.Hide();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            flyoutWindow.Show();
        }
    }
}
