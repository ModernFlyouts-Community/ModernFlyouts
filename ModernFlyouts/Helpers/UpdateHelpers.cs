//using System;
//using System.Collections.Generic;
//using Windows.Foundation;
//using Windows.Services.Store;
//using System.Runtime.InteropServices;
//using ModernFlyouts.Helpers;
//using System.Threading.Tasks;



//private StoreContext context = null;

//public async Task DownloadAndInstallAllUpdatesAsync()
//{
//    if (context == null)
//    {
//        context = StoreContext.GetDefault();
//    }

//    // Get the updates that are available.
//    IReadOnlyList<StorePackageUpdate> updates =
//        await context.GetAppAndOptionalStorePackageUpdatesAsync();

//    if (updates.Count > 0)
//    {
//        // Alert the user that updates are available and ask for their consent
//        // to start the updates.
//        MessageDialog dialog = new MessageDialog(
//            "Download and install updates now? This may cause the application to exit.", "Download and Install?");
//        dialog.Commands.Add(new UICommand("Yes"));
//        dialog.Commands.Add(new UICommand("No"));
//        IUICommand command = await dialog.ShowAsync();

//        if (command.Label.Equals("Yes", StringComparison.CurrentCultureIgnoreCase))
//        {
//            // Download and install the updates.
//            IAsyncOperationWithProgress<StorePackageUpdateResult, StorePackageUpdateStatus> downloadOperation =
//                context.RequestDownloadAndInstallStorePackageUpdatesAsync(updates);

//            // The Progress async method is called one time for each step in the download
//            // and installation process for each package in this request.
//            downloadOperation.Progress = async (asyncInfo, progress) =>
//            {
//                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
//                () =>
//                {
//                    downloadProgressBar.Value = progress.PackageDownloadProgress;
//                });
//            };

//            StorePackageUpdateResult result = await downloadOperation.AsTask();
//        }
//    }
//}






//async void GetEasyUpdates()
//{
//    StoreContext updateManager = StoreContext.GetDefault();
//    IReadOnlyList<StorePackageUpdate> updates = await updateManager.GetAppAndOptionalStorePackageUpdatesAsync();

//    if (updates.Count > 0)
//    {
//        IAsyncOperationWithProgress<StorePackageUpdateResult, StorePackageUpdateStatus> downloadOperation =
//            updateManager.RequestDownloadAndInstallStorePackageUpdatesAsync(updates);
//        StorePackageUpdateResult result = await downloadOperation.AsTask();
//    }
//}







//// Register the active instance of an application for restart in your Update method
//uint res = RelaunchHelper.RegisterApplicationRestart(null, RelaunchHelper.RestartFlags.NONE);




//namespace ModernFlyouts.Helpers
//{
//    class RelaunchHelper
//    {
//        #region Restart Manager Methods
//        /// <summary>
//        /// Registers the active instance of an application for restart.
//        /// </summary>
//        /// <param name="pwzCommandLine">
//        /// A pointer to a Unicode string that specifies the command-line arguments for the application when it is restarted.
//        /// The maximum size of the command line that you can specify is RESTART_MAX_CMD_LINE characters. Do not include the name of the executable
//        /// in the command line; this function adds it for you.
//        /// If this parameter is NULL or an empty string, the previously registered command line is removed. If the argument contains spaces,
//        /// use quotes around the argument.
//        /// </param>
//        /// <param name="dwFlags">One of the options specified in RestartFlags</param>
//        /// <returns>
//        /// This function returns S_OK on success or one of the following error codes:
//        /// E_FAIL for internal error.
//        /// E_INVALIDARG if rhe specified command line is too long.
//        /// </returns>
//        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
//        internal static extern uint RegisterApplicationRestart(string pwzCommandLine, RestartFlags dwFlags);
//        #endregion Restart Manager Methods

//        #region Restart Manager Enums
//        /// <summary>
//        /// Flags for the RegisterApplicationRestart function
//        /// </summary>
//        [Flags]
//        internal enum RestartFlags
//        {
//            /// <summary>None of the options below.</summary>
//            NONE = 0,

//            /// <summary>Do not restart the process if it terminates due to an unhandled exception.</summary>
//            RESTART_NO_CRASH = 1,
//            /// <summary>Do not restart the process if it terminates due to the application not responding.</summary>
//            RESTART_NO_HANG = 2,
//            /// <summary>Do not restart the process if it terminates due to the installation of an update.</summary>
//            RESTART_NO_PATCH = 4,
//            /// <summary>Do not restart the process if the computer is restarted as the result of an update.</summary>
//            RESTART_NO_REBOOT = 8
//        }
//        #endregion Restart Manager Enums

//    }
//}


//using Windows.ApplicationModel;
//using Windows.Management.Deployment;
//public async void CheckForAppInstallerUpdatesAndLaunchAsync(string targetPackageFullName, PackageVolume packageVolume)
//{
//    // Get the current app's package for the current user.
//    PackageManager pm = new PackageManager();
//    Package package = pm.FindPackageForUser(string.Empty, targetPackageFullName);

//    PackageUpdateAvailabilityResult result = await package.CheckUpdateAvailabilityAsync();
//    switch (result.Availability)
//    {
//        case PackageUpdateAvailability.Available:
//        case PackageUpdateAvailability.Required:
//            //Queue up the update and close the current instance
//            await pm.AddPackageByAppInstallerFileAsync(
//            new Uri("https://trial3.azurewebsites.net/HRApp/HRApp.appinstaller"),
//            AddPackageByAppInstallerOptions.ForceApplicationShutdown,
//            packageVolume);
//            break;
//        case PackageUpdateAvailability.NoUpdates:
//            // Close AppInstaller.
//            await ConsolidateAppInstallerView();
//            break;
//        case PackageUpdateAvailability.Unknown:
//        default:
//            // Log and ignore error.
//            Logger.Log($"No update information associated with app {targetPackageFullName}");
//            // Launch target app and close AppInstaller.
//            await ConsolidateAppInstallerView();
//            break;
//    }
//}


//// Queue up the update and close the current app instance.
//private async void CommandInvokedHandler(IUICommand command)
//{
//    if (command.Label == "Update")
//    {
//        PackageManager packagemanager = new PackageManager();
//        await packagemanager.AddPackageAsync(
//        new Uri("https://trial3.azurewebsites.net/HRApp/HRApp.msix"),
//        null,
//        AddPackageOptions.ForceApplicationShutdown
//        );
//    }
//}

