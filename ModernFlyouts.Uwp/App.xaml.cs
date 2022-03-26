using Microsoft.Toolkit.Win32.UI.XamlHost;
using ModernFlyouts.Uwp.Helpers;
using System.Threading.Tasks;

namespace ModernFlyouts.Uwp
{
    public sealed partial class App : XamlApplication
    {
        public App()
        {
            Initialize();
            UnhandledException += OnUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedException;
            InitializeComponent();
            JumpListHelper.CreateJumpListAsync();
        }

        private static void OnUnobservedException(object sender, UnobservedTaskExceptionEventArgs e) => e.SetObserved();

        private static void OnUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e) => e.Handled = true;
    }
}