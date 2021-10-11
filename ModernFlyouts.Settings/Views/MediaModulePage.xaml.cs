using System;

using ModernFlyouts.Settings.ViewModels;

using Windows.UI.Xaml.Controls;

namespace ModernFlyouts.Settings.Views
{
    public sealed partial class MediaModulePage : Page
    {
        public MediaModuleViewModel ViewModel { get; } = new MediaModuleViewModel();

        public MediaModulePage()
        {
            InitializeComponent();
        }
    }
}
