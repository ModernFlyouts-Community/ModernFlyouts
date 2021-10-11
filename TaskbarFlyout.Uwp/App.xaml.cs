using Microsoft.Toolkit.Win32.UI.XamlHost;

namespace TaskbarFlyout.Uwp
{
  public sealed partial class App : XamlApplication
  {
    public App()
    {
      Initialize();
      InitializeComponent();
    }
  }
}