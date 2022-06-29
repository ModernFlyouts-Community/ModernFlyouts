using System;

using ModernFlyouts.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace ModernFlyouts.Views
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
