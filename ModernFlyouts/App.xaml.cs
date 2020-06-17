using System;
using System.Threading;
using System.Windows;

namespace ModernFlyouts
{
    public partial class App : Application
    {
        static Mutex mutex = new Mutex(true, "ModernFlyouts");

        public App()
        {
            Startup += App_Startup;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            if (!mutex.WaitOne(TimeSpan.Zero, true))
            {
                Environment.Exit(0);
                return;
            }

            FlyoutHandler.Instance = new FlyoutHandler();
            FlyoutHandler.Instance.Initialize();
        }
    }
}
