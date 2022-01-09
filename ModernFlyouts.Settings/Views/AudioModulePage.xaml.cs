using System;

using ModernFlyouts.Settings.ViewModels;

using Windows.UI.Xaml.Controls;

namespace ModernFlyouts.Settings.Views
{
    public sealed partial class AudioModulePage : Page
    {
        public AudioModuleViewModel ViewModel { get; } = new AudioModuleViewModel();

        public AudioModulePage()
        {
            InitializeComponent();
        }
    }
}
