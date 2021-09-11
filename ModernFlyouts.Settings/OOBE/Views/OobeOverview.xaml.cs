// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ModernFlyouts.Settings.OOBE.Enums;
using ModernFlyouts.Settings.OOBE.ViewModel;
using ModernFlyouts.Settings.Views;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ModernFlyouts.Settings.OOBE.Views
{
    public sealed partial class OobeOverview : Page
    {
        public OobePowerToysModule ViewModel { get; set; }

        public OobeOverview()
        {
            this.InitializeComponent();
            ViewModel = new OobePowerToysModule(OobeShellPage.OobeShellHandler.Modules[(int)PowerToysModulesEnum.Overview]);
            DataContext = ViewModel;
        }

        private void SettingsLaunchButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (OobeShellPage.OpenMainWindowCallback != null)
            {
                OobeShellPage.OpenMainWindowCallback(typeof(GeneralPage));
            }

            ViewModel.LogOpeningSettingsEvent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.LogOpeningModuleEvent();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ViewModel.LogClosingModuleEvent();
        }
    }
}
