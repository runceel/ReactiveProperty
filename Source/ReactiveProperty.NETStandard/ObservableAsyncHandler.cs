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
        private static readonly Action<object> s_cancelDelegate = DisposeSelf;
        private static readonly SendOrPostCallback s_syncContextPost = PostInvoke;
        private IDisposable _subscription;
        private readonly CancellationTokenRegistration _cancellationTokenRegistration;
        private bool _completed;
        private T _currentValue;
        private ExceptionDispatchInfo _exception;
        private Action _continuation;
        private readonly SynchronizationContext _context;
        private readonly object _gate = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableAsyncHandler{T}"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public ObservableAsyncHandler(IObservable<T> source, CancellationToken cancellationToken)
        {
            if (cancellationToken.CanBeCanceled)
            {
                _cancellationTokenRegistration = cancellationToken.Register(s_cancelDelegate, this, false);
            }
            _subscription = source.Subscribe(this);
            _context = SynchronizationContext.Current;
        }

        /// <summary>
        /// Converts this into a <see cref="Task"/>
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
            _completed = true;
            _cancellationTokenRegistration.Dispose();
            _subscription?.Dispose();
            _subscription = null;
            TryInvokeContinuation();
        }

        private void TryInvokeContinuation()
        {
            if (_continuation != null)
            {
                var c = _continuation;
                _continuation = null;

                if (_context != null && _context != SynchronizationContext.Current)
                {
                    _context.Post(s_syncContextPost, c);
                }
                else
                {
                    c.Invoke();
                }
            }
        }

        void IObserver<T>.OnNext(T value)
        {
            lock (_gate)
            {
                _currentValue = value;
                TryInvokeContinuation();
            }
        }

        void IObserver<T>.OnError(Exception error)
        {
            _exception = ExceptionDispatchInfo.Capture(error);
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
                return (_exception != null || _completed);
            }
        }

        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.OperationCanceledException"></exception>
        public T GetResult()
        {
            if (_exception != null)
            {
                _exception.Throw();
                return default;
            }

            if (_completed)
            {
                throw new OperationCanceledException();
            }

            return _currentValue;
        }

        void ICriticalNotifyCompletion.UnsafeOnCompleted(Action continuation)
        {
            if (_continuation != null)
            {
                throw new InvalidOperationException();
            }

            _continuation = continuation;
        }

        void INotifyCompletion.OnCompleted(Action continuation)
        {
            if (_continuation != null)
            {
                throw new InvalidOperationException();
            }

            _continuation = continuation;
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
        public static TaskAwaiter<T> GetAwaiter<T>(this ReactiveProperty<T> source)
        {
            return WaitUntilValueChangedAsync<T>(source, CancellationToken.None).GetAwaiter();
        }

        /// <summary>
        /// Gets the awaiter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static TaskAwaiter<T> GetAwaiter<T>(this ReactivePropertySlim<T> source)
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
        public static async Task<T> WaitUntilValueChangedAsync<T>(this IReactiveProperty<T> source, CancellationToken cancellationToken = default)
        {
            using var handler = GetAsyncHandler<T>(source, cancellationToken);
            return await handler;
        }

        /// <summary>
        /// Waits the until value changed asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static async Task<T> WaitUntilValueChangedAsync<T>(this IReadOnlyReactiveProperty<T> source, CancellationToken cancellationToken = default)
        {
            using var handler = GetAsyncHandler<T>(source, cancellationToken);
            return await handler;
        }

        /// <summary>
        /// Waits the until value changed asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static async Task<T> WaitUntilValueChangedAsync<T>(this ReactiveProperty<T> source, CancellationToken cancellationToken = default)
        {
            using var handler = GetAsyncHandler<T>(source, cancellationToken);
            return await handler;
        }

        /// <summary>
        /// Waits the until value changed asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static async Task<T> WaitUntilValueChangedAsync<T>(this ReactivePropertySlim<T> source, CancellationToken cancellationToken = default)
        {
            using var handler = GetAsyncHandler<T>(source, cancellationToken);
            return await handler;
        }

        /// <summary>
        /// Waits the until value changed asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static async Task<T> WaitUntilValueChangedAsync<T>(this ReactiveCommand<T> source, CancellationToken cancellationToken = default)
        {
            using var handler = GetAsyncHandler<T>(source, cancellationToken);
            return await handler;
        }
    }
}
