using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

using ModernFlyouts.Helpers;
using ModernFlyouts.Views;
using System.Threading.Tasks;
using ModernFlyouts.Services;

using Windows.ApplicationModel.Resources;

using WinRT.Interop;
using WinUIEx;
using Windows.UI;
using WindowId = Microsoft.UI.WindowId;
using ModernFlyouts;

using H.NotifyIcon;
using Microsoft.UI;
using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System.Diagnostics;
using Windows.Win32;
using WinRT;


namespace ModernFlyouts
{
    /// <summary>
    /// The Settings window of ModernFlyouts.
    /// </summary>
    public sealed partial class SettingsWindow : WinUIEx.WindowEx, IDisposable
    {
        public static XamlRoot XamlRoot { get => _instance.windowRoot.XamlRoot; }
        public static DispatcherQueue WindowDispatcher { get => _instance.DispatcherQueue; }
        public static string AppTitleDisplayName { get => Windows.ApplicationModel.Package.Current.DisplayName; }

        //private readonly ISettingsService _settingsService;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private static SettingsWindow _instance;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private IntPtr hWnd;

        private AppWindow appWindow;

        // private readonly WindowsSystemDispatcherQueueHelper _wsdqHelper;

        public SettingsWindow() : base()
        {
            InitializeComponent();
            _instance = this;
            //_settingsService = settingsService;
            //_oobeCompleted = settingsService.GetValue<bool>(settingsService.KeyOobeIsCompleted);
            Title = AppTitleDisplayName;

            // Hide default title bar.
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(AppTitleBar); // Set XAML element as a draggable region.

            //Register a handler for when the window changes focus
            Activated += SettingsWindow_Activated;

            // AppWindow Interop
            hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
            AppWindow appWindow = AppWindow.GetFromWindowId(windowId);
            appWindow.Closing += AppWindow_Closing;

            // Resize the window 
            uint dpi = PInvoke.GetDpiForWindow((Windows.Win32.Foundation.HWND)hWnd);
            double factor = dpi / 96d;
            appWindow.Resize(new Windows.Graphics.SizeInt32(Convert.ToInt32(1200 * factor), Convert.ToInt32(800 * factor)));



            ShellPage.SetElevationStatus(App.IsElevated);
            ShellPage.SetIsUserAnAdmin(App.IsUserAnAdmin);

            //SetTitleBar();




            // if (_oobeCompleted == true)
            //{
            //rootFrame.Navigate(typeof(ShellPage));
            //}
            //else
            //{
                 // rootFrame.Navigate(typeof(OobePage));
            //}
        }

        /// <summary>
        /// ////////////////////////////////////////////
        /// </summary>
        /// <returns></returns>
        /// 


        private void SetTitleBar()
        {
            // Set window icon
            appWindow = GetAppWindowForCurrentWindow();

            // Only works on Windows 11 for now
            if (AppWindowTitleBar.IsCustomizationSupported())
            {
                //var titlebar = appWindow.TitleBar;
                //titlebar.ExtendsContentIntoTitleBar = true;
                //titlebar.ButtonBackgroundColor = (Color)App.Current.Resources["SolidBackgroundFillColorBase"];
                //titlebar.ButtonInactiveBackgroundColor = (Color)App.Current.Resources["SolidBackgroundFillColorBase"];
                //titlebar.IconShowOptions = IconShowOptions.HideIconAndSystemMenu;

                appWindow.TitleBar.ExtendsContentIntoTitleBar = true;
                appWindow.TitleBar.IconShowOptions = IconShowOptions.HideIconAndSystemMenu;
                var transparent = Windows.UI.Color.FromArgb(0, 0, 0, 0);
                appWindow.TitleBar.BackgroundColor = transparent;
                appWindow.TitleBar.ButtonBackgroundColor = transparent;
                appWindow.TitleBar.InactiveBackgroundColor = transparent;
                appWindow.TitleBar.ButtonInactiveBackgroundColor = transparent;
            }
            else
            {
                //Window.ExtendsContentIntoTitleBar = true;
                //// Windows 10 fallback: stick to default titlebar
                ////appWindow.SetIcon("icon.ico");
                ////ResourceLoader loader = ResourceLoader.GetForViewIndependentUse();
                //Title = loader.GetString("SettingsWindow_Title");
                AppTitleBar.Visibility = Visibility.Collapsed;
            }
        }




        private AppWindow GetAppWindowForCurrentWindow()
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            return AppWindow.GetFromWindowId(wndId);
        }

        public void NavigateToSection(System.Type type)
        {
            ShellPage.Navigate(type);
        }

        public void NavigateToOOBESection(object sender, RoutedEventArgs e)
        {
            //ShellPage.Navigate(typeof(OOBEPage));
            //App.SettingsRoot.Navigate(typeof(OOBEPage));
        }

        public string GetAppTitleFromSystem()
        {
            return Windows.ApplicationModel.Package.Current.DisplayName;
        }


        /// ///////////////////////////////

        private void SettingsWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
        }

        private void AppWindow_Closing(AppWindow sender, AppWindowClosingEventArgs args)
        {
            this.Hide(enableEfficiencyMode: true);
            args.Cancel = true;
        }

        private void OpenAppCommand_ExecuteRequested(Microsoft.UI.Xaml.Input.XamlUICommand sender, Microsoft.UI.Xaml.Input.ExecuteRequestedEventArgs args)
        {
            this.Show(disableEfficiencyMode: true);
            PInvoke.SetForegroundWindow((Windows.Win32.Foundation.HWND)hWnd);
        }

        private void ExitCommand_ExecuteRequested(Microsoft.UI.Xaml.Input.XamlUICommand sender, Microsoft.UI.Xaml.Input.ExecuteRequestedEventArgs args)
        {
            Close();
        }

        public void Dispose()
        {
        }
    }
}
