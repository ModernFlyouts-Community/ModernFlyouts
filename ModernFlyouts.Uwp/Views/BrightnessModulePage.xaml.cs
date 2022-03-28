using System;

using ModernFlyouts.Uwp.ViewModels;

using Windows.UI.Xaml.Controls;

namespace ModernFlyouts.Uwp.Views
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
