using ModernFlyouts.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using Windows.Services.Store;
using Windows.Foundation;
using System.Runtime.InteropServices;
using Windows.UI.Popups;

namespace ModernFlyouts.Navigation
{
    public partial class GeneralSettingsPage : Page
    {
        public GeneralSettingsPage()
        {
            InitializeComponent();
        }

        private void ExitButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            FlyoutHandler.SafelyExitApplication();
        }

        private async void ResetButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            await AppDataHelper.ClearAppDataAsync();
            FlyoutHandler.Instance.UIManager.RestartRequired = true;
        }


        StoreContext context = StoreContext.GetDefault();

        public async Task DownloadAndInstallAllUpdatesInBackgroundAsync()
        {
            if (context == null)
            {
                context = StoreContext.GetDefault();
            }

            // Get the updates that are available.
            IReadOnlyList<StorePackageUpdate> storePackageUpdates =
                await context.GetAppAndOptionalStorePackageUpdatesAsync();

            if (storePackageUpdates.Count > 0)
            {

                if (!context.CanSilentlyDownloadStorePackageUpdates)
                {
                    return;
                }

                // Start the silent downloads and wait for the downloads to complete.
                StorePackageUpdateResult downloadResult =
                    await context.TrySilentDownloadStorePackageUpdatesAsync(storePackageUpdates);

                }
            }
        }

    //private async Task InstallUpdate(IReadOnlyList<StorePackageUpdate> storePackageUpdates)
    //    {
    //    StorePackageUpdateResult downloadResult = await context.TrySilentDownloadAndInstallStorePackageUpdatesAsync(storePackageUpdates);

    //}
}



//private StoreContext context = null;

//public async Task DownloadAndInstallAllUpdatesAsync()
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