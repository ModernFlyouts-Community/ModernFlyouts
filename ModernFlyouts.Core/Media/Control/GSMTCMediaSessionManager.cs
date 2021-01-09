using System;
using System.Windows;
using Windows.Media.Control;

namespace ModernFlyouts.Core.Media.Control
{
    public class GSMTCMediaSessionManager : MediaSessionManager
    {
        private GlobalSystemMediaTransportControlsSessionManager GSMTCSessionManager;

        public override async void OnEnabled()
        {
            try
            {
                GSMTCSessionManager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
                GSMTCSessionManager.SessionsChanged += GSMTCSessionsChanged;

                LoadSessions();
            }
            catch { }
        }

        private void GSMTCSessionsChanged(GlobalSystemMediaTransportControlsSessionManager sender, SessionsChangedEventArgs args)
        {
            Application.Current.Dispatcher.Invoke(LoadSessions);
        }

        private void ClearSessions()
        {
            foreach (var session in MediaSessions)
            {
                session.Disconnect();
            }

            MediaSessions.Clear();
        }

        private void LoadSessions()
        {
            ClearSessions();

            if (GSMTCSessionManager != null)
            {
                var sessions = GSMTCSessionManager.GetSessions();

                foreach (var session in sessions)
                {
                    MediaSessions.Add(new GSMTCMediaSession(session));
                }
            }
        }

        public override void OnDisabled()
        {
            try
            {
                if (GSMTCSessionManager != null)
                {
                    GSMTCSessionManager.SessionsChanged -= GSMTCSessionsChanged;
                    GSMTCSessionManager = null;
                }

                ClearSessions();
            }
            catch { }
        }
    }
}
