using System;

using ModernFlyouts.WinUI.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace ModernFlyouts.WinUI.Views
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
