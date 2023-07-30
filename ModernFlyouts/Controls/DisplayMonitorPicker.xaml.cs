using CommunityToolkit.Mvvm.Input;
using ModernFlyouts.Core.Display;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ModernFlyouts.Controls
{
    public partial class DisplayMonitorPicker : UserControl
    {
        #region Properties

        #region PreferredDisplayMonitor

        public static readonly DependencyProperty PreferredDisplayMonitorProperty =
            DependencyProperty.Register(
                nameof(PreferredDisplayMonitor),
                typeof(DisplayMonitor),
                typeof(DisplayMonitorPicker),
                new PropertyMetadata());

        public DisplayMonitor PreferredDisplayMonitor
        {
            get => (DisplayMonitor)GetValue(PreferredDisplayMonitorProperty);
            set => SetValue(PreferredDisplayMonitorProperty, value);
        }

        #endregion

        #region SetPreferredDisplayMonitorCommand

        public static readonly DependencyProperty SetPreferredDisplayMonitorCommandProperty =
            DependencyProperty.Register(
                nameof(SetPreferredDisplayMonitorCommand),
                typeof(ICommand),
                typeof(DisplayMonitorPicker),
                new PropertyMetadata());

        public ICommand SetPreferredDisplayMonitorCommand
        {
            get => (ICommand)GetValue(SetPreferredDisplayMonitorCommandProperty);
            set => SetValue(SetPreferredDisplayMonitorCommandProperty, value);
        }

        #endregion

        #endregion

        public DisplayMonitorPicker()
        {
            InitializeComponent();

            SetPreferredDisplayMonitorCommand = new RelayCommand<DisplayMonitor>(SetPreferredDisplayMonitor, x => true);
        }

        private void SetPreferredDisplayMonitor(DisplayMonitor displayMonitor)
        {
            PreferredDisplayMonitor = displayMonitor;
        }
    }
}
