//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AppUIBasics.Common;
using AppUIBasics.Data;
using AppUIBasics.Helper;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.System.Profile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace AppUIBasics
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        
        /// <summary>
        /// Initializes the singleton Application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            this.Resuming += App_Resuming;
            this.RequiresPointerMode = ApplicationRequiresPointerMode.WhenRequested;

            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 6))
            {
                this.FocusVisualKind = AnalyticsInfo.VersionInfo.DeviceFamily == "Xbox" ? FocusVisualKind.Reveal : FocusVisualKind.HighVisibility;
            }
        }

        public void EnableSound(bool withSpatial = false)
        {
            ElementSoundPlayer.State = ElementSoundPlayerState.On;

            if(!withSpatial)
                ElementSoundPlayer.SpatialAudioMode = ElementSpatialAudioMode.Off;
            else
                ElementSoundPlayer.SpatialAudioMode = ElementSpatialAudioMode.On;
        }

        public static TEnum GetEnum<TEnum>(string text) where TEnum : struct
        {
            if (!typeof(TEnum).GetTypeInfo().IsEnum)
            {
                throw new InvalidOperationException("Generic parameter 'TEnum' must be an enum.");
            }
            return (TEnum)Enum.Parse(typeof(TEnum), text);
        }

        private void App_Resuming(object sender, object e)
        {
            switch (NavigationRootPage.RootFrame?.Content)
            {
                case ItemPage itemPage:
                    itemPage.SetInitialVisuals();
                    break;
                case NewControlsPage newControlsPage:
                case AllControlsPage allControlsPage:
                    NavigationRootPage.Current.NavigationView.AlwaysShowHeader = false;
                    break;
            }
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
#if DEBUG
            //if (System.Diagnostics.Debugger.IsAttached)
            //{
            //    this.DebugSettings.EnableFrameRateCounter = true;
            //}

            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.BindingFailed += DebugSettings_BindingFailed;
            }
#endif
            //draw into the title bar
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            
            await EnsureWindow(args);
        }

        private void DebugSettings_BindingFailed(object sender, BindingFailedEventArgs e)
        {
            
        }

        protected async override void OnActivated(IActivatedEventArgs args)
        {
            await EnsureWindow(args);

            base.OnActivated(args);
        }

        private async Task EnsureWindow(IActivatedEventArgs args)
        {
            // No matter what our destination is, we're going to need control data loaded - let's knock that out now.
            // We'll never need to do this again.
            await ControlInfoDataSource.Instance.GetGroupsAsync();

            Frame rootFrame = GetRootFrame();

            ThemeHelper.Initialize();

            Type targetPageType = typeof(NewControlsPage);
            string targetPageArguments = string.Empty;

            if (args.Kind == ActivationKind.Launch)
            {
                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                    }
                    catch (SuspensionManagerException)
                    {
                        //Something went wrong restoring state.
                        //Assume there is no state and continue
                    }
                }

                targetPageArguments = ((LaunchActivatedEventArgs)args).Arguments;
            }
            else if (args.Kind == ActivationKind.Protocol)
            {
                Match match;

                string targetId = string.Empty;

                switch (((ProtocolActivatedEventArgs)args).Uri?.AbsoluteUri)
                {
                    case string s when IsMatching(s, "(/*)category/(.*)"):
                        targetId = match.Groups[2]?.ToString();
                        if (targetId == "AllControls")
                        {
                            targetPageType = typeof(AllControlsPage);
                        }
                        else if (targetId == "NewControls")
                        {
                            targetPageType = typeof(NewControlsPage);
                        }
                        else if (ControlInfoDataSource.Instance.Groups.Any(g => g.UniqueId == targetId))
                        {
                            targetPageType = typeof(SectionPage);
                        }
                        break;

                    case string s when IsMatching(s, "(/*)item/(.*)"):
                        targetId = match.Groups[2]?.ToString();
                        if (ControlInfoDataSource.Instance.Groups.Any(g => g.Items.Any(i => i.UniqueId == targetId)))
                        {
                            targetPageType = typeof(ItemPage);
                        }
                        break;
                }

                targetPageArguments = targetId;

                bool IsMatching(string parent, string expression)
                {
                    match = Regex.Match(parent, expression);
                    return match.Success;
                }
            }

            rootFrame.Navigate(targetPageType, targetPageArguments);

            if (targetPageType == typeof(NewControlsPage))
            {
                ((Microsoft.UI.Xaml.Controls.NavigationViewItem)((NavigationRootPage)Window.Current.Content).NavigationView.MenuItems[0]).IsSelected = true;
            }
            else if (targetPageType == typeof(ItemPage))
            {
                NavigationRootPage.Current.EnsureNavigationSelection(targetPageArguments);
            }

            // Ensure the current window is active
            Window.Current.Activate();
        }

        private Frame GetRootFrame()
        {
            Frame rootFrame;
            if (!(Window.Current.Content is NavigationRootPage rootPage))
            {
                rootPage = new NavigationRootPage();
                rootFrame = (Frame)rootPage.FindName("rootFrame");
                if (rootFrame == null)
                {
                    throw new Exception("Root frame not found");
                }
                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");
                rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];
                rootFrame.NavigationFailed += OnNavigationFailed;

                Window.Current.Content = rootPage;
            }
            else
            {
                rootFrame = (Frame)rootPage.FindName("rootFrame");
            }

            return rootFrame;
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }
    }
}
