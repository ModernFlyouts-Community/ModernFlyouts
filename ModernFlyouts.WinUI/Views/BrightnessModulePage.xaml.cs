using System;

using ModernFlyouts.WinUI.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace ModernFlyouts.WinUI.Views
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
