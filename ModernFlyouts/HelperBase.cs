using System.Windows;

namespace ModernFlyouts
{
    public abstract class HelperBase : DependencyObject
    {
        public HelperBase Instance { get; set; }

        public abstract event ShowFlyoutEventHandler ShowFlyoutRequested;

        public delegate void ShowFlyoutEventHandler(object sender, bool handled);

        #region Properties

        public static readonly DependencyProperty PrimaryContentProperty =
            DependencyProperty.Register("PrimaryContent",
                typeof(FrameworkElement),
                typeof(HelperBase),
                new PropertyMetadata(null));

        public FrameworkElement PrimaryContent
        {
            get => (FrameworkElement)GetValue(PrimaryContentProperty);
            set => SetValue(PrimaryContentProperty, value);
        }

        public static readonly DependencyProperty SecondaryContentProperty =
            DependencyProperty.Register("SecondaryContent",
                typeof(FrameworkElement),
                typeof(HelperBase),
                new PropertyMetadata(null));

        public FrameworkElement SecondaryContent
        {
            get => (FrameworkElement)GetValue(SecondaryContentProperty);
            set => SetValue(SecondaryContentProperty, value);
        }

        public static readonly DependencyProperty PrimaryContentVisibleProperty =
            DependencyProperty.Register("PrimaryContentVisible",
                typeof(bool),
                typeof(HelperBase),
                new PropertyMetadata(true));

        public bool PrimaryContentVisible
        {
            get => (bool)GetValue(PrimaryContentVisibleProperty);
            set => SetValue(PrimaryContentVisibleProperty, value);
        }

        public static readonly DependencyProperty SecondaryContentVisibleProperty =
            DependencyProperty.Register("SecondaryContentVisible",
                typeof(bool),
                typeof(HelperBase),
                new PropertyMetadata(false));

        public bool SecondaryContentVisible
        {
            get => (bool)GetValue(SecondaryContentVisibleProperty);
            set => SetValue(SecondaryContentVisibleProperty, value);
        }

        #endregion
    }
}