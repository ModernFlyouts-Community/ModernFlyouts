using System;

using ModernFlyouts.WinUI.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace ModernFlyouts.WinUI.Views
{
    public sealed partial class AirplaneModeModulePage : Page
    {
        public AirplaneModeModuleViewModel ViewModel { get; } = new AirplaneModeModuleViewModel();

        public AirplaneModeModulePage()
        {
            InitializeComponent();
        }
    }
}
