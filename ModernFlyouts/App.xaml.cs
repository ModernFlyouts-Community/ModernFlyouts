using System.Windows;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;


namespace ModernFlyouts
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Startup += App_Startup;
            JumpListHelper.CreateJumpList();
        }
        
        private void App_Startup(object sender, StartupEventArgs e)
        {
            FlyoutHandler.Instance = new FlyoutHandler();
            FlyoutHandler.Instance.Initialize();
            AppCenter.Start("26393d67-ab03-4e26-a6db-aa76bf989c21",
                typeof(Analytics), typeof(Crashes));
        }
    }
}
