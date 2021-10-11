using System;

using ModernFlyouts.Settings.ViewModels;

using Windows.UI.Xaml.Controls;

namespace ModernFlyouts.Settings.Views
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
