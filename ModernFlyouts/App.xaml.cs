using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Windows;
using System.Windows.Shell;

namespace ModernFlyouts
{
    public partial class App : Application, ISingleInstanceApp
    {
        public const string AppName = "ModernFlyouts";
        public static string AppPath = string.Empty;

        private const string arg_settings = "/settings";
        private const string arg_exit = "/exit-safe";
        private const string arg_restore = "/restore";
        private const char arg_delimiter = ' ';
        private static Mutex mutex = new Mutex(true, AppName);

        public App()
        {
            AppPath = Process.GetCurrentProcess().MainModule.FileName;
            Startup += App_Startup;
            CreateJumpList();
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                ProcessCommandLineArgs(e.Args);

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
                StreamReader reader = new StreamReader(pipeServer);
                var rawArgs = await reader.ReadToEndAsync();

                IList<string> args = rawArgs.Split(arg_delimiter);

                if (Current != null)
                {
                    Current.Dispatcher.Invoke(() =>
                    {
                        if (Current is ISingleInstanceApp singleInstanceApp)
                        {
                            singleInstanceApp.OnSecondAppStarted(args);
                        }
                    });
                }

                pipeServer.Disconnect();
            }
        }

        public static void ProcessCommandLineArgs(IList<string> args, bool isFirstInstance = true)
        {
            string arg = string.Empty;

            if ((isFirstInstance && args?.Count > 0) || (!isFirstInstance && args?.Count > 1))
            {
                arg = isFirstInstance ? args[0] : args[1];
            }

            if (arg == string.Empty)
            {
                if (!isFirstInstance)
                {
                    FlyoutHandler.ShowSettingsWindow();
                }
            }
            else if (arg.ToLowerInvariant() == arg_settings)
            {
                if (FlyoutHandler.HasInitialized)
                {
                    FlyoutHandler.ShowSettingsWindow();
                }
                else
                {
                    FlyoutHandler.Initialized += (_, __) => FlyoutHandler.ShowSettingsWindow();
                }
            }
            else if (arg.ToLowerInvariant() == arg_restore)
            {
                if (!DUIHandler.IsDUIAvailable())
                {
                    DUIHandler.GetAllInfos();
                }
                FlyoutHandler.SafelyExitApplication();
            }
            else if (arg.ToLowerInvariant() == arg_exit)
            {
                FlyoutHandler.SafelyExitApplication();
            }
        }

        private static async void SignalFirstInstance(string channelName)
        {
            using NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", channelName, PipeDirection.Out);
            await pipeClient.ConnectAsync(0).ConfigureAwait(false);
            var args = Environment.GetCommandLineArgs();
            StreamWriter writer = new StreamWriter(pipeClient) { AutoFlush = true };

            foreach (var arg in args)
            {
                writer.Write(arg + arg_delimiter);
            }

            writer.Flush();

            Environment.Exit(0);
        }

        public void OnSecondAppStarted(IList<string> args)
        {
            ProcessCommandLineArgs(args, false);
        }

        private void CreateJumpList()
        {
            JumpList jumpList = new JumpList();
            JumpList.SetJumpList(Current, jumpList);
            JumpTask settingsTask = new JumpTask
            {
                Title = "Settings",
                Description = "Open settings window",
                ApplicationPath = AppPath,
                Arguments = arg_settings
            };
            jumpList.JumpItems.Add(settingsTask);

            JumpTask restoreTask = new JumpTask
            {
                Title = "Restore Default",
                Description = "Restores the windows default flyout and safely quits this app",
                ApplicationPath = AppPath,
                Arguments = arg_restore
            };
            jumpList.JumpItems.Add(restoreTask);

            JumpTask exitTask = new JumpTask
            {
                Title = "Exit",
                Description = "Exit the app safely",
                ApplicationPath = AppPath,
                Arguments = arg_exit
            };
            jumpList.JumpItems.Add(exitTask);

            jumpList.Apply();
        }
    }

    internal interface ISingleInstanceApp
    {
        void OnSecondAppStarted(IList<string> args);
    }
}
