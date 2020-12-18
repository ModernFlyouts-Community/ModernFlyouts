﻿using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace ModernFlyouts.Core.Media.Control
{
    public abstract class MediaSessionManager<TMediaSession> : MediaSessionManagerBase
        where TMediaSession : MediaSession
    {
        public new ObservableCollection<TMediaSession> MediaSessions { get; } = new();

        public MediaSessionManager()
        {
            MediaSessions.CollectionChanged += OnMediaSessionsCollectionChanged;
        }

        private void OnMediaSessionsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SessionsChanged?.Invoke(this, EventArgs.Empty);
        }

        public override bool ContainsAnySession()
        {
            return MediaSessions.Count > 0;
        }

        public event EventHandler SessionsChanged;
    }
}
