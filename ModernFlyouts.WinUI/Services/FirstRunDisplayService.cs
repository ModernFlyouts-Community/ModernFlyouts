using System;
using System.Threading.Tasks;

using CommunityToolkit.WinUI.Helpers;

using ModernFlyouts.WinUI.Views;

using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace ModernFlyouts.WinUI.Services
{
    public static class FirstRunDisplayService
    {
        private static bool shown = false;

        internal static async Task ShowIfAppropriateAsync()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, async () =>
                {
                    if (SystemInformation.Instance.IsFirstRun && !shown)
                    {
                        shown = true;
                        var dialog = new FirstRunDialog();
                        await dialog.ShowAsync();
                    }
                });

        }
    }
}
