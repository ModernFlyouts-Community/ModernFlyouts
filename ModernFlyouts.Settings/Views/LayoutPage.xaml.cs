using System;

using ModernFlyouts.Settings.ViewModels;

using Windows.UI.Xaml.Controls;

namespace ModernFlyouts.Settings.Views
{
    public sealed partial class LayoutPage : Page
    {
        public LayoutViewModel ViewModel { get; } = new LayoutViewModel();

        public LayoutPage()
        {
            InitializeComponent();
        }
    }
}
