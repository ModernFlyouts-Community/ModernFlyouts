using System;

using ModernFlyouts.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace ModernFlyouts.Views
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
