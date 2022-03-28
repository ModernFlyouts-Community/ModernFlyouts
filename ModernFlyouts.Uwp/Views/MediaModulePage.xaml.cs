using System;

using ModernFlyouts.Uwp.ViewModels;

using Windows.UI.Xaml.Controls;

namespace ModernFlyouts.Uwp.Views
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
