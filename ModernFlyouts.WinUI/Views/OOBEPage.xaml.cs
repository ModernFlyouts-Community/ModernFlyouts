using CommunityToolkit.WinUI.UI.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

using Microsoft.UI;
using ModernFlyouts.WinUI.Helpers;
using Microsoft.UI.Windowing;
using WinUIEx;
using WinRT.Interop;
using Microsoft.UI.Xaml.Controls;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ModernFlyouts.WinUI.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OOBEPage : Page
    {
        Visibility IsMobile;
        private AppWindow appWindow;

        public OOBEPage()
        {
            //SetTitleBar();

            this.InitializeComponent();


            //if (WindowEx.Current.Bounds.Width  > 830)
            //{
            IsMobile = Visibility.Visible;
            //}
            //else
            //{
            //    IsMobile = Visibility.Collapsed;
            //}

            //if (WindowEx.Current.Bounds.Height > 700)
            //{
            Stepper.Visibility = Visibility.Visible;
            Skipper.Visibility = Visibility.Visible;
            //}
            //else
            //{
            //    Stepper.Visibility = Visibility.Collapsed;
            //    Skipper.Visibility = Visibility.Collapsed;
            //}

            //var currentWindow = WindowHelper.GetWindowForElement(this);
            //currentWindow.ExtendsContentIntoTitleBar = true;
            //currentWindow.SetTitleBar(CustomDragRegion);
            //CustomDragRegion.MinWidth = 188;

            //var currentWindow = WindowHelper.GetWindowForElement(this);
            //currentWindow.ExtendsContentIntoTitleBar = true;

            //var coreTitleBar = WindowHelper.GetCurrentView().TitleBar;
            //coreTitleBar.ExtendViewIntoTitleBar = true;
            //CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            // AppWindowTitleBar.ButtonBackgroundColor = Colors.Transparent;
        }

        //private AppWindow GetAppWindowForCurrentWindow()
        //{
        //    IntPtr hWnd = WindowNative.GetWindowHandle(this);
        //    WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
        //    return AppWindow.GetFromWindowId(wndId);
        //}

        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {

            //Frame rootFrame = new Frame();
            //var currentWindow = WindowHelper.GetWindowForElement(this);
            //Window.Current.Content = rootFrame;
            //rootFrame.Navigate(typeof(ShellPage));
        }
        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            //if (WindowEx.Current.Bounds.Width > 830)
            //{
                IsMobile = Visibility.Visible;
            //}
            //else
            //{
            //    IsMobile = Visibility.Collapsed;
            //}

            //if (WindowEx.Current.Bounds.Height > 700)
            //{
                Stepper.Visibility = Visibility.Visible;
                Skipper.Visibility = Visibility.Visible;
            //}
            //else
            //{
            //    Stepper.Visibility = Visibility.Collapsed;
            //    Skipper.Visibility = Visibility.Collapsed;
            //}
            Bindings.Update();

        }

        //private void SetTitleBar()
        //{
        //    // Set window icon
        //    appWindow = GetAppWindowForCurrentWindow();

        //    // Only works on Windows 11 for now
        //    if (AppWindowTitleBar.IsCustomizationSupported())
        //    {
        //        //var titlebar = appWindow.TitleBar;
        //        //titlebar.ExtendsContentIntoTitleBar = true;
        //        //titlebar.ButtonBackgroundColor = (Color)App.Current.Resources["SolidBackgroundFillColorBase"];
        //        //titlebar.ButtonInactiveBackgroundColor = (Color)App.Current.Resources["SolidBackgroundFillColorBase"];
        //        //titlebar.IconShowOptions = IconShowOptions.HideIconAndSystemMenu;

        //        appWindow.TitleBar.ExtendsContentIntoTitleBar = true;
        //        appWindow.TitleBar.IconShowOptions = IconShowOptions.HideIconAndSystemMenu;
        //        var transparent = Windows.UI.Color.FromArgb(0, 0, 0, 0);
        //        appWindow.TitleBar.BackgroundColor = transparent;
        //        appWindow.TitleBar.ButtonBackgroundColor = transparent;
        //        appWindow.TitleBar.InactiveBackgroundColor = transparent;
        //        appWindow.TitleBar.ButtonInactiveBackgroundColor = transparent;
        //    }
        //    else
        //    {
        //        //Window.ExtendsContentIntoTitleBar = true;
        //        //// Windows 10 fallback: stick to default titlebar
        //        ////appWindow.SetIcon("icon.ico");
        //        ////ResourceLoader loader = ResourceLoader.GetForViewIndependentUse();
        //        //Title = loader.GetString("SettingsWindow_Title");
        //        //AppWindowTitleBar.Visibility = Visibility.Collapsed;
        //    }
        //}



    }
}
