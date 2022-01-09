using Microsoft.Toolkit.Win32.UI.XamlHost;
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
        }

        private static void OnUnobservedException(object sender, UnobservedTaskExceptionEventArgs e) => e.SetObserved();

        private static void OnUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e) => e.Handled = true;
    }
}