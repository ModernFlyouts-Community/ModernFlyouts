using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using Microsoft.Toolkit.Wpf.UI.XamlHost;
using ModernFlyouts.Settings.Views;
using ModernFlyouts.Wpf.Helpers;
using Windows.ApplicationModel.Resources;
using Windows.Data.Json;
using ModernFlyouts.Settings;


namespace ModernFlyouts.Wpf
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {

        private static Window inst;

        private bool isOpen = true;

        public SettingsWindow()
        {

            this.InitializeComponent();

            Utils.FitToScreen(this);

            //ResourceLoader loader = ResourceLoader.GetForViewIndependentUse();
            //Title = loader.GetString("SettingsWindow_Title");
        }

        public static void CloseHiddenWindow()
        {
            if (inst != null && inst.Visibility == Visibility.Hidden)
            {
                inst.Close();
            }
        }

        public void NavigateToSection(Type type)
        {
            if (inst != null)
            {
                Activate();
                ShellPage.Navigate(type);
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var handle = new WindowInteropHelper(this).Handle;
            NativeMethods.GetWindowPlacement(handle, out var startupPlacement);
            var placement = Utils.DeserializePlacementOrDefault(handle);
            NativeMethods.SetWindowPlacement(handle, ref placement);

            var windowRect = new Rectangle((int)Left, (int)Top, (int)Width, (int)Height);
            var screenRect = new Rectangle((int)SystemParameters.VirtualScreenLeft, (int)SystemParameters.VirtualScreenTop, (int)SystemParameters.VirtualScreenWidth, (int)SystemParameters.VirtualScreenHeight);
            var intersection = Rectangle.Intersect(windowRect, screenRect);

            // Restore default position if 5% of width or height of the window is offscreen
            if (intersection.Width < (Width * 0.95) || intersection.Height < (Height * 0.95))
            {
                NativeMethods.SetWindowPlacement(handle, ref startupPlacement);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            var handle = new WindowInteropHelper(this).Handle;

            Utils.SerializePlacement(handle);
        }

        private void WindowsXamlHost_ChildChanged(object sender, EventArgs e)
        {
            // If sender is null, it could lead to a NullReferenceException. This might occur on restarting as admin (check https://github.com/microsoft/PowerToys/issues/7393 for details)
            if (sender == null)
            {
                return;
            }

            // Hook up x:Bind source.
            WindowsXamlHost windowsXamlHost = sender as WindowsXamlHost;
            ShellPage shellPage = windowsXamlHost.GetUwpInternalObject() as ShellPage;

            if (shellPage != null)
            {
                shellPage.Refresh();
            }

            // XAML Islands: If the window is open, explicitly force it to be shown to solve the blank dialog issue https://github.com/microsoft/PowerToys/issues/3384
            if (isOpen)
            {
                try
                {
                    Show();
                }
                catch (InvalidOperationException)
                {
                }
            }
        }

        private void SettingsWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            isOpen = false;

            // XAML Islands: If the window is closed while minimized, exit the process. Required to avoid process not terminating issue - https://github.com/microsoft/PowerToys/issues/4430
            if (WindowState == WindowState.Minimized)
            {
                // Run Environment.Exit on a separate task to avoid performance impact
                System.Threading.Tasks.Task.Run(() => { Environment.Exit(0); });
            }
        }

        private void SettingsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            inst = (Window)sender;
        }

        private void SettingsWindow_Activated(object sender, EventArgs e)
        {
            if (((Window)sender).Visibility == Visibility.Hidden)
            {
                ((Window)sender).Visibility = Visibility.Visible;
            }
        }
    }
}