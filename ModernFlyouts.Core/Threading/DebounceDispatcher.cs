using System;
using System.Windows.Threading;

namespace ModernFlyouts.Core.Threading
{
    public class DebounceDispatcher
    {
        private DispatcherTimer timer;

        private Action _action;

        public DebounceDispatcher(DispatcherPriority priority = DispatcherPriority.ApplicationIdle,
            Dispatcher dispatcher = null)
        {
            if (dispatcher == null)
                dispatcher = Dispatcher.CurrentDispatcher;
            timer = new DispatcherTimer(priority, dispatcher);
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            _action?.Invoke();
        }

        public void Debounce(TimeSpan interval, Action action)
        {
            timer.Stop();

            _action = action;

            timer.Interval = interval;

            timer.Start();
        }
    }
}
