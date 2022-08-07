using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ModernFlyouts.Services;
using ModernFlyouts.ViewModels;
using Windows.Data.Json;
using Microsoft.UI.Xaml.Automation.Peers;
using CommunityToolkit.Mvvm.Input;

namespace ModernFlyouts.Views
{
    // TODO WTS: Change the icons and titles for all NavigationViewItems in ShellPage.xaml.
    public sealed partial class ShellPage : UserControl
    {
        /// <summary>
        /// Gets or sets a shell handler to be used to update contents of the shell dynamically from page within the frame.
        /// </summary>
        public static ShellPage ShellHandler { get; set; }

        /// <summary>
        /// Gets view model.
        /// </summary>
        public ShellViewModel ViewModel { get; } = new ShellViewModel();

        //private readonly IWindowManager _windowManager;

        public static bool IsElevated { get; set; }

        public static bool IsUserAnAdmin { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellPage"/> class.
        /// Shell page constructor.
        /// </summary>
        public ShellPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
            ShellHandler = this;
            ViewModel.Initialize(shellFrame, navigationView, KeyboardAccelerators);
            shellFrame.Navigate(typeof(GeneralSettings));

        }


        ////////OOBE////

        private void OOBEItem_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
           // NavigateToOOBESection();
            // //Frame rootFrame = new Frame();
            // var window = (Application.Current as App)?.Window as SettingsWindow;
            //////Window.Current.Content = rootFrame;
            //SettingsWindow.rootFrame.Navigate(typeof(OOBEPage));
            Navigate(typeof(OOBEPage));
        }

        //private void OOBEItem_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        //{
        //    var OOBEdialog = new TeachingTip();

        //    //OOBEdialog.XamlRoot = this.XamlRoot;
        //    //OOBEdialog.MinHeight = this.ActualWidth;
        //    OOBEdialog.PreferredPlacement = TeachingTipPlacementMode.Center;
        //    //OOBEdialog.MinWidth = this.ActualWidth;
        //    //OOBEdialog.Content = new OOBEPage();
        //    OOBEdialog.Content = "test";
        //    OOBEdialog.IsOpen = true;

        //    // _ = await OOBEdialog.ShowAsync();

        //}

        private async void WhatsNew_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            ContentDialog WhatsNewdialog = new ContentDialog();

            WhatsNewdialog.XamlRoot = this.XamlRoot;
            WhatsNewdialog.Title = "Whats New in this Version";
            WhatsNewdialog.CloseButtonText = "Close";
            WhatsNewdialog.DefaultButton = ContentDialogButton.Close;
            WhatsNewdialog.Content = new MarkdownContentDialog(await AssetsHelper.GetReleaseNoteAsync());
            //new WhatsNewDialog();
            _ = await WhatsNewdialog.ShowAsync();
        }



        //#region PrivacyPolicyCommand

        //internal IAsyncRelayCommand PrivacyPolicyCommand { get; }

        //private async Task ExecutePrivacyPolicyCommandAsync()
        //{
        //    await _windowManager.ShowContentDialogAsync(
        //        new MarkdownContentDialog(
        //            await AssetsHelper.GetPrivacyStatementAsync()),
        //        Strings.Close,
        //        title: Strings.PrivacyPolicy);
        //}

        //#endregion

        public static void SetElevationStatus(bool isElevated)
        {
            IsElevated = isElevated;
        }

        public static void SetIsUserAnAdmin(bool isAdmin)
        {
            IsUserAnAdmin = isAdmin;
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


        //private async void FeedbackItem_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        //{
        //    await Windows.System.Launcher.LaunchUriAsync(new Uri("https://aka.ms/ModernFlyoutsGiveFeedback"));
        //}


    }
}
