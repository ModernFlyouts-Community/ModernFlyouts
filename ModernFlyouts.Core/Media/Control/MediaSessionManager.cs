using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace ModernFlyouts.Core.Media.Control
{
    public abstract class MediaSessionManager<TMediaSession> : ObservableObject
        where TMediaSession : MediaSession
    {
        public ObservableCollection<TMediaSession> MediaSessions { get; } = new();

        public MediaSessionManager()
        {
            MediaSessions.CollectionChanged += OnMediaSessionsCollectionChanged;
        }

        public abstract void OnEnabled();

        public abstract void OnDisabled();

        private void OnMediaSessionsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SessionsChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool ContainsAnySession()
        {
            return MediaSessions.Count > 0;
        }

        public event EventHandler SessionsChanged;
    }
}
