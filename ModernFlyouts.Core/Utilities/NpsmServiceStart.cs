using ModernFlyouts.Core.Interop;
using System;

namespace ModernFlyouts.Core.Utilities
{
    public static class NpsmServiceStart
    {
        private static readonly ulong WNF_NPSM_SERVICE_STARTED = 0xC951E23A3BC0875;
        private static readonly ulong WNF_SHEL_SESSION_LOGON_COMPLETE = 0xD83063EA3BE3835;

        private static readonly object _subscriptionLock = new object();
        private static IntPtr _subId;

        private static event EventHandler _npsmServiceStarted;

        //This prevents from being GC'd
        private static readonly Wnf.WnfUserCallback wnfSubHandler = new Wnf.WnfUserCallback(WnfSubHandler);

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
                        var wnfData = Wnf.QueryWnf(WNF_SHEL_SESSION_LOGON_COMPLETE);
                        Wnf.SubscribeWnf(WNF_SHEL_SESSION_LOGON_COMPLETE, wnfData.Changestamp, wnfSubHandler, out _subId);
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
