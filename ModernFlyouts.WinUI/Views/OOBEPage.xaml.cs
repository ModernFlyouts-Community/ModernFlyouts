using CommunityToolkit.WinUI.UI.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ViewManagement;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ModernFlyouts.WinUI.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OOBEPage : Page
    {
        Visibility IsMobile;
        public OOBEPage()
        {
            this.InitializeComponent();
            if (Window.Current.Bounds.Width > 830)
            {
                IsMobile = Visibility.Visible;
            }
            else
            {
                IsMobile = Visibility.Collapsed;
            }

            if (Window.Current.Bounds.Height > 700)
            {
                Stepper.Visibility = Visibility.Visible;
                Skipper.Visibility = Visibility.Visible;
            }
            else
            {
                Stepper.Visibility = Visibility.Collapsed;
                Skipper.Visibility = Visibility.Collapsed;
            }

            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
        }
        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = new Frame();
            Window.Current.Content = rootFrame;
            rootFrame.Navigate(typeof(ShellPage));
        }
        private UIElement CreateShell()
        {
            return new Views.ShellPage();
        }
        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Window.Current.Bounds.Width > 830)
            {
                IsMobile = Visibility.Visible;
            }
            else
            {
                IsMobile = Visibility.Collapsed;
            }

            if (Window.Current.Bounds.Height > 700)
            {
                Stepper.Visibility = Visibility.Visible;
                Skipper.Visibility = Visibility.Visible;
            }
            else
            {
                Stepper.Visibility = Visibility.Collapsed;
                Skipper.Visibility = Visibility.Collapsed;
            }
            Bindings.Update();

        }
    }
}
