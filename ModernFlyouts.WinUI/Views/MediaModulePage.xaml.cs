using System;

using ModernFlyouts.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace ModernFlyouts.Views
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
