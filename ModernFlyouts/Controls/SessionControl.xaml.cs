using ModernFlyouts.Core.Media.Control;
using ModernWpf.Media.Animation;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ModernFlyouts
{
    public partial class SessionControl : UserControl
    {
        private MediaSession _mediaSession;

        #region Properties

        public static readonly DependencyProperty AlignThumbnailToRightProperty =
            DependencyProperty.Register(
                nameof(AlignThumbnailToRight),
                typeof(bool),
                typeof(SessionControl),
                new PropertyMetadata(true, OnAlignThumbnailToRightChanged));

        public bool AlignThumbnailToRight
        {
            get => (bool)GetValue(AlignThumbnailToRightProperty);
            set => SetValue(AlignThumbnailToRightProperty, value);
        }
        #endregion

        public SessionControl()
        {
            InitializeComponent();

            Loaded += SessionControl_Loaded;
            Unloaded += SessionControl_Unloaded;
            DataContextChanged += SessionControl_DataContextChanged;

            AlignThumbnailToRight = FlyoutHandler.Instance.UIManager.AlignGSMTCThumbnailToRight;
            BindingOperations.SetBinding(this, AlignThumbnailToRightProperty,
                new Binding(nameof(UI.UIManager.AlignGSMTCThumbnailToRight)) { Source = FlyoutHandler.Instance.UIManager });
        }

        private void SessionControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is MediaSession oldMediaSession)
            {
                oldMediaSession.MediaPropertiesChanging -= MediaSession_MediaPropertiesChanging;
                oldMediaSession.MediaPropertiesChanged -= MediaSession_MediaPropertiesChanged;
            }
            if (e.NewValue is MediaSession mediaSession)
            {
                _mediaSession = mediaSession;
                mediaSession.MediaPropertiesChanging += MediaSession_MediaPropertiesChanging;
                mediaSession.MediaPropertiesChanged += MediaSession_MediaPropertiesChanged;
            }
        }

        private void SessionControl_Loaded(object sender, RoutedEventArgs e)
        {
            FlyoutHandler.Instance.FlyoutWindow.FlyoutTimedHiding += FlyoutWindow_FlyoutTimedHiding;
            FlyoutHandler.Instance.FlyoutWindow.FlyoutHidden += FlyoutWindow_FlyoutHidden;

            if (_mediaSession != null)
            {
                _mediaSession.MediaPropertiesChanging += MediaSession_MediaPropertiesChanging;
                _mediaSession.MediaPropertiesChanged += MediaSession_MediaPropertiesChanged;
            }
        }

        private void FlyoutWindow_FlyoutTimedHiding(object sender, RoutedEventArgs e)
        {
            if (TimelineInfoFlyout.IsOpen)
            {
                e.Handled = true;
            }
        }

        private void FlyoutWindow_FlyoutHidden(object sender, RoutedEventArgs e)
        {
            TimelineInfoFlyout.Hide();
        }

        private void SessionControl_Unloaded(object sender, RoutedEventArgs e)
        {
            TimelineInfoFlyout.Hide();

            FlyoutHandler.Instance.FlyoutWindow.FlyoutTimedHiding -= FlyoutWindow_FlyoutTimedHiding;
            FlyoutHandler.Instance.FlyoutWindow.FlyoutHidden -= FlyoutWindow_FlyoutHidden;

            if (_mediaSession != null)
            {
                _mediaSession.MediaPropertiesChanging -= MediaSession_MediaPropertiesChanging;
                _mediaSession.MediaPropertiesChanged -= MediaSession_MediaPropertiesChanged;
            }
        }

        private void MediaSession_MediaPropertiesChanging(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                BeginTrackTransition();
            });
        }

        private void MediaSession_MediaPropertiesChanged(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                EndTrackTransition();
            });
        }

        private void BeginTrackTransition()
        {
            ThumbnailBackgroundBrush.BeginAnimation(Brush.OpacityProperty, null);
            ThumbnailImageBrush.BeginAnimation(Brush.OpacityProperty, null);
            TextBlockGrid.BeginAnimation(OpacityProperty, null);
            mediaArtistBlockTranslateTransform.BeginAnimation(TranslateTransform.YProperty, null);
            mediaTitleBlockTranslateTransform.BeginAnimation(TranslateTransform.YProperty, null);

            ThumbnailBackgroundBrush.Opacity = 0.0;
            ThumbnailImageBrush.Opacity = 0.0;
            TextBlockGrid.Opacity = 0.0;
            mediaArtistBlockTranslateTransform.Y = 0.0;
            mediaTitleBlockTranslateTransform.Y = 0.0;
        }

        private void EndTrackTransition()
        {
            var direction = _mediaSession.TrackChangeDirection;

            var fadeAnim = new FadeInThemeAnimation() { Duration = TimeSpan.FromMilliseconds(367)};

            ThumbnailBackgroundBrush.BeginAnimation(Brush.OpacityProperty, fadeAnim);
            ThumbnailImageBrush.BeginAnimation(Brush.OpacityProperty, fadeAnim);
            TextBlockGrid.BeginAnimation(OpacityProperty, fadeAnim);

            double offset = direction switch
            {
                Core.Media.MediaPlaybackTrackChangeDirection.Forward => 300.0,
                Core.Media.MediaPlaybackTrackChangeDirection.Backward => -300.0,
                _ => 40.0,
            };

            DependencyProperty property = direction switch
            {
                Core.Media.MediaPlaybackTrackChangeDirection.Unknown => TranslateTransform.YProperty,
                _ => TranslateTransform.XProperty,
            };

            double delay = direction switch
            {
                Core.Media.MediaPlaybackTrackChangeDirection.Unknown => 100.0,
                _ => 0.0,
            };

            var anim1 = new DoubleAnimationUsingKeyFrames()
            {
                KeyFrames =
                {
                    new DiscreteDoubleKeyFrame(offset, TimeSpan.Zero),
                    new SplineDoubleKeyFrame(0, TimeSpan.FromMilliseconds(367), new KeySpline(0.1, 0.9, 0.2, 1))
                }
            };
            mediaTitleBlockTranslateTransform.BeginAnimation(property, anim1);

            var anim2 = new DoubleAnimationUsingKeyFrames()
            {
                BeginTime = TimeSpan.FromMilliseconds(delay),
                KeyFrames = anim1.KeyFrames
            };
            mediaArtistBlockTranslateTransform.BeginAnimation(property, anim2);
        }

        private static void OnAlignThumbnailToRightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sessionControl = d as SessionControl;
            var alignThumbnailToRight = (bool)e.NewValue;

            var C0 = sessionControl.ContentGrid.ColumnDefinitions[0];
            var C2 = sessionControl.ContentGrid.ColumnDefinitions[2];

            if (alignThumbnailToRight)
            {
                C0.Width = new GridLength(15, GridUnitType.Pixel);
                C2.Width = new GridLength(0, GridUnitType.Auto);
                sessionControl.ThumbnailGrid.SetValue(Grid.ColumnProperty, 2);
                sessionControl.thumbnailBGOpacityBrush.GradientOrigin = sessionControl.thumbnailBGOpacityBrush.Center = new Point(1, 0.5);
            }
            else
            {
                C0.Width = new GridLength(0, GridUnitType.Auto);
                C2.Width = new GridLength(15, GridUnitType.Pixel);
                sessionControl.ThumbnailGrid.SetValue(Grid.ColumnProperty, 0);
                sessionControl.thumbnailBGOpacityBrush.GradientOrigin = sessionControl.thumbnailBGOpacityBrush.Center = new Point(0, 0.5);
            }
        }
    }
}
