﻿using ModernFlyouts.AppLifecycle;
using ModernFlyouts.Core.Interop;
using ModernFlyouts.Helpers;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace ModernFlyouts
{
    public class Program
    {
        public const string AppName = "ModernFlyouts";
        public const string AppLauncherName = "ModernFlyoutsLauncher";

        [STAThread]
        private static void Main(string[] args)
        {
            Thread thread = new Thread(() => {
                AppLifecycleManager.StartApplication(args, () =>
                {
#if DEBUG
                    Debugger.Launch();
#elif RELEASE
                    Microsoft.AppCenter.AppCenter.Start("26393d67-ab03-4e26-a6db-aa76bf989c21",
                        typeof(Microsoft.AppCenter.Analytics.Analytics), typeof(Microsoft.AppCenter.Crashes.Crashes));
#endif
                    InitializePrivateUseClasses();

                    AppDataMigration.Perform();

                    NativeFlyoutHandler.Instance = new NativeFlyoutHandler();
                    NativeFlyoutHandler.Instance.Initialize();

                    LocalizationHelper.Initialize();

                    var app = new App();
                    app.Run();
                });
            });

            //If you lauch directly from the host bridge it won't be STA.
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
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
                        //if (AppLifecycleManager.IsBuildBetaChannel)
                        //{
                        //    MessageBox.Show("App update successfully!", AppName);
                        //}

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
