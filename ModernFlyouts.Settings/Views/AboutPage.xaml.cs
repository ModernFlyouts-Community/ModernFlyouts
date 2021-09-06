using System;

using ModernFlyouts.Settings.ViewModels;

using Windows.UI.Xaml.Controls;

namespace ModernFlyouts.Settings.Views
{
    public sealed partial class AboutPage : Page
    {
        public AboutViewModel ViewModel { get; } = new AboutViewModel();

        public AboutPage()
        {
            InitializeComponent();
        }
    }
}
