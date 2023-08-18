using ModernFlyouts.Core.UI;
using ModernFlyouts.UI;
using NAudio.Gui;
using System.Windows;
using System.Windows.Controls;

namespace ModernFlyouts.Controls
{
    public partial class VolumeControl : UserControl
    {

        private Grid grid;

        public VolumeControl(Orientation orientation)
        {
            InitializeComponent();

            if (orientation == Orientation.Vertical)
            {
                SetToHorizontal();
            } else {
                SetToVertical();
            }
        }

        public void SetToHorizontal()
        {
            MinWidth = UIManager.FlyoutWidth;
            VolumeButton.SetValue(DockPanel.DockProperty, Dock.Left);
            textVal.SetValue(DockPanel.DockProperty, Dock.Right);
            VolumeSlider.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            VolumeSlider.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            VolumeSlider.Orientation = Orientation.Horizontal;
        }

        public void SetToVertical()
        {
            MinWidth = 0;
            VolumeSlider.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            VolumeSlider.Orientation = Orientation.Vertical;
            VolumeButton.SetValue(DockPanel.DockProperty, Dock.Top);
            textVal.SetValue(DockPanel.DockProperty, Dock.Bottom);
            VolumeSlider.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
        }
    }
}
