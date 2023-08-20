using ModernFlyouts.Core.UI;
using ModernFlyouts.UI;
using NAudio.Gui;
using System.Windows;
using System.Windows.Controls;

namespace ModernFlyouts.Controls
{
    public partial class VolumeControl : UserControl
    {

        public VolumeControl(Orientation orientation)
        {
            InitializeComponent();

            if (orientation == Orientation.Vertical)
            {
                SetToVertical();
            } else {
                SetToHorizontal();
            }
        }

        public void SetToHorizontal()
        {
            Horizontal.MinWidth = UIManager.FlyoutWidth;
            Vertical.MinHeight = 0;

            VolumeSlider.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            VolumeSlider.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            VolumeSlider.Orientation = Orientation.Horizontal;

            Horizontal.Children.Clear();

            Thickness pad = new Thickness();
            pad.Right = 4;
            textVal.Padding = pad;

            Vertical.Children.Remove(VolumeButton);
            Vertical.Children.Remove(VolumeSlider);
            Vertical.Children.Remove(textVal);

            Horizontal.Children.Add(VolumeButton);
            Horizontal.Children.Add(VolumeSlider);
            Horizontal.Children.Add(textVal);



        }

        public void SetToVertical()
        {
            Horizontal.MinWidth = 0;
            Vertical.MinHeight = UIManager.DefaultSessionControlHeight;

            VolumeSlider.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            VolumeSlider.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            VolumeSlider.Orientation = Orientation.Vertical;

            Vertical.Children.Clear();

            Thickness pad = new Thickness();
            pad.Bottom = 4;
            textVal.Padding = pad;

            Horizontal.Children.Remove(VolumeButton);
            Horizontal.Children.Remove(VolumeSlider);
            Horizontal.Children.Remove(textVal);

            Vertical.Children.Add(VolumeButton);
            Vertical.Children.Add(VolumeSlider);
            Vertical.Children.Add(textVal);

        }
    }
}
