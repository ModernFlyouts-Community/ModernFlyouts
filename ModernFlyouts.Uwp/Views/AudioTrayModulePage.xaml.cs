using System;

using ModernFlyouts.Uwp.ViewModels;

using Windows.UI.Xaml.Controls;

namespace ModernFlyouts.Uwp.Views
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
