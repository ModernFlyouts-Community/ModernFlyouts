using ModernFlyouts.Uwp.ViewModels;

namespace ModernFlyouts.Uwp
{
    public sealed partial class TrayIconView
    {
        public TrayIconView()
        {
            InitializeComponent();
        }

        public ITrayIconViewModel ViewModel { get; set; }
    }
}
