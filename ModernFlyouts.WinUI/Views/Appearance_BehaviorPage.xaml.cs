using System;

using ModernFlyouts.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ModernFlyouts.Views
{
    public sealed partial class Appearance_BehaviorPage : Page
    {
        public Appearance_BehaviorViewModel ViewModel { get; } = new Appearance_BehaviorViewModel();

        public Appearance_BehaviorPage()
        {
            InitializeComponent();
        }
        public static int UpdateUIThemeMethod(string themeName)
        {
            switch (themeName?.ToUpperInvariant())
            {
                case "LIGHT":
                    ShellPage.ShellHandler.RequestedTheme = ElementTheme.Light;
                    break;
                case "DARK":
                    ShellPage.ShellHandler.RequestedTheme = ElementTheme.Dark;
                    break;
                case "SYSTEM":
                    ShellPage.ShellHandler.RequestedTheme = ElementTheme.Default;
                    break;
                default:
                    // TODO WTS: Replace Logger with appcentre logging analysis
                    //Logger.LogError($"Unexpected theme name: {themeName}");
                    break;
            }

            return 0;
        }

        private void OpenColorsSettings_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Helpers.StartProcessHelper.Start(Helpers.StartProcessHelper.ColorsSettings);
        }
    }
}
