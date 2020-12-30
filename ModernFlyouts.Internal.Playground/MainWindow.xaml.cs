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
                ZBandID = ZBandID.Desktop,
                Content = myView,
                Alignment = FlyoutWindowAlignment.Bottom | FlyoutWindowAlignment.Right,
                FlyoutWindowType = FlyoutWindowType.OnScreen,
                Margin = new(10),
                Offset = new(20, 1, 20, 30)
            };
            flyoutWindow.IsOpen = true;

            myView.DataContext = flyoutWindow;
            //BandWindow.SetBandWindow(myView, flyoutWindow);

            flyoutWindow.Deactivated += (s, e) => flyoutWindow.IsOpen = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            flyoutWindow.IsOpen = true;
        }
    }
}
