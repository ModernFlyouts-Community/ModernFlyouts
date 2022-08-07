using Microsoft.UI.Windowing;

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

using Microsoft.UI;
using Windows.ApplicationModel.Resources;

using WinRT.Interop;
using WinUIEx;
using Windows.UI;
using WindowId = Microsoft.UI.WindowId;
using Microsoft.UI.Xaml;
using ModernFlyouts;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ModernFlyouts
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsWindow : WinUIEx.WindowEx
    {
        private AppWindow appWindow;

        public SettingsWindow()
        {
            ShellPage.SetElevationStatus(App.IsElevated);
            ShellPage.SetIsUserAnAdmin(App.IsUserAnAdmin);

            SetTitleBar();

            this.InitializeComponent();

        }

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
            ShellPage.Navigate(typeof(OOBEPage));
            //App.SettingsRoot.Navigate(typeof(OOBEPage));
        }

        public string GetAppTitleFromSystem()
        {
            return Windows.ApplicationModel.Package.Current.DisplayName;
        }





    }
}
