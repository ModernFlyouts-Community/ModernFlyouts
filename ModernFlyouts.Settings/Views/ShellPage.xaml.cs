using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ModernFlyouts.Settings.Services;
using ModernFlyouts.Settings.ViewModels;
using Windows.Data.Json;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using WinUI = Microsoft.UI.Xaml.Controls;



namespace ModernFlyouts.Settings.Views
{
    // TODO WTS: Change the icons and titles for all NavigationViewItems in ShellPage.xaml.
    public sealed partial class ShellPage : UserControl
    {
        /// <summary>
        /// Gets or sets a shell handler to be used to update contents of the shell dynamically from page within the frame.
        /// </summary>
        public static ShellPage ShellHandler { get; set; }

        public ShellViewModel ViewModel { get; } = new ShellViewModel();


        public ShellPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
            ShellHandler = this;
            ViewModel.Initialize(shellFrame, navigationView, KeyboardAccelerators);
            shellFrame.Navigate(typeof(GeneralSettings));

            //var titleBar = ApplicationView.GetForCurrentView().TitleBar;

            //titleBar.ButtonBackgroundColor = Colors.Transparent;
            //titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            //// Hide default title bar.
            //var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            //coreTitleBar.ExtendViewIntoTitleBar = true;
            //UpdateTitleBarLayout(coreTitleBar);

            //// Set XAML element as a draggable region.
            //Window.Current.SetTitleBar(AppTitleBar);

            //// Register a handler for when the size of the overlaid caption control changes.
            //// For example, when the app moves to a screen with a different DPI.
            //coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;

            //// Register a handler for when the title bar visibility changes.
            //// For example, when the title bar is invoked in full screen mode.
            //coreTitleBar.IsVisibleChanged += CoreTitleBar_IsVisibleChanged;

            ////Register a handler for when the window changes focus
            //Window.Current.Activated += Current_Activated;
        }

        //private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        //{
        //    UpdateTitleBarLayout(sender);
        //}

        //private void UpdateTitleBarLayout(CoreApplicationViewTitleBar coreTitleBar)
        //{
        //    // Update title bar control size as needed to account for system size changes.
        //    AppTitleBar.Height = coreTitleBar.Height;

        //    // Ensure the custom title bar does not overlap window caption controls
        //    Thickness currMargin = AppTitleBar.Margin;
        //    AppTitleBar.Margin = new Thickness(currMargin.Left, currMargin.Top, coreTitleBar.SystemOverlayRightInset, currMargin.Bottom);
        //}

        //        private void CoreTitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        //        {
        //            if (sender.IsVisible)
        //            {
        //                AppTitleBar.Visibility = Visibility.Visible;
        //            }
        //            else
        //            {
        //                AppTitleBar.Visibility = Visibility.Collapsed;
        //            }
        //        }

        //        // Update the TitleBar based on the inactive/active state of the app
        //        private void Current_Activated(object sender, Windows.UI.Core.WindowActivatedEventArgs e)
        //        {
        //            SolidColorBrush defaultForegroundBrush = (SolidColorBrush)Application.Current.Resources["TextFillColorPrimaryBrush"];
        //            SolidColorBrush inactiveForegroundBrush = (SolidColorBrush)Application.Current.Resources["TextFillColorDisabledBrush"];

        //            if (e.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
        //            {
        //                AppTitle.Foreground = inactiveForegroundBrush;
        //            }
        //            else
        //            {
        //                AppTitle.Foreground = defaultForegroundBrush;
        //            }
        //        }

        //        // Update the TitleBar content layout depending on NavigationView DisplayMode
        //        private void NavigationViewControl_DisplayModeChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewDisplayModeChangedEventArgs args)
        //        {
        //#pragma warning disable CS0219 // Variable is assigned but its value is never used
        //            const int topIndent = 16;
        //#pragma warning restore CS0219 // Variable is assigned but its value is never used
        //            const int expandedIndent = 48;
        //            int minimalIndent = 104;

