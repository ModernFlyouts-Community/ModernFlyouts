using System;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace TaskbarFlyout.Uwp
{
  public sealed partial class FlyoutControl : UserControl
  {
    public FlyoutControl()
    {
      InitializeComponent();
    }

    public async void ShowFlyout()
    {
      await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
      {
        FlyoutBase.ShowAttachedFlyout(FlyoutGrid);
      });
    }
  }
}