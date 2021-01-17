using ModernFlyouts.Core.Utilities;
using NPSMLib;
using System.Linq;
using System.Windows;

namespace ModernFlyouts.Core.Media.Control
{
    public class NowPlayingMediaSessionManager : MediaSessionManager
    {
        private NowPlayingSessionManager NPSessionManager;

        public override void OnEnabled()
        {
            NPSessionManager = new();
            NPSessionManager.SessionListChanged += NPSessionsChanged;
            NpsmServiceStart.NpsmServiceStarted += NpsmServiceStart_NpsmServiceStarted;

            LoadSessions();
        }

        private void NpsmServiceStart_NpsmServiceStarted(object sender, System.EventArgs e)
        {
            //For some reasons it doesn't clear the sessions (you'll have duplicates)...
            //But at least it shouldn't die anymore
            //Oh, and of course this fixes 19041 issue where GSMTC/NPSMLib stops working randomly...
            //Well, the NPSM service crashes and restarts but old handles still remains.
            //That's why we create a new instance when NPSM is (re)started so we have a new one.

            Application.Current.Dispatcher.Invoke(() =>
            {
                NPSessionManager = new();
                NPSessionManager.SessionListChanged += NPSessionsChanged;

                LoadSessions();
            });
        }

        private void NPSessionsChanged(object sender, NowPlayingSessionManagerEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => OnSessionsChanged(e.NowPlayingSessionInfo, e.NotificationType));
        }

        private void OnSessionsChanged(NowPlayingSessionInfo nowPlayingSessionInfo, NowPlayingSessionManagerNotificationType notificationType)
        {
            nowPlayingSessionInfo.GetInfo(out _, out uint pid, out _);

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

        private void ClearSessions()
        {
            foreach (var session in MediaSessions)
            {
                session.Disconnect();
            }

            MediaSessions.Clear();

            RaiseMediaSessionsChanged();
        }

        private void LoadSessions()
        {
            ClearSessions();

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

        public override void OnDisabled()
        {
            if (NPSessionManager != null)
            {
                NPSessionManager.SessionListChanged -= NPSessionsChanged;
                NPSessionManager = null;
            }

            ClearSessions();
        }
    }
}
