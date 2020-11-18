using ModernFlyouts.Navigation;
using ModernFlyouts.Helpers;
using ModernWpf.Controls;
using ModernWpf.Media.Animation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace ModernFlyouts
{
    public partial class SettingsWindow : Window
    {
        private bool _isActive;

        public SettingsWindow()
        {
            InitializeComponent();

            WindowPlacementHelper.SetPlacement(new WindowInteropHelper(this).EnsureHandle(), AppDataHelper.SettingsWindowPlacement);

            ContentFrame.Navigated += OnNavigated;

            NavView_Navigate("general", new EntranceNavigationTransitionInfo());

            KeyDown += (s, e) =>
            {
                if (e.Key == Key.Back || (e.Key == Key.Left && Keyboard.Modifiers == ModifierKeys.Alt))
                {
                    BackRequested();
                }
            };
        }

        protected override void OnActivated(EventArgs e)
        {
            if (!_isActive)
            {
                Workarounds.RenderLoopFix.ApplyFix();
                _isActive = true;
            }

            base.OnActivated(e);
        }

        protected override void OnDeactivated(EventArgs e)
        {
            _isActive = false;

            base.OnDeactivated(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            AppDataHelper.SettingsWindowPlacement = WindowPlacementHelper.GetPlacement(new WindowInteropHelper(this).Handle);
            e.Cancel = true;
            Hide();

            base.OnClosing(e);
        }

        #region Navigation

        private readonly List<(string Tag, Type PageType)> _pages = new List<(string Tag, Type PageType)>
        {
            ("general", typeof(GeneralSettingsPage)),
            ("about", typeof(AboutPage)),
            ("personalization", typeof(PersonalizationPage)),
            ("audio_module", typeof(AudioModulePage)),
            ("brightness_module", typeof(BrightnessModulePage)),
            ("airplane_mode_module", typeof(AirplaneModeModulePage)),
            ("lock_keys_module", typeof(LockKeysModulePage))
        };

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem != null)
            {
                var navItemTag = args.SelectedItemContainer.Tag.ToString();
                NavView_Navigate(navItemTag, args.RecommendedNavigationTransitionInfo);
            }
        }

        private void NavView_Navigate(string navItemTag, NavigationTransitionInfo info)
        {
            var item = _pages.FirstOrDefault(p => p.Tag.Equals(navItemTag));
            Type pageType = item.PageType;

            if (pageType != null && ContentFrame.CurrentSourcePageType != pageType)
            {
                ContentFrame.Navigate(pageType, null, info);
            }
        }

        private void NavView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            BackRequested();
        }

        private bool BackRequested()
        {
            if (!ContentFrame.CanGoBack) return false;

            if (NavView.IsPaneOpen &&
                (NavView.DisplayMode == NavigationViewDisplayMode.Minimal
                 || NavView.DisplayMode == NavigationViewDisplayMode.Compact))
            {
                return false;
            }

            ContentFrame.GoBack();
            return true;
        }

        private void OnNavigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            NavView.IsBackEnabled = ContentFrame.CanGoBack;
            Type sourcePageType = ContentFrame.SourcePageType;
            if (sourcePageType != null)
            {
                var item = _pages.FirstOrDefault(p => p.PageType == sourcePageType);

                NavView.SelectedItem = NavView.FooterMenuItems
                    .OfType<NavigationViewItem>().
                    FirstOrDefault(n => n.Tag.Equals(item.Tag)) ??
                    NavView.MenuItems
                    .OfType<NavigationViewItem>()
                    .FirstOrDefault(n => n.Tag.Equals(item.Tag));

                HeaderBlock.Text =
                    ((NavigationViewItem)NavView.SelectedItem)?.Content?.ToString();
            }
        }

        #endregion
    }
}
