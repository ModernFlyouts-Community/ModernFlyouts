using ModernFlyouts.Controls;
using System.Windows;
using System.Windows.Controls;

namespace ModernFlyouts.Views
{
    public partial class FlyoutView : UserControl
    {
        #region Properties

        #region FlyoutHelper

        public static readonly DependencyProperty FlyoutHelperProperty =
            DependencyProperty.Register(
                nameof(FlyoutHelper),
                typeof(FlyoutHelperBase),
                typeof(FlyoutView),
                new PropertyMetadata(null, OnFlyoutHelperPropertyChanged));

        public FlyoutHelperBase FlyoutHelper
        {
            get => (FlyoutHelperBase)GetValue(FlyoutHelperProperty);
            set => SetValue(FlyoutHelperProperty, value);
        }

        private static void OnFlyoutHelperPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FlyoutView flyoutView)
            {
                if (e.NewValue is FlyoutHelperBase flyoutHelper)
                {
                    flyoutView.PrimaryHost.Content = flyoutHelper.PrimaryContent;
                    flyoutView.SecondaryHost.Content = flyoutHelper.SecondaryContent;
                }
                else
                {
                    flyoutView.PrimaryHost.Content = null;
                    flyoutView.SecondaryHost.Content = null;
                }
            }
        }

        #endregion

        #region FlyoutTopBar

        public static readonly DependencyProperty FlyoutTopBarProperty =
            DependencyProperty.Register(
                nameof(FlyoutTopBar),
                typeof(FlyoutTopBar),
                typeof(FlyoutView),
                new PropertyMetadata(null, OnFlyoutTopBarPropertyChanged));

        public FlyoutTopBar FlyoutTopBar
        {
            get => (FlyoutTopBar)GetValue(FlyoutTopBarProperty);
            set => SetValue(FlyoutTopBarProperty, value);
        }

        private static void OnFlyoutTopBarPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FlyoutView flyoutView)
            {
                flyoutView.TopBarHost.Content = e.NewValue;
            }
        }

        #endregion

        #endregion

        public FlyoutView()
        {
            InitializeComponent();
        }
    }
}
