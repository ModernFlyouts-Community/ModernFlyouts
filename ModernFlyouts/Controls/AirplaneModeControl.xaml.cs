using ModernWpf.Controls;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;

namespace ModernFlyouts.Controls
{
    public partial class AirplaneModeControl : UserControl
    {
        public AirplaneModeControl()
        {
            InitializeComponent();
        }

        public void InvalidateProperties()
        {
            BindingOperations.GetBindingExpression(MainStackPanel, StackPanel.DataContextProperty).UpdateTarget();
        }
    }
}
