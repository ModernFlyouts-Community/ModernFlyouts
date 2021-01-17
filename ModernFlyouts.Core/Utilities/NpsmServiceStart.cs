using ModernFlyouts.Core.Interop;
using System;

namespace ModernFlyouts.Core.Utilities
{
    public static class NpsmServiceStart
    {
        private static readonly object _subscriptionLock = new object();
        private static IntPtr _subId;

        private static event EventHandler _npsmServiceStarted;

        //This prevents from being GC'd
        private static Wnf.WnfUserCallback wnfSubHandler = new Wnf.WnfUserCallback(WnfSubHandler);

        private static IntPtr WnfSubHandler(ulong stateName, uint changeStamp, IntPtr typeId, IntPtr callbackContext, IntPtr bufferPtr, uint bufferSize)
        {
            _npsmServiceStarted?.Invoke(null, new EventArgs());
            return IntPtr.Zero;
        }

        /// <summary>
        /// Occurs when the NPSM service is started or restarted
        /// Of course this runs on a different thread.
        /// </summary>
        public static event EventHandler NpsmServiceStarted
        {
            add
            {
                lock (_subscriptionLock)
                {
                    if (_npsmServiceStarted == null)
                    {
                        var wnfData = Wnf.QueryWnf(Wnf.WNF_NPSM_SERVICE_STARTED);
                        Wnf.SubscribeWnf(Wnf.WNF_NPSM_SERVICE_STARTED, wnfData.Changestamp, wnfSubHandler, out _subId);
                    }

                    _npsmServiceStarted += value;
                }
            }
            remove
            {
                lock (_subscriptionLock)
                {
                    _npsmServiceStarted -= value;

                    if (_npsmServiceStarted == null)
                    {
                        Wnf.UnsubscribeWnf(_subId);
                        _subId = IntPtr.Zero;
                    }
                }
            }
        }
    }

    public class NpsmServiceStartedEventArgs : EventArgs
    { }
}
