using ModernFlyouts.Core.Threading;
using ModernFlyouts.Core.Utilities;
using NPSMLib;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ModernFlyouts.Core.Media.Control
{
    public class NowPlayingMediaSessionManager : MediaSessionManager
    {
        private NowPlayingSessionManager NPSessionManager;

        public override async void OnEnabled()
        {
            NpsmService.Started += NpsmService_Started;
            await OnNpsmServiceStarted();
        }

        private async void NpsmService_Started(object sender, EventArgs e)
        {
            await OnNpsmServiceStarted();
        }

        private async Task OnNpsmServiceStarted()
        {
            //Example: explorer.exe crashes, the NPSMLib still holds the "link"
            //THEN explorer.exe restarts and NPSM restarts too, reloading all NPSM sessions

            //Now recreate NPSessionManager.

            try
            {
                await CleanupNPSM();

                NPSessionManager = new();
                NPSessionManager.SessionListChanged += NPSessionsChanged;

                await LoadSessions();
            }
            catch
            {
                //This is in case NPSM dies immediately after sending a wnf notification

                await CleanupNPSM();
            }
        }

        private async void NPSessionsChanged(object sender, NowPlayingSessionManagerEventArgs e)
        {
            await NPSessionsChangedUI(e);
        }

        private async UiTask NPSessionsChangedUI(NowPlayingSessionManagerEventArgs e)
        {
            OnSessionsChanged(e.NowPlayingSessionInfo, e.NotificationType);
        }

        private void OnSessionsChanged(NowPlayingSessionInfo nowPlayingSessionInfo, NowPlayingSessionManagerNotificationType notificationType)
        {
            nowPlayingSessionInfo.GetInfo(out _, out uint pid, out _);

            if (NPSessionManager == null)
            {
                //T H I S    S H O U L D    N E V E R    H A P P E N.
                return;
            }

            if (notificationType == NowPlayingSessionManagerNotificationType.SessionCreated)
            {
                var session = NPSessionManager.FindSession(nowPlayingSessionInfo);

                if (session != null)
                {
                    MediaSessions.Add(new NowPlayingMediaSession(session));
                    RaiseMediaSessionsChanged();
                }
            }
            else if (notificationType == NowPlayingSessionManagerNotificationType.CurrentSessionChanged)
            {
                var mediaSession = MediaSessions
                .Cast<NowPlayingMediaSession>()
                .FirstOrDefault(x => x.NowPlayingSession.GetSessionInfo().Equals(nowPlayingSessionInfo));

                if (mediaSession != null)
                {
                    CurrentMediaSession = mediaSession;
                    foreach (var s in MediaSessions)
                    {
                        s.IsCurrent = false;
                    }

                    mediaSession.IsCurrent = true;
                    RaiseCurrentMediaSessionChanged();
                }
            }
            else if (notificationType == NowPlayingSessionManagerNotificationType.SessionDisconnected)
            {
                var mediaSession = MediaSessions
                .Cast<NowPlayingMediaSession>()
                .FirstOrDefault(x => x.NowPlayingSession.GetSessionInfo().Equals(nowPlayingSessionInfo));

                if (mediaSession != null)
                {
                    mediaSession.Disconnect();
                    MediaSessions.Remove(mediaSession);
                    RaiseMediaSessionsChanged();
                }
            }
        }

        private async UiTask ClearSessions()
        {
            foreach (var session in MediaSessions)
            {
                session.Disconnect();
            }

            MediaSessions.Clear();

            RaiseMediaSessionsChanged();
        }

        private async UiTask LoadSessions()
        {
            await ClearSessions();

            if (NPSessionManager != null)
            {
                var sessions = NPSessionManager.GetSessions();

                foreach (var session in sessions)
                {
                    MediaSessions.Add(new NowPlayingMediaSession(session));
                }
            }

            RaiseMediaSessionsChanged();
        }

        public override async void OnDisabled()
        {
            NpsmService.Started -= NpsmService_Started;
            await CleanupNPSM();
        }

        private async Task CleanupNPSM()
        {
            if (NPSessionManager != null)
            {
                NPSessionManager.SessionListChanged -= NPSessionsChanged;
                NPSessionManager = null;
            }

            await ClearSessions();
        }
    }
}
