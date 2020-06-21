using System.Windows.Controls;

namespace ModernFlyouts.Navigation
{
    public partial class GeneralSettingsPage : Page
    {
        public GeneralSettingsPage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            FlyoutHandler.SafelyExitApplication();
        }
    }
}
