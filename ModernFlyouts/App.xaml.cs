using System;
using System.IO.Pipes;
using System.Threading;
using System.Windows;

namespace ModernFlyouts
{
    public partial class App : Application, ISingleInstanceApp
    {
        public const string AppName = "ModernFlyouts";

        private static Mutex mutex = new Mutex(true, AppName);

        public App()
        {
            Startup += App_Startup;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                CreateRemoteService(AppName);

                FlyoutHandler.Instance = new FlyoutHandler();
                FlyoutHandler.Instance.Initialize();
            }
            else
            {
                SignalFirstInstance(AppName);
            }
        }

        private static async void CreateRemoteService(string channelName)
        {
            using NamedPipeServerStream pipeServer = new NamedPipeServerStream(channelName, PipeDirection.In);
            while (true)
            {
                await pipeServer.WaitForConnectionAsync().ConfigureAwait(false);
                if (Current != null)
                {
                    Current.Dispatcher.Invoke(() =>
                    {
                        if (Current is ISingleInstanceApp singleInstanceApp)
                        {
                            singleInstanceApp.OnSecondAppStarted();
                        }
                    });
                }

                pipeServer.Disconnect();
            }
        }

        private static async void SignalFirstInstance(string channelName)
        {
            using NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", channelName, PipeDirection.Out);
            await pipeClient.ConnectAsync(0).ConfigureAwait(false);
            Environment.Exit(0);
        }

        public void OnSecondAppStarted()
        {
            FlyoutHandler.ShowSettingsWindow();
        }
    }

    internal interface ISingleInstanceApp
    {
        void OnSecondAppStarted();
    }
}
