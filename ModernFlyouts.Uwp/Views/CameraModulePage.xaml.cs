using System;

using ModernFlyouts.Uwp.ViewModels;

using Windows.UI.Xaml.Controls;

namespace ModernFlyouts.Uwp.Views
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
