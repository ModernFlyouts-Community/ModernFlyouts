using ModernFlyouts.Helpers;
using System.Windows.Controls;

namespace ModernFlyouts.Navigation
{
    public partial class GeneralSettingsPage : Page
    {
        public GeneralSettingsPage()
        {
            InitializeComponent();
        }

        private void AlignButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            FlyoutHandler.Instance.FlyoutWindow.AlignFlyout();
        }
        
        private void ExitButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            FlyoutHandler.SafelyExitApplication();
        }

        private async void ResetButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            await AppDataHelper.ClearAppDataAsync();
            FlyoutHandler.Instance.UIManager.RestartRequired = true;
        }
    }
}
