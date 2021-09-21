using System;

using ModernFlyouts.Settings.ViewModels;

using Windows.UI.Xaml.Controls;

namespace ModernFlyouts.Settings.Views
{
    public sealed partial class AudioTrayModulePage : Page
    {
        public AudioTrayModuleViewModel ViewModel { get; } = new AudioTrayModuleViewModel();

        public AudioTrayModulePage()
        {
            InitializeComponent();
        }
    }
}
