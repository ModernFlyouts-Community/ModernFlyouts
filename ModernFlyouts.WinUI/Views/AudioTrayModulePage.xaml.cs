using System;

using ModernFlyouts.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace ModernFlyouts.Views
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
