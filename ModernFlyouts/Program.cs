using ModernFlyouts.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Threading;
using System.Windows;

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

                LocalizationHelper.Initialize();

                var app = new App();
                app.Run();
            }
            else
            {
                SignalFirstInstance(AppName, args);
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

            if (args?.Count > 0)
            {
                arg = args[0];
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

        private static void SignalFirstInstance(string channelName, string[] args)
        {
            string rawArgs = string.Empty;
            foreach (var arg in args)
            {
                rawArgs += arg + JumpListHelper.arg_delimiter;
            }
            
            using NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", channelName, PipeDirection.Out);
            pipeClient.Connect(0);
            
            StreamWriter writer = new StreamWriter(pipeClient) { AutoFlush = true };
            writer.Write(rawArgs);
            writer.Flush();
            writer.Close();
            pipeClient.Dispose();

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

        public static string AppVersion
        {
            get => Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private enum CommandType
        {
            ShowSettings = 0,
            RestoreDefault = 1,
            SafeExit = 2
        }
    }
}
