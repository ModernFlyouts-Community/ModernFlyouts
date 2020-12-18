using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections;

namespace ModernFlyouts.Core.Media.Control
{
    public abstract class MediaSessionManagerBase : ObservableObject
    {
        public IEnumerable MediaSessions
        {
            get;
        }

        public abstract void OnEnabled();

        public abstract void OnDisabled();

        public virtual bool ContainsAnySession()
        {
            return false;
        }
    }
}
