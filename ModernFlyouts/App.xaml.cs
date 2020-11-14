using System.Windows;


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
        }
    }
}
