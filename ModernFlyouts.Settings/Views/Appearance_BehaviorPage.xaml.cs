using System;

using ModernFlyouts.Settings.ViewModels;

using Windows.UI.Xaml.Controls;

namespace ModernFlyouts.Settings.Views
{
    public sealed partial class Appearance_BehaviorPage : Page
    {
        public Appearance_BehaviorViewModel ViewModel { get; } = new Appearance_BehaviorViewModel();

        public Appearance_BehaviorPage()
        {
            InitializeComponent();
        }

        private void OpenColorsSettings_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Helpers.StartProcessHelper.Start(Helpers.StartProcessHelper.ColorsSettings);
        }
    }
}
