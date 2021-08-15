//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using Settings.UWP.Common;
using Settings.UWP.Helper;
using System.Linq;
using Windows.System;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace AppUIBasics
{
    /// <summary>
    /// A page that displays the app's settings.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public string Version
        {
            get
            {
                var version = Windows.ApplicationModel.Package.Current.Id.Version;
                return string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
            }
        }

        public SettingsPage()
        {
            this.InitializeComponent();
            Loaded += OnSettingsPageLoaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            NavigationRootPage.Current.NavigationView.Header = "Settings";
        }

        private void OnSettingsPageLoaded(object sender, RoutedEventArgs e)
        {
            var currentTheme = ThemeHelper.RootTheme.ToString();
            (ThemePanel.Children.Cast<RadioButton>().FirstOrDefault(c => c?.Tag?.ToString() == currentTheme)).IsChecked = true;
        }

        private void OnThemeRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            var selectedTheme = ((RadioButton)sender)?.Tag?.ToString();
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;

            if (selectedTheme != null)
            {
                ThemeHelper.RootTheme = App.GetEnum<ElementTheme>(selectedTheme);
            }
        }

        private void OnThemeRadioButtonKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Up)
            {
                NavigationRootPage.Current.PageHeader.Focus(FocusState.Programmatic);
            }
        }
    }
}
