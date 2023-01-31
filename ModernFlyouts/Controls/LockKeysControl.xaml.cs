using ModernWpf.Controls;
using System.Windows.Controls;
using System.Windows.Data;

namespace ModernFlyouts.Controls
{
    public partial class LockKeysControl : UserControl
    {
        public LockKeysControl()
        {
            InitializeComponent();
        }

        public void InvalidateProperties()
        {
            BindingOperations.GetBindingExpression(LockStatusText, TextBlock.TextProperty).UpdateTarget();
            BindingOperations.GetBindingExpression(LockStatusGlyph, FontIcon.GlyphProperty).UpdateTarget();
        }
    }
}
