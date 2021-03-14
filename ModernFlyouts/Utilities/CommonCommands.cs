using Microsoft.Toolkit.Mvvm.Input;
using ModernFlyouts.Core.UI;
using ModernFlyouts.Helpers;
using System.Threading.Tasks;
using System.Windows;

namespace ModernFlyouts.Utilities
{
    public static class CommonCommands
    {
        public static RelayCommand OpenSettingsWindowCommand { get; } =
            new RelayCommand(() => FlyoutHandler.ShowSettingsWindow(), () => FlyoutHandler.HasInitialized);

        public static RelayCommand AlignOnScreenFlyoutToDefaultPosition { get; } =
            new RelayCommand(() => FlyoutHandler.Instance.AlignFlyout(), () => FlyoutHandler.HasInitialized);

        public static RelayCommand PinUnpinFlyoutTopBarCommand { get; } =
            new RelayCommand(() =>
            {
                var uiManager = FlyoutHandler.Instance.UIManager;
                if (uiManager.TopBarVisibility == UI.TopBarVisibility.Visible)
                {
                    uiManager.TopBarVisibility = UI.TopBarVisibility.AutoHide;
                }
                else if (uiManager.TopBarVisibility == UI.TopBarVisibility.AutoHide)
                {
                    uiManager.TopBarVisibility = UI.TopBarVisibility.Visible;
                }
            }, () => FlyoutHandler.HasInitialized);

        public static RelayCommand<FrameworkElement> CloseFlyoutCommand { get; } =
            new RelayCommand<FrameworkElement>(x =>
            {
                var flyoutWindow = FlyoutWindow.GetFlyoutWindow(x);
                if (flyoutWindow != null)
                {
                    flyoutWindow.IsOpen = false;
                }
            }, x => FlyoutHandler.HasInitialized && x != null);

        public static RelayCommand ExitAppCommand { get; } =
            new RelayCommand(() => FlyoutHandler.SafelyExitApplication(), () => FlyoutHandler.HasInitialized);

        public static AsyncRelayCommand ResetAppDataCommand { get; } =
            new AsyncRelayCommand(() => Task.Run(async () =>
            {
                await AppDataHelper.ClearAppDataAsync();
                FlyoutHandler.Instance.UIManager.RestartRequired = true;
            }), () => FlyoutHandler.HasInitialized);

        static CommonCommands()
        {
            FlyoutHandler.Initialized += (_, __) => Refresh();
        }

        private static void Refresh()
        {
            OpenSettingsWindowCommand.NotifyCanExecuteChanged();
            AlignOnScreenFlyoutToDefaultPosition.NotifyCanExecuteChanged();
            PinUnpinFlyoutTopBarCommand.NotifyCanExecuteChanged();
            CloseFlyoutCommand.NotifyCanExecuteChanged();
            ExitAppCommand.NotifyCanExecuteChanged();
            ResetAppDataCommand.NotifyCanExecuteChanged();
        }
    }
}
