using System;

using ModernFlyouts.Uwp.ViewModels;

using Windows.UI.Xaml.Controls;

namespace ModernFlyouts.Uwp.Views
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
