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

            LoadSessions();
        }

        private void NPSessionsChanged(object sender, NowPlayingSessionManagerEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                e.NowPlayingSessionInfo.GetInfo(out _, out uint pid, out _);

                if (e.NotificationType == NowPlayingSessionManagerNotificationType.SessionCreated)
                {
                    var sessionInfo = e.NowPlayingSessionInfo;
                    var session = NPSessionManager.FindSession(sessionInfo);

                    if (session != null)
                    {
                        MediaSessions.Add(new NowPlayingMediaSession(session));
                        RaiseMediaSessionsChanged();
                    }
                }
                else if (e.NotificationType == NowPlayingSessionManagerNotificationType.CurrentSessionChanged)
                {
                    var sessionInfo = e.NowPlayingSessionInfo;
                    var mediaSession = MediaSessions
                    .Cast<NowPlayingMediaSession>()
                    .FirstOrDefault(x => x.NowPlayingSession.PID == pid);

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
                else if (e.NotificationType == NowPlayingSessionManagerNotificationType.SessionDisconnected)
                {
                    var mediaSession = MediaSessions
                    .Cast<NowPlayingMediaSession>()
                    .FirstOrDefault(x => x.NowPlayingSession.PID == pid);

                    if (mediaSession != null)
                    {
                        mediaSession.Disconnect();
                        MediaSessions.Remove(mediaSession);
                        RaiseMediaSessionsChanged();
                    }
                }
            });
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
