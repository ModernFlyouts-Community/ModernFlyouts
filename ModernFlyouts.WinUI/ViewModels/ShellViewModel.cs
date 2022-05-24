using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;


using ModernFlyouts.WinUI.Helpers;
using ModernFlyouts.WinUI.Services;
using ModernFlyouts.WinUI.Views;

using Windows.System;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using ModernFlyouts.Core.Models;

namespace ModernFlyouts.WinUI.ViewModels
{
    public class ShellViewModel : ObservableObject
    {
        private readonly KeyboardAccelerator _altLeftKeyboardAccelerator = BuildKeyboardAccelerator(VirtualKey.Left, VirtualKeyModifiers.Menu);
        private readonly KeyboardAccelerator _backKeyboardAccelerator = BuildKeyboardAccelerator(VirtualKey.GoBack);

        private bool _isBackEnabled;
        private IList<KeyboardAccelerator> _keyboardAccelerators;
        private NavigationView _navigationView;
        private NavigationViewItem _selected;
        private ICommand _loadedCommand;
        private ICommand _itemInvokedCommand;

        public IList<BackdropType> BackdropTypes { get; } = new List<BackdropType>();

        public bool IsAcrylicSupported => Environment.OSVersion.Version.Build >= 22523;
        public bool IsMicaSupported => Environment.OSVersion.Version.Build >= 22523;
        public static bool IsImmersiveDarkModeSupported { get; } = Environment.OSVersion.Version.Build >= 19041;


        public bool IsBackEnabled
        {
            get { return _isBackEnabled; }
            set { SetProperty(ref _isBackEnabled, value); }
        }

        public NavigationViewItem Selected
        {
            get { return _selected; }
            set { SetProperty(ref _selected, value); }
        }

        public ICommand LoadedCommand => _loadedCommand ?? (_loadedCommand = new RelayCommand(OnLoaded));

        public ICommand ItemInvokedCommand => _itemInvokedCommand ?? (_itemInvokedCommand = new RelayCommand<NavigationViewItemInvokedEventArgs>(OnItemInvoked));

        public ShellViewModel()
        {
        }

        public void Initialize(Frame frame, NavigationView navigationView, IList<KeyboardAccelerator> keyboardAccelerators)
        {
            _navigationView = navigationView;
            _keyboardAccelerators = keyboardAccelerators;
            NavigationService.Frame = frame;
            NavigationService.NavigationFailed += Frame_NavigationFailed;
            NavigationService.Navigated += Frame_Navigated;
            _navigationView.BackRequested += OnBackRequested;



            if (BackdropTypes.Count <= 0)
            {
                BackdropTypes.Add(BackdropType.Solid);
                if (IsMicaSupported)
                {
                    BackdropTypes.Add(BackdropType.Mica);
                }
                if (IsAcrylicSupported)
                {
                    BackdropTypes.Add(BackdropType.Acrylic);
                }
            }

        }

        private async void OnLoaded()
        {
            // Keyboard accelerators are added here to avoid showing 'Alt + left' tooltip on the page.
            // More info on tracking issue https://github.com/Microsoft/microsoft-ui-xaml/issues/8
            _keyboardAccelerators.Add(_altLeftKeyboardAccelerator);
            _keyboardAccelerators.Add(_backKeyboardAccelerator);
            await Task.CompletedTask;
        }

        private void OnItemInvoked(NavigationViewItemInvokedEventArgs args)
        {
            {
                var selectedItem = args.InvokedItemContainer as NavigationViewItem;
                var pageType = selectedItem?.GetValue(NavHelper.NavigateToProperty) as Type;

                if (pageType != null)
                {
                    NavigationService.Navigate(pageType, null, args.RecommendedNavigationTransitionInfo);
                }
            }
        }

        private void OnBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            NavigationService.GoBack();
        }

        private void Frame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw e.Exception;
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            IsBackEnabled = NavigationService.CanGoBack;

            var selectedItem = GetSelectedItem(_navigationView.MenuItems, e.SourcePageType);
            if (selectedItem != null)
            {
                Selected = selectedItem;
            }
        }

        private NavigationViewItem GetSelectedItem(IEnumerable<object> menuItems, Type pageType)
        {
            foreach (var item in menuItems.OfType<NavigationViewItem>())
            {
                if (IsMenuItemForPageType(item, pageType))
                {
                    return item;
                }

                var selectedChild = GetSelectedItem(item.MenuItems, pageType);
                if (selectedChild != null)
                {
                    return selectedChild;
                }
            }

            return null;
        }

        private bool IsMenuItemForPageType(NavigationViewItem menuItem, Type sourcePageType)
        {
            var pageType = menuItem.GetValue(NavHelper.NavigateToProperty) as Type;
            return pageType == sourcePageType;
        }

        private static KeyboardAccelerator BuildKeyboardAccelerator(VirtualKey key, VirtualKeyModifiers? modifiers = null)
        {
            var keyboardAccelerator = new KeyboardAccelerator() { Key = key };
            if (modifiers.HasValue)
            {
                keyboardAccelerator.Modifiers = modifiers.Value;
            }

            keyboardAccelerator.Invoked += OnKeyboardAcceleratorInvoked;
            return keyboardAccelerator;
        }

        private static void OnKeyboardAcceleratorInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            var result = NavigationService.GoBack();
            args.Handled = result;
        }
    }
}
//private async Task ShowReleaseNoteAsync()
//{
//    // Make sure we work in background.
//    await TaskScheduler.Default;

//    PackageVersion v = Package.Current.Id.Version;
//    string? currentVersion = $"{v.Major}.{v.Minor}.{v.Build}.{v.Revision}";
//    string? lastVersion = _settingsProvider.GetSetting(PredefinedSettings.LastVersionRan);

//    if (!_settingsProvider.GetSetting(PredefinedSettings.FirstTimeStart) && currentVersion != lastVersion)
//    {
//        _notificationService.ShowInAppNotification(
//            Strings.GetFormattedNotificationReleaseNoteTitle(currentVersion),
//            Strings.NotificationReleaseNoteActionableActionText,
//            () =>
//            {
//                ThreadHelper.ThrowIfNotOnUIThread();
//                Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/veler/DevToys/releases")).AsTask().Forget();
//            },
//            await AssetsHelper.GetReleaseNoteAsync());

//        _marketingService.NotifyAppJustUpdated();
//    }

//    _settingsProvider.SetSetting(PredefinedSettings.FirstTimeStart, false);
//    _settingsProvider.SetSetting(PredefinedSettings.LastVersionRan, currentVersion);
//}

//private async Task ShowAvailableUpdateAsync()
//{
//    // Make sure we work in background.
//    await TaskScheduler.Default;

//    PackageUpdateAvailabilityResult result = await Package.Current.CheckUpdateAvailabilityAsync();

//    if (result.Availability is PackageUpdateAvailability.Required or PackageUpdateAvailability.Available)
//    {
//        _notificationService.ShowInAppNotification(
//            Strings.NotificationUpdateAvailableTitle,
//            Strings.NotificationUpdateAvailableActionableActionText,
//            () =>
//            {
//                ThreadHelper.ThrowIfNotOnUIThread();
//                Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store://downloadsandupdates")).AsTask().Forget();
//            });
//    }
//}
