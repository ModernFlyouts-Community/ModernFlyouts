using System;

namespace ModernFlyouts.Wpf
{
  public class Program
  {
    [STAThread]
    public static void Main()
    {
      using (new ModernFlyouts.Uwp.App())
      {
        var app = new App();
        app.InitializeComponent();
        app.Run();
      }
    }
  }
}