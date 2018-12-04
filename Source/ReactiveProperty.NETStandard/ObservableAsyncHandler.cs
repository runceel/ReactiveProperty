using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Reactive.Bindings
{
    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="System.IDisposable"/>
    /// <seealso cref="System.IObserver{T}"/>
    /// <seealso cref="System.Runtime.CompilerServices.ICriticalNotifyCompletion"/>
    public class ObservableAsyncHandler<T> : IDisposable, IObserver<T>, ICriticalNotifyCompletion
    {
        private static readonly Action<object> cancelDelegate = DisposeSelf;
        private static readonly SendOrPostCallback syncContextPost = PostInvoke;
        private IDisposable subscription;
        private CancellationTokenRegistration cancellationTokenRegistration;
        private CancellationToken token;
        private bool completed;
        private T currentValue;
        private ExceptionDispatchInfo exception;
        private Action continuation;
        private SynchronizationContext context;
        private readonly object gate = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableAsyncHandler{T}"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public ObservableAsyncHandler(IObservable<T> source, CancellationToken cancellationToken)
        {
            if (cancellationToken.CanBeCanceled)
            {
                cancellationTokenRegistration = cancellationToken.Register(cancelDelegate, this, false);
            }
            subscription = source.Subscribe(this);
            context = SynchronizationContext.Current;
        }

        /// <summary>
        /// Ases the task.
        /// </summary>
        /// <returns></returns>
        public async Task AsTask()
        {
            await this;
        }

        private static void DisposeSelf(object state)
        {
            var self = (ObservableAsyncHandler<T>)state;
            self.Dispose();
        }

        private static void PostInvoke(object state)
        {
            var continuation = (Action)state;
            continuation();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            completed = true;
            cancellationTokenRegistration.Dispose();
            subscription?.Dispose();
            subscription = null;
            TryInvokeContinuation();
        }

        private void TryInvokeContinuation()
        {
            if (continuation != null)
            {
                var c = continuation;
                continuation = null;

                if (context != null && context != SynchronizationContext.Current)
                {
                    context.Post(syncContextPost, c);
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
                currentValue = value;
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

        /// <summary>
        /// Gets the awaiter.
        /// </summary>
        /// <returns></returns>
        public ObservableAsyncHandler<T> GetAwaiter()
        {
            return this;
        }

        // awaiter

        /// <summary>
        /// Gets a value indicating whether this instance is completed.
        /// </summary>
        /// <value><c>true</c> if this instance is completed; otherwise, <c>false</c>.</value>
        public bool IsCompleted
        {
            get
            {
                return (exception != null || token.IsCancellationRequested || completed);
            }
        }

        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.OperationCanceledException"></exception>
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
            if (this.continuation != null)
            {
                throw new InvalidOperationException();
            }

            this.continuation = continuation;
        }

        void INotifyCompletion.OnCompleted(Action continuation)
        {
            if (this.continuation != null)
            {
                throw new InvalidOperationException();
            }

            this.continuation = continuation;
        }
    }

    /// <summary>
    /// Observable Async Handler Extensions
    /// </summary>
    public static class ObservableAsyncHandlerExtensions
    {
        /// <summary>
        /// Gets the asynchronous handler.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static ObservableAsyncHandler<T> GetAsyncHandler<T>(this IObservable<T> source, CancellationToken cancellationToken)
        {
            return new ObservableAsyncHandler<T>(source, cancellationToken);
        }

        /// <summary>
        /// Gets the awaiter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static TaskAwaiter<T> GetAwaiter<T>(this IReactiveProperty<T> source)
        {
            return WaitUntilValueChangedAsync<T>(source, CancellationToken.None).GetAwaiter();
        }

        /// <summary>
        /// Gets the awaiter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static TaskAwaiter<T> GetAwaiter<T>(this IReadOnlyReactiveProperty<T> source)
        {
            return WaitUntilValueChangedAsync<T>(source, CancellationToken.None).GetAwaiter();
        }

        /// <summary>
        /// Gets the awaiter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static TaskAwaiter<T> GetAwaiter<T>(this ReactiveCommand<T> source)
        {
            return WaitUntilValueChangedAsync<T>(source, CancellationToken.None).GetAwaiter();
        }

        // one shot

        /// <summary>
        /// Waits the until value changed asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static async Task<T> WaitUntilValueChangedAsync<T>(this IReactiveProperty<T> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var handler = GetAsyncHandler<T>(source, cancellationToken))
            {
                return await handler;
            }
        }

        /// <summary>
        /// Waits the until value changed asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static async Task<T> WaitUntilValueChangedAsync<T>(this IReadOnlyReactiveProperty<T> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var handler = GetAsyncHandler<T>(source, cancellationToken))
            {
                return await handler;
            }
        }

        /// <summary>
        /// Waits the until value changed asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static async Task<T> WaitUntilValueChangedAsync<T>(this ReactiveCommand<T> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var handler = GetAsyncHandler<T>(source, cancellationToken))
            {
                return await handler;
            }
        }
    }
}
