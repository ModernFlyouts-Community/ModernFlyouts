using System;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ModernFlyouts.WinUI.Views
{
    public sealed partial class FirstRunDialog : ContentDialog
    {
        public FirstRunDialog()
        {
            // TODO WTS: Update the contents of this dialog with any important information you want to show when the app is used for the first time.
            RequestedTheme = (Window.Current.Content as FrameworkElement).RequestedTheme;
            InitializeComponent();
        }
        private void OOBE_Closed_Click(object sender, object e)
        {
            this.Hide();
        }


        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            FlipViewControl.SelectedIndex = FlipViewControl.SelectedIndex + 1;
        }

        private void PreviousBtn_Click(object sender, RoutedEventArgs e)
        {
            FlipViewControl.SelectedIndex = FlipViewControl.SelectedIndex - 1;
        }

        private void FlipViewControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NextBtn.IsEnabled = true;
            PreviousBtn.IsEnabled = true;
            if (FlipViewControl.SelectedIndex == 0)
            {
                PreviousBtn.IsEnabled = false;
            }

            if (FlipViewControl.SelectedIndex == 3)
            {
                NextBtn.IsEnabled = false;
            }
        }

        //If decide to change to window/ page
        //private async Task<bool> OpenPageAsWindowAsync(Type t)
        //{
        //    CoreApplicationView view = CoreApplication.CreateNewView();
        //    int id = 0;

        //    await view.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
        //    {
        //        var frame = new Frame();
        //        frame.Navigate(t, null);
        //        Window.Current.Content = frame;
        //        Window.Current.Activate();
        //        view.TitleBar.ExtendViewIntoTitleBar = true;

        //        ApplicationView appView = ApplicationView.GetForCurrentView();
        //        appView.TitleBar.BackgroundColor = Colors.Transparent;
        //        appView.TitleBar.ButtonBackgroundColor = Colors.Transparent;
        //        appView.TitleBar.ButtonForegroundColor = Colors.Black;
        //        //appView.TitleBar.ButtonForegroundColor = ((SolidColorBrush)Application.Current.Resources["PrimaryTextColor"]).Color;
        //        appView.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
        //        appView.TitleBar.InactiveBackgroundColor = Colors.Transparent;
        //        appView.TitleBar.ForegroundColor = Colors.Black;
        //        id = ApplicationView.GetForCurrentView().Id;
        //    });

        //    return await ApplicationViewSwitcher.TryShowAsStandaloneAsync(id);
        //}


    }
}
