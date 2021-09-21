using System;

using ModernFlyouts.Settings.ViewModels;

using Windows.UI.Xaml.Controls;

namespace ModernFlyouts.Settings.Views
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
