using System.Diagnostics;
using System.Windows.Controls;
using Windows.ApplicationModel;

namespace ModernFlyouts.Navigation
{
    public partial class AboutPage : Page
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        private void RateAndReviewButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var psi = new ProcessStartInfo()
            {
                FileName = "ms-windows-store://review/?PFM=" + Package.Current.Id.FamilyName,
                UseShellExecute = true
            };
            Process.Start(psi);
        }
    }
}