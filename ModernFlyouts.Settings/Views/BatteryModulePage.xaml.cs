using System;

using ModernFlyouts.Settings.ViewModels;

using Windows.UI.Xaml.Controls;

namespace ModernFlyouts.Settings.Views
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
