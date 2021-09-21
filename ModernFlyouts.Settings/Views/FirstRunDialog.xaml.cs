using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ModernFlyouts.Settings.Views
{
    public sealed partial class FirstRunDialog : ContentDialog
    {
        public FirstRunDialog()
        {
            // TODO WTS: Update the contents of this dialog with any important information you want to show when the app is used for the first time.
            RequestedTheme = (Window.Current.Content as FrameworkElement).RequestedTheme;
            InitializeComponent();
        }
        private void OOBE_Closed_Click(object sender, object e)
        {
            this.Hide();
        }
    }
}
