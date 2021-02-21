using ModernFlyouts.Core.Helpers;
using ModernFlyouts.Core.Utilities;
using NPSMLib;
using System;
using System.Linq;

namespace ModernFlyouts.Core.Media.Control
{
    public class NowPlayingMediaSessionManager : MediaSessionManager
    {
        private NowPlayingSessionManager NPSessionManager;

        public override async void OnEnabled()
        {
            NpsmServiceStart.NpsmServiceStarted += NpsmServiceStart_NpsmServiceStarted;
            NpsmServiceStart_NpsmServiceStarted(null, null);
        }

        private async void NpsmServiceStart_NpsmServiceStarted(object sender, EventArgs e)
        {
            //Example: explorer.exe crashes, the NPSMLib still holds the "link"
            //THEN explorer.exe restarts and NPSM restarts too, reloading all NPSM sessions

            //Now recreate NPSessionManager.
            try
            {
                NPSessionManager = new();
                NPSessionManager.SessionListChanged += NPSessionsChanged;

                await LoadSessions();
            }
            catch (Exception)
            {
                //This is in case NPSM dies immediately after sending a wnf notification
                await ClearSessions();
                if (NPSessionManager != null)
                    NPSessionManager.SessionListChanged -= NPSessionsChanged;
                NPSessionManager = null;
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
            if (NPSessionManager != null)
            {
                NPSessionManager.SessionListChanged -= NPSessionsChanged;
                NPSessionManager = null;
            }

            await ClearSessions();
        }
    }
}
