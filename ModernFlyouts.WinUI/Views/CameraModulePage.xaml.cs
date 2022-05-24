using System;

using ModernFlyouts.WinUI.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace ModernFlyouts.WinUI.Views
{
    public sealed partial class CameraModulePage : Page
    {
        public CameraModuleViewModel ViewModel { get; } = new CameraModuleViewModel();

        public CameraModulePage()
        {
            InitializeComponent();
        }
    }
}
