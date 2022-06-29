using System;

using ModernFlyouts.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace ModernFlyouts.Views
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
