using System.Windows;

namespace ModernFlyouts.Internal.Playground
{
    public partial class App : Application
    {
        public App()
        {
            Startup += App_Startup;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            var m = new MainWindow();
            m.Show();
        }
    }
}
