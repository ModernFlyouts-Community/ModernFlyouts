using System;

using ModernFlyouts.WinUI.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace ModernFlyouts.WinUI.Views
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
