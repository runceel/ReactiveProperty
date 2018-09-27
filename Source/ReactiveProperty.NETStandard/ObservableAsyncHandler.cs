using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Reactive.Bindings
{
    // Reusable
    public class ObservableAsyncHandler<T> : IDisposable, IObserver<T>, ICriticalNotifyCompletion
    {
        static readonly Action<object> cancelDelegate = DisposeSelf;
        static readonly SendOrPostCallback syncContextPost = PostInvoke;

        IDisposable subscription;
        CancellationTokenRegistration cancellationTokenRegistration;
        CancellationToken token;

        bool completed;
        T currentValue;
        ExceptionDispatchInfo exception;
        Action continuation;
        SynchronizationContext context;
        readonly object gate = new object();

        public ObservableAsyncHandler(IObservable<T> source, CancellationToken cancellationToken)
        {
            if (cancellationToken.CanBeCanceled)
            {
                cancellationTokenRegistration = cancellationToken.Register(cancelDelegate, this, false);
            }
            this.subscription = source.Subscribe(this);
            this.context = SynchronizationContext.Current;
        }

        // for the Task.WhenAll, WhenAny
        public async Task AsTask()
        {
            await this;
        }

        static void DisposeSelf(object state)
        {
            var self = (ObservableAsyncHandler<T>)state;
            self.Dispose();
        }

        static void PostInvoke(object state)
        {
            var continuation = (Action)state;
            continuation();
        }

        public void Dispose()
        {
            completed = true;
            cancellationTokenRegistration.Dispose();
            subscription?.Dispose();
            subscription = null;
            TryInvokeContinuation();
        }

        void TryInvokeContinuation()
        {
            if (this.continuation != null)
            {
                var c = this.continuation;
                this.continuation = null;

                if (this.context != null && this.context != SynchronizationContext.Current)
                {
                    this.context.Post(syncContextPost, c);
                }
                else
                {
                    c.Invoke();
                }
            }
        }

        void IObserver<T>.OnNext(T value)
        {
            lock (gate)
            {
                this.currentValue = value;
                TryInvokeContinuation();
            }
        }

        void IObserver<T>.OnError(Exception error)
        {
            exception = ExceptionDispatchInfo.Capture(error);
            Dispose();
        }

        void IObserver<T>.OnCompleted()
        {
            Dispose();
        }

        // awaitable

        public ObservableAsyncHandler<T> GetAwaiter()
        {
            return this;
        }

        // awaiter

        public bool IsCompleted
        {
            get
            {
                return (exception != null || token.IsCancellationRequested || completed);
            }
        }

        public T GetResult()
        {
            if (exception != null)
            {
                exception.Throw();
                return default(T);
            }

            token.ThrowIfCancellationRequested();

            if (completed)
            {
                throw new OperationCanceledException();
            }

            return currentValue;
        }

        void ICriticalNotifyCompletion.UnsafeOnCompleted(Action continuation)
        {
            if (this.continuation != null) throw new InvalidOperationException();
            this.continuation = continuation;
        }

        void INotifyCompletion.OnCompleted(Action continuation)
        {
            if (this.continuation != null) throw new InvalidOperationException();
            this.continuation = continuation;
        }
    }

    public static class ObservableAsyncHandlerExtensions
    {
        public static ObservableAsyncHandler<T> GetAsyncHandler<T>(this IObservable<T> source, CancellationToken cancellationToken)
        {
            return new ObservableAsyncHandler<T>(source, cancellationToken);
        }

        // not recommended, should use WaitUntilValueChanged(with CancellationToken)

        public static TaskAwaiter<T> GetAwaiter<T>(this IReactiveProperty<T> source)
        {
            return WaitUntilValueChangedAsync<T>(source, CancellationToken.None).GetAwaiter();
        }

        public static TaskAwaiter<T> GetAwaiter<T>(this IReadOnlyReactiveProperty<T> source)
        {
            return WaitUntilValueChangedAsync<T>(source, CancellationToken.None).GetAwaiter();
        }

        public static TaskAwaiter<T> GetAwaiter<T>(this ReactiveCommand<T> source)
        {
            return WaitUntilValueChangedAsync<T>(source, CancellationToken.None).GetAwaiter();
        }

        // one shot

        public static async Task<T> WaitUntilValueChangedAsync<T>(this IReactiveProperty<T> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var handler = GetAsyncHandler<T>(source, cancellationToken))
            {
                return await handler;
            }
        }

        public static async Task<T> WaitUntilValueChangedAsync<T>(this IReadOnlyReactiveProperty<T> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var handler = GetAsyncHandler<T>(source, cancellationToken))
            {
                return await handler;
            }
        }

        public static async Task<T> WaitUntilValueChangedAsync<T>(this ReactiveCommand<T> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var handler = GetAsyncHandler<T>(source, cancellationToken))
            {
                return await handler;
            }
        }
    }
}
