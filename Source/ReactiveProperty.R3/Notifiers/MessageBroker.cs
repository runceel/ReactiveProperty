using System;
using System.Collections.Generic;

namespace Reactive.Bindings.R3.Notifiers;

/// <summary>
/// In-memory pub/sub broker filtered by type. R3-only, self-contained implementation.
/// </summary>
public class MessageBroker : IMessageBroker, IDisposable
{
    /// <summary>
    /// The global-scope <see cref="MessageBroker"/>.
    /// </summary>
    public static readonly IMessageBroker Default = new MessageBroker();

    private readonly Dictionary<Type, object> _notifiers = new();
    private bool _isDisposed;

    /// <inheritdoc />
    public void Publish<T>(T message)
    {
        Action<T>[] handlers;
        lock (_notifiers)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(MessageBroker));
            }

            if (!_notifiers.TryGetValue(typeof(T), out var notifier))
            {
                return;
            }

            handlers = ((List<Action<T>>)notifier).ToArray();
        }

        foreach (var handler in handlers)
        {
            handler.Invoke(message);
        }
    }

    /// <inheritdoc />
    public IDisposable Subscribe<T>(Action<T> action)
    {
        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        lock (_notifiers)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(MessageBroker));
            }

            if (!_notifiers.TryGetValue(typeof(T), out var notifier))
            {
                notifier = new List<Action<T>>();
                _notifiers.Add(typeof(T), notifier);
            }

            ((List<Action<T>>)notifier).Add(action);
        }

        return new Subscription<T>(this, action);
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
        private readonly MessageBroker _parent;
        private readonly Action<T> _action;

        public Subscription(MessageBroker parent, Action<T> action)
        {
            _parent = parent;
            _action = action;
        }

        public void Dispose()
        {
            lock (_parent._notifiers)
            {
                if (_parent._notifiers.TryGetValue(typeof(T), out var notifier))
                {
                    ((List<Action<T>>)notifier).Remove(_action);
                }
            }
        }
    }
}
