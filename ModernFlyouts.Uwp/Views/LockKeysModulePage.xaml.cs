using System;

using ModernFlyouts.Uwp.ViewModels;

using Windows.UI.Xaml.Controls;

namespace ModernFlyouts.Uwp.Views
{
    public sealed partial class LockKeysModulePage : Page
    {
        public LockKeysModuleViewModel ViewModel { get; } = new LockKeysModuleViewModel();

        public LockKeysModulePage()
        {
            InitializeComponent();
        }
    }
}
