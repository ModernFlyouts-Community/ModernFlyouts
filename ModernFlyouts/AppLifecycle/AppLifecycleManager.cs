using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Threading;

namespace ModernFlyouts.AppLifecycle
{
    public class AppLifecycleManager
    {
        /// <summary>
        /// This constant allows us to distinguish between beta and stable releases and will helps us to enable/disable features for a certain build.<br/>
        /// This value could be changed by adding or removing '-beta' suffix to the app version in the *.csproj.<br/>
        /// <strong>For example:</strong> 'x.y.z.i-beta' would make this value <see langword="true" /> and removing '-beta' would make this <see langword="false"/>.
        /// </summary>
        public const bool IsBuildBetaChannel =
#if BETA
            true;
#else
            false;
#endif

        #region App activation & Single instancing

        private static Mutex mutex = new Mutex(true, Program.AppName);

        /// <summary>
        /// Starts the application as single instance and redirects the command line arguments from subsequent instances to the first instance.
        /// </summary>
        /// <param name="args">Commandline arguments to process or pass to the first instance.</param>
        /// <param name="action">The action to perform after the first instance has been intialized.</param>
        public static void StartApplication(string[] args, Action action)
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                ProcessCommandLineArgs(args);

                CreateRemoteService(Program.AppName);

                action();
            }
            else
            {
                SignalFirstInstance(Program.AppName, args);
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
                    Program.RunCommand(RunCommandType.ShowSettings);
                }
            }
            else if (arg.ToLowerInvariant() == JumpListHelper.arg_settings)
            {
                Program.RunCommand(RunCommandType.ShowSettings);
            }
            else if (arg.ToLowerInvariant() == JumpListHelper.arg_restore)
            {
                Program.RunCommand(RunCommandType.RestoreDefault);
            }
            else if (arg.ToLowerInvariant() == JumpListHelper.arg_exit)
            {
                Program.RunCommand(RunCommandType.SafeExit);
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

#endregion
    }
}
