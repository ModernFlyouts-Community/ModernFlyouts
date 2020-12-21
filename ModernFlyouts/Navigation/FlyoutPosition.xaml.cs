using ModernFlyouts.Helpers;
using System.Windows.Controls;

namespace ModernFlyouts.Navigation
{
    public partial class FlyoutPosition : Page
    {
        public FlyoutPosition()
        {
            InitializeComponent();
        }

        private void AlignButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            FlyoutHandler.Instance.FlyoutWindow.AlignFlyout();
        }

        private async void ResetButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            await AppDataHelper.ClearAppDataAsync();
            FlyoutHandler.Instance.UIManager.RestartRequired = true;
        }
    }
}