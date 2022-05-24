using System;

using ModernFlyouts.WinUI.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace ModernFlyouts.WinUI.Views
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
