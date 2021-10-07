using System;
using System.Windows;
using System.Windows.Input;

using Microsoft.Toolkit.Wpf.UI.XamlHost;

using TaskbarFlyout.Uwp;

using Windows.UI.Xaml.Controls;

namespace TaskbarFlyout.Wpf
{
  public partial class MainWindow : Window
  {
    private readonly FlyoutWindow _flyoutWindow;

    public MainWindow()
    {
      InitializeComponent();

      _flyoutWindow = new FlyoutWindow();
      _flyoutWindow.Show();
    }

    private void DragWindow(object sender, MouseButtonEventArgs e)
    {
      DragMove();
    }

    private void OnCloseButtonControlLoaded(object sender, EventArgs e)
    {
      var control = (sender as WindowsXamlHost).Child as CloseButtonControl;

      if (control == null)
      {
        return;
      }

      (control.Content as Button).Click += (_, __) =>
      {
        Application.Current.Shutdown();
      };
    }

    private void OnMainControlLoaded(object sender, EventArgs e)
    {
      var control = (sender as WindowsXamlHost).Child as MainControl;

      if (control == null)
      {
        return;
      }

      control.ShowHideFlyoutWindowButton.Click += (_, __) =>
      {
        var isChecked = (bool)control.ShowHideFlyoutWindowButton.IsChecked;
        _flyoutWindow.Visibility = isChecked ? Visibility.Visible : Visibility.Hidden;
      };

      control.ShowFlyoutButton.Click += (_, __) =>
      {
        _flyoutWindow.ShowFlyout();
      };

      control.CenterFlyoutWindowButton.Click += (_, __) =>
      {
        var screenWidth = SystemParameters.PrimaryScreenWidth;
        var screenHeight = SystemParameters.PrimaryScreenHeight;
        var windowWidth = _flyoutWindow.Width;
        var windowHeight = _flyoutWindow.Height;

        _flyoutWindow.Left = (screenWidth / 2) - (windowWidth / 2);
        _flyoutWindow.Top = (screenHeight / 2) - (windowHeight / 2);
      };
    }
  }
}