using System;

using ModernFlyouts.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace ModernFlyouts.Views
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
