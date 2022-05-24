using System;

using ModernFlyouts.WinUI.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace ModernFlyouts.WinUI.Views
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
