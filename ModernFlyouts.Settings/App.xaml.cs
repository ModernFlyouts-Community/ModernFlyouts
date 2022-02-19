using System;
using System.Globalization;
using System.Reflection;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

using ModernFlyouts.Settings.Services;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.ViewManagement;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using ModernFlyouts.Settings.Views;
using System.Threading.Tasks;
using System.Runtime.ExceptionServices;
using Microsoft.Toolkit.Uwp.Helpers;

namespace ModernFlyouts.Settings
{
    public sealed partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // TODO WTS: Add your app in the app center and set your secret here. More at https://docs.microsoft.com/appcenter/sdk/getting-started/uwp
            AppCenter.Start("26393d67-ab03-4e26-a6db-aa76bf989c21",
                   typeof(Analytics), typeof(Crashes));

            UnhandledException += OnUnhandledException;

            TaskScheduler.UnobservedTaskException += OnUnobservedException;
            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
        }

        private static void OnUnobservedException(object sender, UnobservedTaskExceptionEventArgs e) => e.SetObserved();

        private static void OnUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e) => e.Handled = true;

        private void CurrentDomain_FirstChanceException(object sender, FirstChanceExceptionEventArgs e)
        {
            //ADD APPCENTER HANDLING HERE AND TO METHODS ABOVE
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    if (SystemInformation.Instance.IsFirstRun == true)
                    {
                        rootFrame.Navigate(typeof(ShellPage), e.Arguments);
                    }
                    else
                    {
                        rootFrame.Navigate(typeof(ShellPage), e.Arguments);
                    }
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }
    }
}
