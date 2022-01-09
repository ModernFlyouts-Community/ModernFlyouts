using System;

using ModernFlyouts.Settings.ViewModels;

using Windows.UI.Xaml.Controls;

namespace ModernFlyouts.Settings.Views
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
