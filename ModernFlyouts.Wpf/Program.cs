using System;
using System.Collections.Generic;
using System.Text;

namespace ModernFlyouts.Wpf
{
  public class Program
  {
    public const string AppName = "ModernFlyouts";

    [System.STAThreadAttribute()]
    public static void Main()
    {
        using (new ModernFlyouts.Settings.App())
        {
            ModernFlyouts.Wpf.App app = new ModernFlyouts.Wpf.App();
            app.InitializeComponent();
            app.Run();
        }
    }

    //    [STAThread]
    //public static void Main()
    //{
    //  using (new ModernFlyouts.Uwp.App())
    //  {
    //    var app = new App();
    //    app.InitializeComponent();
    //    app.Run();
    //  }
    //}
  }
}