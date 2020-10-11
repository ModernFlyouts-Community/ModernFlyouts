using ModernFlyouts.Navigation;
using ModernWpf.Controls;
using ModernWpf.Media.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace ModernFlyouts
{
	public partial class SettingsWindow : Window
	{
		public SettingsWindow()
		{
			InitializeComponent();

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

		private readonly List<(string Tag, Type PageType)> _pages = new List<(string Tag, Type PageType)>
		{
			("general", typeof(GeneralSettingsPage)),
			("about", typeof(AboutPage)),
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

				NavView.SelectedItem = NavView.MenuItems
					.OfType<NavigationViewItem>()
					.First(n => n.Tag.Equals(item.Tag));

				NavView.Header =
					((NavigationViewItem)NavView.SelectedItem)?.Content?.ToString();
			}
		}
	}
}
