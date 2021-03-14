using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ModernFlyouts.Core.Helpers
{
    //Taken from here! 
    //https://medium.com/criteo-engineering/switching-back-to-the-ui-thread-in-wpf-uwp-in-modern-c-5dc1cc8efa5e

    [AsyncMethodBuilder(typeof(UiTaskMethodBuilder))]
    public class UiTask
    {
        internal TaskCompletionSource<object> Promise { get; } = new TaskCompletionSource<object>();

        public Task AsTask() => Promise.Task;

        public TaskAwaiter<object> GetAwaiter()
        {
            return Promise.Task.GetAwaiter();
        }

        public static implicit operator Task(UiTask task) => task.AsTask();
    }

    public class UiTaskMethodBuilder
    {
        private readonly Dispatcher _dispatcher;

        public UiTaskMethodBuilder(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public void Start<TStateMachine>(ref TStateMachine stateMachine)
            where TStateMachine : IAsyncStateMachine
        {
            if (!_dispatcher.CheckAccess())
            {
                _dispatcher.BeginInvoke(new Action(stateMachine.MoveNext));
            }
            else
            {
                stateMachine.MoveNext();
            }
        }

        public static UiTaskMethodBuilder Create()
        {
            return new UiTaskMethodBuilder(Application.Current.Dispatcher);
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
        }

        public void SetResult()
        {
            Task.Promise.SetResult(null);
        }

        public void SetException(Exception exception)
        {
            Task.Promise.SetException(exception);
        }

        public UiTask Task { get; } = new UiTask();

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(
            ref TAwaiter awaiter,
            ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            awaiter.OnCompleted(ResumeAfterAwait(stateMachine));
        }

        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(
            ref TAwaiter awaiter,
            ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            awaiter.UnsafeOnCompleted(ResumeAfterAwait(stateMachine));
        }

        private Action ResumeAfterAwait<TStateMachine>(TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            return () =>
            {
                if (!_dispatcher.CheckAccess())
                {
                    _dispatcher.BeginInvoke(new Action(stateMachine.MoveNext));
                }
                else
                {
                    stateMachine.MoveNext();
                }
            };
        }
    }
}
