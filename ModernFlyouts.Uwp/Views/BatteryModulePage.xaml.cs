using System;

using ModernFlyouts.Uwp.ViewModels;

using Windows.UI.Xaml.Controls;

namespace ModernFlyouts.Uwp.Views
{
    public sealed partial class BatteryModulePage : Page
    {
        public BatteryModuleViewModel ViewModel { get; } = new BatteryModuleViewModel();

        public BatteryModulePage()
        {
            InitializeComponent();
        }
    }
}
