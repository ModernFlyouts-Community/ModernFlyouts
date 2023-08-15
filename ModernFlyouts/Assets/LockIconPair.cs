using System.Windows;
using System.Windows.Automation.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;



namespace ModernFlyouts.Assets
{
    public class IconPair
    {
        public DrawingImage OnImage { get; set; }
        public DrawingImage OffImage { get; set; }
    }

    public class LockIcons
    {
        public IconPair caps = new IconPair
        {
            OnImage = (DrawingImage)Application.Current.Resources["caps_on"],
            OffImage = (DrawingImage)Application.Current.Resources["caps_off"]
        };

        public IconPair scroll_lock = new IconPair
        {
            OnImage = (DrawingImage)Application.Current.Resources["scroll_lock_on"],
            OffImage = (DrawingImage)Application.Current.Resources["scroll_lock_off"]
        };

        public IconPair num = new IconPair
        {
            OnImage = (DrawingImage)Application.Current.Resources["num_on"],
            OffImage = (DrawingImage)Application.Current.Resources["num_off"]
        };

        public IconPair insert = new IconPair
        {
            OnImage = (DrawingImage)Application.Current.Resources["insert_on"],
            OffImage = (DrawingImage)Application.Current.Resources["insert_off"]
        };
    }
}
