using ModernFlyouts.Core.Media.Control;
using ModernFlyouts.Core.UI;
using ModernFlyouts.UI;
using NAudio.Gui;
using System.Windows;
using System.Windows.Controls;

namespace ModernFlyouts.Controls
{
    public partial class VolumeControl : UserControl
    {
        private Orientation _orientation;

        public VolumeControl(Orientation orientation)
        {
            InitializeComponent();
            _orientation = orientation;
            FlyoutHandler.Instance.UIManager.PropertyChanged += CheckIfTopbarChanged;

            if (orientation == Orientation.Vertical)
            {
                SetToVertical();
            } else {
                SetToHorizontal();
            }
        }

        public void SetToHorizontal()
        {
            _orientation = Orientation.Horizontal;

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
            _orientation = Orientation.Vertical;

            Horizontal.MinWidth = 0;
            Vertical.MinHeight = UIManager.DefaultSessionControlHeight;

            int mediaSessionCount = FlyoutHandler.Instance.AudioFlyoutHelper.AllMediaSessions.Count;
            TopBarVisibility topBarVisibility = FlyoutHandler.Instance.UIManager.TopBarVisibility;
            if (topBarVisibility == TopBarVisibility.Visible || topBarVisibility == TopBarVisibility.AutoHide)
            {
                Vertical.MinHeight = Vertical.MinHeight - 32; // topbar height
            }


            Vertical.MinHeight = Vertical.MinHeight - 4; // account for padding

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



        private void CheckIfTopbarChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_orientation == Orientation.Vertical)
            {
                Vertical.MinHeight = UIManager.DefaultSessionControlHeight;

                TopBarVisibility topBarVisibility = FlyoutHandler.Instance.UIManager.TopBarVisibility;

                if (topBarVisibility == TopBarVisibility.Visible || topBarVisibility == TopBarVisibility.AutoHide)
                {
                    Vertical.MinHeight = Vertical.MinHeight - 32; // topbar height
                }

                Vertical.MinHeight = Vertical.MinHeight - 4; // account for padding
            }
        }
    }
}
