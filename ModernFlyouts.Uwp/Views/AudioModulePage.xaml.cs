using System;

using ModernFlyouts.Uwp.ViewModels;

using Windows.UI.Xaml.Controls;

namespace ModernFlyouts.Uwp.Views
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
