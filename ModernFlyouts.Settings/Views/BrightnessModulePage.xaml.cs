using System;

using ModernFlyouts.Settings.ViewModels;

using Windows.UI.Xaml.Controls;

namespace ModernFlyouts.Settings.Views
{
    public sealed partial class BrightnessModulePage : Page
    {
        public BrightnessModuleViewModel ViewModel { get; } = new BrightnessModuleViewModel();

        public BrightnessModulePage()
        {
            InitializeComponent();
        }
    }
}
