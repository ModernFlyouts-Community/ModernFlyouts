using Microsoft.UI.Xaml;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

using ModernFlyouts.WinUI.Helpers;
using ModernFlyouts.WinUI.Views;
using System.Threading.Tasks;
using ModernFlyouts.WinUI.Services;

using Microsoft.UI;
using Windows.ApplicationModel.Resources;

using WinRT.Interop;
using WinUIEx;
using Windows.UI;
using WindowId = Microsoft.UI.WindowId;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ModernFlyouts.WinUI
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

            this.InitializeComponent();

            SetTitleBar();
        }

        private void SetTitleBar()
        {
            // Set window icon
            appWindow = GetAppWindowForCurrentWindow();

            // Only works on Windows 11 for now
            if (AppWindowTitleBar.IsCustomizationSupported())
            {
                var titlebar = appWindow.TitleBar;
                titlebar.ExtendsContentIntoTitleBar = true;
                titlebar.ButtonBackgroundColor = (Color)App.Current.Resources["SolidBackgroundFillColorBase"];
                titlebar.ButtonInactiveBackgroundColor = (Color)App.Current.Resources["SolidBackgroundFillColorBase"];
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

        public string GetAppTitleFromSystem()
        {
            return Windows.ApplicationModel.Package.Current.DisplayName;
        }

    }
}
