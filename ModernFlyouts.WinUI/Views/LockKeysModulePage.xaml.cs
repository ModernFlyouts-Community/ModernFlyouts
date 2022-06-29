using System;

using ModernFlyouts.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace ModernFlyouts.Views
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
