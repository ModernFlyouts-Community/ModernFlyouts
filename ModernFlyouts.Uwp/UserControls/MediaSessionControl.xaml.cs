using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using NPSMLib;
using ModernFlyouts.Uwp.Helpers;
using Windows.UI.Core;
using ModernFlyouts.Standard.Classes;
using ModernFlyouts.Uwp.Services;
using System;

namespace ModernFlyouts.Uwp.UserControls
{
    public sealed partial class MediaSessionControl : UserControl
    {
        public MediaObjectInfo MediaInfo { get; set; } 
        public MediaTimelineProperties MediaTimeline { get; set; }
        public MediaSession MediaSession
        {
            get => (MediaSession)GetValue(MediaSessionProperty);
            set => SetValue(MediaSessionProperty, value);
        }

        public static readonly DependencyProperty MediaSessionProperty =
          DependencyProperty.Register("MediaSession", typeof(MediaSession), typeof(MediaSessionControl), null);

        public MediaSessionControl()
        {
            InitializeComponent();
            (this.Content as FrameworkElement).DataContext = this;
            setup();
        }


        private async void MediaSessionControl_MediaPlaybackDataChanged(object sender, MediaPlaybackDataChangedArgs e)
        {
       
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                try
                {
                    MediaSession.MediaPlayingSession.ActivateMediaPlaybackDataSource().MediaPlaybackDataChanged += MediaSessionControl_MediaPlaybackDataChanged;
                    MediaInfo = MediaSessionHelper.GetMediaInfo(MediaSession.MediaPlayingSession);
                    MediaTimeline = MediaSessionHelper.GetTimelineInfo(MediaSession.MediaPlayingSession);
                    MediaImage.Source = await MediaSessionService.GetMusicThumbnail(MediaSession.MediaPlayingSession);
                    AppImage.Source = await MediaSessionService.ConvertIconToBitmap(MediaSession.AppIcon);
                    Bindings.Update();
                }
                catch
                {

                }
            });
        }

        public async void setup()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async() =>
            {
            try
            {
                MediaSession.MediaPlayingSession.ActivateMediaPlaybackDataSource().MediaPlaybackDataChanged += MediaSessionControl_MediaPlaybackDataChanged;
                MediaInfo = MediaSessionHelper.GetMediaInfo(MediaSession.MediaPlayingSession);
                MediaTimeline = MediaSessionHelper.GetTimelineInfo(MediaSession.MediaPlayingSession);
                MediaImage.Source = await MediaSessionService.GetMusicThumbnail(MediaSession.MediaPlayingSession);
                AppImage.Source = await MediaSessionService.ConvertIconToBitmap(MediaSession.AppIcon);
               }
                                    catch
                                    {

                                    }
                                   /*   endtime = MediaTimeline.EndTime.ToString(@"mm\:ss"); 
                                      currenttime = MediaTimeline.Position.ToString(@"mm\:ss");
                                      currenttimeval = (int)MediaTimeline.Position.TotalSeconds;
                                      endtimeval = (int)MediaTimeline.EndTime.TotalSeconds;*/
                    Bindings.Update();
            });
        }
    }
}
