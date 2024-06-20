using ModernFlyouts.Core.UI;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ModernFlyouts.Navigation
{
    public partial class AudioModulePage : Page
    {
        public AudioModulePage()
        {
            InitializeComponent();

            OrientationPicker.ItemsSource = new Collection<Orientation>
            {
                Orientation.Horizontal, Orientation.Vertical,
            };
        }
    }
}
