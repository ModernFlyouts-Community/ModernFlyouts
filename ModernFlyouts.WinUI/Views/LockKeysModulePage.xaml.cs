using System;

using ModernFlyouts.WinUI.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace ModernFlyouts.WinUI.Views
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
