using System;
using Windows.Services.Store;
using System.Threading.Tasks;

using System.Collections.Generic;
using Windows.Foundation;

using System.Runtime.InteropServices;
using ModernFlyouts.Uwp.Helpers;

//private StoreContext context = null;


namespace ModernFlyouts.Uwp.Helpers
{
    internal static class UpdateHelper
    {
        internal static async Task<bool> CheckIfUpdateIsAvailable()
        {
            var sContext = StoreContext.GetDefault();
            var updates = await sContext.GetAppAndOptionalStorePackageUpdatesAsync();
            return updates.Count > 0;
        }

        internal static bool IsWindows11()
            => Environment.OSVersion.Version.Build >= 22000;
    }
}

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
