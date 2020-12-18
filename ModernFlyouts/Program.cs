using ModernFlyouts.AppLifecycle;
using ModernFlyouts.Helpers;
using ModernFlyouts.Core.Interop;
using System;
using System.Reflection;
using System.Windows;

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
                InitializePrivateUseClasses();

#if RELEASE
                Microsoft.AppCenter.AppCenter.Start("26393d67-ab03-4e26-a6db-aa76bf989c21",
                    typeof(Microsoft.AppCenter.Analytics.Analytics), typeof(Microsoft.AppCenter.Crashes.Crashes));
#endif

                AppDataMigration.Perform();

                NativeFlyoutHandler.Instance = new NativeFlyoutHandler();
                NativeFlyoutHandler.Instance.Initialize();

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
                        NativeFlyoutHandler.Instance.VerifyNativeFlyoutCreated();
                        FlyoutHandler.SafelyExitApplication();
                        break;
                    }
                case RunCommandType.SafeExit:
                    {
                        FlyoutHandler.SafelyExitApplication();
                        break;
                    }
                case RunCommandType.AppUpdated:
                    {
                        if (AppLifecycleManager.IsBuildBetaChannel)
                        {
                            MessageBox.Show("App update successfully!", AppName);
                        }

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

        internal static void InitializePrivateUseClasses()
        {
#if Screenshots
            FlyoutHandler.Initialized += (_, __) => Private.ScreenshotHelper.Initialize();
#endif
        }
    }

    internal enum RunCommandType
    {
        ShowSettings = 0,
        RestoreDefault = 1,
        SafeExit = 2,
        AppUpdated = 3
    }
}
