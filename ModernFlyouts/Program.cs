using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using ModernFlyouts.AppLifecycle;
using ModernFlyouts.Helpers;
using ModernFlyouts.Interop;
using System;
using System.Reflection;

namespace ModernFlyouts
{
    public class Program
    {
        public const string AppName = "ModernFlyouts";

        [STAThread]
        private static void Main(string[] args)
        {
            AppLifecycleManager.StartApplication(args, () =>
            {
                AppCenter.Start("26393d67-ab03-4e26-a6db-aa76bf989c21",
                    typeof(Analytics), typeof(Crashes));

                AppDataMigration.Perform();

                DUIHandler.ForceFindDUIAndHide(false);

                LocalizationHelper.Initialize();

                var app = new App();
                app.Run();
            });
        }

        internal static void RunCommand(RunCommandType runCommandType)
        {
            switch (runCommandType)
            {
                case RunCommandType.ShowSettings:
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
                case RunCommandType.RestoreDefault:
                    {
                        if (!DUIHandler.IsDUIAvailable())
                        {
                            DUIHandler.GetAllInfos();
                        }
                        FlyoutHandler.SafelyExitApplication();
                        break;
                    }
                case RunCommandType.SafeExit:
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
    }

    internal enum RunCommandType
    {
        ShowSettings = 0,
        RestoreDefault = 1,
        SafeExit = 2
    }
}