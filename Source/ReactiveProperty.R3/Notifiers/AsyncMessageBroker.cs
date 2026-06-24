using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reactive.Bindings.R3.Notifiers;

/// <summary>
/// In-memory asynchronous pub/sub broker filtered by type. R3-only, self-contained implementation.
/// </summary>
public class AsyncMessageBroker : IAsyncMessageBroker, IDisposable
{
    /// <summary>
    /// The global-scope <see cref="AsyncMessageBroker"/>.
    /// </summary>
    public static readonly IAsyncMessageBroker Default = new AsyncMessageBroker();

    private readonly Dictionary<Type, object> _notifiers = new();
    private bool _isDisposed;

    /// <inheritdoc />
    public Task PublishAsync<T>(T message)
    {
        Func<T, Task>[] handlers;
        lock (_notifiers)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(AsyncMessageBroker));
            }

            if (!_notifiers.TryGetValue(typeof(T), out var notifier))
            {
                return Task.CompletedTask;
            }

            handlers = ((List<Func<T, Task>>)notifier).ToArray();
        }

        var tasks = new Task[handlers.Length];
        for (var i = 0; i < handlers.Length; i++)
        {
            tasks[i] = handlers[i].Invoke(message);
        }

        return Task.WhenAll(tasks);
    }

    /// <inheritdoc />
    public IDisposable Subscribe<T>(Func<T, Task> asyncAction)
    {
        if (asyncAction is null)
        {
            throw new ArgumentNullException(nameof(asyncAction));
        }

        lock (_notifiers)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(AsyncMessageBroker));
            }

            if (!_notifiers.TryGetValue(typeof(T), out var notifier))
            {
                notifier = new List<Func<T, Task>>();
                _notifiers.Add(typeof(T), notifier);
            }

            ((List<Func<T, Task>>)notifier).Add(asyncAction);
        }

        return new Subscription<T>(this, asyncAction);
    }

    /// <summary>
    /// Stops the pub/sub system.
    /// </summary>
    public void Dispose()
    {
        lock (_notifiers)
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
                _notifiers.Clear();
            }
        }
    }

    private sealed class Subscription<T> : IDisposable
    {
        private readonly AsyncMessageBroker _parent;
        private readonly Func<T, Task> _asyncAction;

        public Subscription(AsyncMessageBroker parent, Func<T, Task> asyncAction)
        {
            _parent = parent;
            _asyncAction = asyncAction;
        }

        public void Dispose()
        {
            lock (_parent._notifiers)
            {
                if (_parent._notifiers.TryGetValue(typeof(T), out var notifier))
                {
                    ((List<Func<T, Task>>)notifier).Remove(_asyncAction);
                }
            }
        }
    }
}
