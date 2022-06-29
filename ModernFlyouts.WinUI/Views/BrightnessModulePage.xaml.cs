using System;

using ModernFlyouts.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace ModernFlyouts.Views
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