        //            // If the back button is not visible, reduce the TitleBar content indent.
        //            if (navigationView.IsBackButtonVisible.Equals(Microsoft.UI.Xaml.Controls.NavigationViewBackButtonVisible.Collapsed))
        //            {
        //                minimalIndent = 48;
        //            }

        //            Thickness currMargin = AppTitleBar.Margin;

        //            if (sender.DisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Minimal)
        //            {
        //                AppTitleBar.Margin = new Thickness(minimalIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
        //            }
        //            else
        //            {
        //                AppTitleBar.Margin = new Thickness(expandedIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
        //            }
        //        }

        //        void UpdateAppTitle(CoreApplicationViewTitleBar coreTitleBar)
        //        {
        //            //ensure the custom title bar does not overlap window caption controls
        //            Thickness currMargin = AppTitleBar.Margin;
        //            AppTitleBar.Margin = new Thickness(currMargin.Left, currMargin.Top, coreTitleBar.SystemOverlayRightInset, currMargin.Bottom);
        //        }

        //        public string GetAppTitleFromSystem()
        //        {
        //            return Windows.ApplicationModel.Package.Current.DisplayName;
        //        }


        ////////OOBE////

        private void OOBEItem_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Frame rootFrame = new Frame();
            Window.Current.Content = rootFrame;
            rootFrame.Navigate(typeof(OOBEPage));
        }

        ///Temporary Whats New trigger
        private async void WhatsNew_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var dialog = new WhatsNewDialog();
            await dialog.ShowAsync();
        }



        public static void Navigate(Type type)
        {
            NavigationService.Navigate(type);
        }

        public void Refresh()
        {
            shellFrame.Navigate(typeof(GeneralSettings));
        }

        private bool navigationViewInitialStateProcessed; // avoid announcing initial state of the navigation pane.

        [SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Params are required for event handler signature requirements.")]
#pragma warning disable CA1822 // Mark members as static
        private void NavigationView_PaneOpened(Microsoft.UI.Xaml.Controls.NavigationView sender, object args)
        {
            if (!navigationViewInitialStateProcessed)
            {
                navigationViewInitialStateProcessed = true;
                return;
            }

            var peer = FrameworkElementAutomationPeer.FromElement(sender);
            if (peer == null)
            {
                peer = FrameworkElementAutomationPeer.CreatePeerForElement(sender);
            }

            if (AutomationPeer.ListenerExists(AutomationEvents.MenuOpened))
            {
                var loader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
                peer.RaiseNotificationEvent(
                    AutomationNotificationKind.ActionCompleted,
                    AutomationNotificationProcessing.ImportantMostRecent,
                    loader.GetString("Shell_NavigationMenu_Announce_Open"),
                    "navigationMenuPaneOpened");
            }
        }

        [SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Params are required for event handler signature requirements.")]
        private void NavigationView_PaneClosed(Microsoft.UI.Xaml.Controls.NavigationView sender, object args)
        {
            if (!navigationViewInitialStateProcessed)
            {
                navigationViewInitialStateProcessed = true;
                return;
            }

            var peer = FrameworkElementAutomationPeer.FromElement(sender);
            if (peer == null)
            {
                peer = FrameworkElementAutomationPeer.CreatePeerForElement(sender);
            }

            if (AutomationPeer.ListenerExists(AutomationEvents.MenuClosed))
            {
                var loader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
                peer.RaiseNotificationEvent(
                    AutomationNotificationKind.ActionCompleted,
                    AutomationNotificationProcessing.ImportantMostRecent,
                    loader.GetString("Shell_NavigationMenu_Announce_Collapse"),
                    "navigationMenuPaneClosed");
            }
        }


        //private async void FeedbackItem_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        //{
        //    await Windows.System.Launcher.LaunchUriAsync(new Uri("https://aka.ms/ModernFlyoutsGiveFeedback"));
        //}


        public string GetAppTitleFromSystem()
        {
            return Windows.ApplicationModel.Package.Current.DisplayName;
        }







    }
}
