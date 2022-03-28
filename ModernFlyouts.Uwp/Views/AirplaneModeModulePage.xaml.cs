using System;

using ModernFlyouts.Uwp.ViewModels;

using Windows.UI.Xaml.Controls;

namespace ModernFlyouts.Uwp.Views
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
