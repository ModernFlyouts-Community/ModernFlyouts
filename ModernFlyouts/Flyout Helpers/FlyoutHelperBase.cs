using System.Windows;

namespace ModernFlyouts
{
    public abstract class FlyoutHelperBase : DependencyObject
    {
        public FlyoutHelperBase Instance { get; set; }

        public abstract event ShowFlyoutEventHandler ShowFlyoutRequested;

        public delegate void ShowFlyoutEventHandler(FlyoutHelperBase sender);

        #region Properties

        public static readonly DependencyProperty PrimaryContentProperty =
            DependencyProperty.Register(
                nameof(PrimaryContent),
                typeof(FrameworkElement),
                typeof(FlyoutHelperBase),
                new PropertyMetadata(null));

        public FrameworkElement PrimaryContent
        {
            get => (FrameworkElement)GetValue(PrimaryContentProperty);
            set => SetValue(PrimaryContentProperty, value);
        }

        public static readonly DependencyProperty SecondaryContentProperty =
            DependencyProperty.Register(
                nameof(SecondaryContent),
                typeof(FrameworkElement),
                typeof(FlyoutHelperBase),
                new PropertyMetadata(null));

        public FrameworkElement SecondaryContent
        {
            get => (FrameworkElement)GetValue(SecondaryContentProperty);
            set => SetValue(SecondaryContentProperty, value);
        }

        public static readonly DependencyProperty PrimaryContentVisibleProperty =
            DependencyProperty.Register(
                nameof(PrimaryContentVisible),
                typeof(bool),
                typeof(FlyoutHelperBase),
                new PropertyMetadata(true));

        public bool PrimaryContentVisible
        {
            get => (bool)GetValue(PrimaryContentVisibleProperty);
            set => SetValue(PrimaryContentVisibleProperty, value);
        }

        public static readonly DependencyProperty SecondaryContentVisibleProperty =
            DependencyProperty.Register(
                nameof(SecondaryContentVisible),
                typeof(bool),
                typeof(FlyoutHelperBase),
                new PropertyMetadata(false));

        public bool SecondaryContentVisible
        {
            get => (bool)GetValue(SecondaryContentVisibleProperty);
            set => SetValue(SecondaryContentVisibleProperty, value);
        }

        public bool AlwaysHandleDefaultFlyout { get; protected set; } = false;

        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register(
                nameof(IsEnabled),
                typeof(bool),
                typeof(FlyoutHelperBase),
                new PropertyMetadata(true, OnIsEnabledChanged));

        public bool IsEnabled
        {
            get => (bool)GetValue(IsEnabledProperty);
            set => SetValue(IsEnabledProperty, value);
        }

        private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var helper = d as FlyoutHelperBase;
            if ((bool)e.NewValue)
            {
                helper.OnEnabled();
            }
            else
            {
                helper.OnDisabled();
            }
        }

        #endregion

        protected virtual void OnEnabled()
        {

        }

        protected virtual void OnDisabled()
        {
            if (FlyoutHandler.Instance.FlyoutWindow.FlyoutHelper == this)
            {
                FlyoutHandler.Instance.FlyoutWindow.Visible = false;
            }
        }
    }
}