using System;

using ModernFlyouts.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace ModernFlyouts.Views
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
