using System;
using System.Windows;
using System.Windows.Input;

using Microsoft.Toolkit.Wpf.UI.XamlHost;

using TaskbarFlyout.Uwp;

namespace TaskbarFlyout.Wpf
{
  public partial class FlyoutWindow : Window
  {
    private FlyoutControl _flyoutControl;

    public FlyoutWindow()
    {
      InitializeComponent();
    }

    public void ShowFlyout()
    {
      if (_flyoutControl == null)
      {
        return;
      }

      Activate();

      _flyoutControl.ShowFlyout();
    }

    private void DragWindow(object sender, MouseButtonEventArgs e)
    {
      DragMove();
    }

    private void OnFlyoutControlLoaded(object sender, EventArgs e)
    {
      _flyoutControl = (sender as WindowsXamlHost).Child as FlyoutControl;
    }
  }
}