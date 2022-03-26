using System;
using System.Collections.Generic;
using System.Text;

namespace ModernFlyouts.Wpf
{
  public class Program
  {
    public const string AppName = "ModernFlyouts";

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