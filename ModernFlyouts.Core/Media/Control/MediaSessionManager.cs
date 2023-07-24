using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;

namespace ModernFlyouts.Core.Media.Control
{
    public abstract class MediaSessionManager : ObservableObject
    {
        public ObservableCollection<MediaSession> MediaSessions { get; } = new();

        private MediaSession currentMediaSession;

        public MediaSession CurrentMediaSession
        {
            get => currentMediaSession;
            protected set
            {
                if (SetProperty(ref currentMediaSession, value))
                {
                    RaiseCurrentMediaSessionChanged();
                }
            }
        }

        public abstract void OnEnabled();

        public abstract void OnDisabled();

        public virtual bool ContainsAnySession()
        {
            return MediaSessions.Count > 0;
        }

        protected void RaiseCurrentMediaSessionChanged()
        {
            CurrentMediaSessionChanged?.Invoke(this, EventArgs.Empty);
        }

        protected void RaiseMediaSessionsChanged()
        {
            MediaSessionsChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler CurrentMediaSessionChanged;

        public event EventHandler MediaSessionsChanged;
    }
}
