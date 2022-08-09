using CommunityToolkit.WinUI.UI.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

using Microsoft.UI;
using ModernFlyouts.Helpers;
using Microsoft.UI.Windowing;
using WinUIEx;
using WinRT.Interop;
using Microsoft.UI.Xaml.Controls;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ModernFlyouts.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OOBEPage : Page
    {
        //Visibility IsMobile;
        //private AppWindow appWindow;

        public OOBEPage()
        {
            //SetTitleBar();

            this.InitializeComponent();


            //if (WindowEx.Current.Bounds.Width  > 830)
            //{
            //IsMobile = Visibility.Visible;
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
        }

        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
           // Frame rootFrame = new Frame();
            //Window.Current.Content = rootFrame;
            ShellPage.Navigate(typeof(ShellPage));
        }
        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            //if (WindowEx.Current.Bounds.Width > 830)
            //{
            //IsMobile = Visibility.Visible;
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


    }
}
