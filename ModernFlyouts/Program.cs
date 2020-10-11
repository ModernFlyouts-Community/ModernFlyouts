using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Threading;

namespace ModernFlyouts
{
    public class Program
    {
        public const string AppName = "ModernFlyouts";

        private static Mutex mutex = new Mutex(true, AppName);

        [STAThread]
        static void Main(string[] args)
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                ProcessCommandLineArgs(args);

                CreateRemoteService(AppName);

                DUIHandler.ForceFindDUIAndHide(false);

                var app = new App();
                app.Run();
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

                IList<string> args = rawArgs.Split(JumpListHelper.arg_delimiter);

                ProcessCommandLineArgs(args, false);

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
                    RunCommand(CommandType.ShowSettings);
                }
            }
            else if (arg.ToLowerInvariant() == JumpListHelper.arg_settings)
            {
                RunCommand(CommandType.ShowSettings);
            }
            else if (arg.ToLowerInvariant() == JumpListHelper.arg_restore)
            {
                RunCommand(CommandType.RestoreDefault);
            }
            else if (arg.ToLowerInvariant() == JumpListHelper.arg_exit)
            {
                RunCommand(CommandType.SafeExit);
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
                writer.Write(arg + JumpListHelper.arg_delimiter);
            }

            writer.Flush();

            Environment.Exit(0);
        }

        private static void RunCommand(CommandType commandType)
        {
            switch (commandType)
            {
                case CommandType.ShowSettings:
                    {
                        if (FlyoutHandler.HasInitialized)
                        {
                            FlyoutHandler.ShowSettingsWindow();
                        }
                        else
                        {
                            FlyoutHandler.Initialized += (_, __) => FlyoutHandler.ShowSettingsWindow();
                        }
                        break;
                    }
                case CommandType.RestoreDefault:
                    {
                        if (!DUIHandler.IsDUIAvailable())
                        {
                            DUIHandler.GetAllInfos();
                        }
                        FlyoutHandler.SafelyExitApplication();
                        break;
                    }
                case CommandType.SafeExit:
                    {
                        FlyoutHandler.SafelyExitApplication();
                        break;
                    }
                default:
                    break;
            }
        }

        private enum CommandType
        {
            ShowSettings = 0,
            RestoreDefault = 1,
            SafeExit = 2
        }
    }
}
