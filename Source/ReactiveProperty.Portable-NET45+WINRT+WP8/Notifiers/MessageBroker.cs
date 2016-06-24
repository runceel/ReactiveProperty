using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Reactive.Bindings.Notifiers
{
    /// <summary>
    /// In-Memory PubSub filtered by Type.
    /// </summary>
    public interface IMessagePublisher
    {
        /// <summary>
        /// Send Message to all receiver.
        /// </summary>
        void Publish<T>(T message);
    }

    /// <summary>
    /// In-Memory PubSub filtered by Type.
    /// </summary>
    public interface IMessageSubscriber
    {
        /// <summary>
        /// Subscribe typed message.
        /// </summary>
        IDisposable Subscribe<T>(Action<T> action);
    }

    /// <summary>
    /// In-Memory PubSub filtered by Type.
    /// </summary>
    public interface IMessageBroker : IMessagePublisher, IMessageSubscriber
    {
    }

    /// <summary>
    /// In-Memory PubSub filtered by Type.
    /// </summary>
    public interface IAsyncMessagePublisher
    {
        /// <summary>
        /// Send Message to all receiver and await complete.
        /// </summary>
        Task PublishAsync<T>(T message);
    }

    /// <summary>
    /// In-Memory PubSub filtered by Type.
    /// </summary>
    public interface IAsyncMessageSubscriber
    {
        /// <summary>
        /// Subscribe typed message.
        /// </summary>
        IDisposable Subscribe<T>(Func<T, Task> asyncAction);
    }

    /// <summary>
    /// In-Memory PubSub filtered by Type.
    /// </summary>
    public interface IAsyncMessageBroker : IAsyncMessagePublisher, IAsyncMessageSubscriber
    {
    }

    /// <summary>
    /// In-Memory PubSub filtered by Type.
    /// </summary>
    public class MessageBroker : IMessageBroker, IDisposable
    {
        /// <summary>
        /// MessageBroker in Global scope.
        /// </summary>
        public static readonly IMessageBroker Default = new MessageBroker();

        bool isDisposed = false;
        readonly Dictionary<Type, object> notifiers = new Dictionary<Type, object>();

        /// <summary>
        /// Send Message to all receiver.
        /// </summary>
        public void Publish<T>(T message)
        {
            ImmutableList<Action<T>> notifier;
            lock (notifiers)
            {
                if (isDisposed) throw new ObjectDisposedException("AsyncMessageBroker");

                object _notifier;
                if (notifiers.TryGetValue(typeof(T), out _notifier))
                {
                    notifier = (ImmutableList<Action<T>>)_notifier;
                }
                else
                {
                    return;
                }
            }

            var data = notifier.Data;
            for (int i = 0; i < data.Length; i++)
            {
                data[i].Invoke(message);
            }
        }

        /// <summary>
        /// Subscribe typed message.
        /// </summary>
        public IDisposable Subscribe<T>(Action<T> action)
        {
            lock (notifiers)
            {
                if (isDisposed) throw new ObjectDisposedException("MessageBroker");

                object _notifier;
                if (!notifiers.TryGetValue(typeof(T), out _notifier))
                {
                    var notifier = ImmutableList<Action<T>>.Empty;
                    notifier = notifier.Add(action);
                    notifiers.Add(typeof(T), notifier);
                }
                else
                {
                    var notifier = (ImmutableList<Action<T>>)_notifier;
                    notifier = notifier.Add(action);
                    notifiers[typeof(T)] = notifier;
                }
            }

            return new Subscription<T>(this, action);
        }

        /// <summary>
        /// Stop Pub-Sub system.
        /// </summary>
        public void Dispose()
        {
            lock (notifiers)
            {
                if (!isDisposed)
                {
                    isDisposed = true;
                    notifiers.Clear();
                }
            }
        }

        class Subscription<T> : IDisposable
        {
            readonly MessageBroker parent;
            readonly Action<T> action;

            public Subscription(MessageBroker parent, Action<T> action)
            {
                this.parent = parent;
                this.action = action;
            }

            public void Dispose()
            {
                lock (parent.notifiers)
                {
                    object _notifier;
                    if (parent.notifiers.TryGetValue(typeof(T), out _notifier))
                    {
                        var notifier = (ImmutableList<Action<T>>)_notifier;
                        notifier = notifier.Remove(action);

                        parent.notifiers[typeof(T)] = notifier;
                    }
                }
            }
        }
    }

    /// <summary>
    /// In-Memory PubSub filtered by Type.
    /// </summary>
    public class AsyncMessageBroker : IAsyncMessageBroker, IDisposable
    {
        static readonly Task EmptyTask = Task.FromResult<object>(null);

        /// <summary>
        /// AsyncMessageBroker in Global scope.
        /// </summary>
        public static readonly IAsyncMessageBroker Default = new AsyncMessageBroker();

        bool isDisposed = false;
        readonly Dictionary<Type, object> notifiers = new Dictionary<Type, object>();

        /// <summary>
        /// Send Message to all receiver and await complete.
        /// </summary>
        public Task PublishAsync<T>(T message)
        {
            ImmutableList<Func<T, Task>> notifier;
            lock (notifiers)
            {
                if (isDisposed) throw new ObjectDisposedException("AsyncMessageBroker");

                object _notifier;
                if (notifiers.TryGetValue(typeof(T), out _notifier))
                {
                    notifier = (ImmutableList<Func<T, Task>>)_notifier;
                }
                else
                {
                    return EmptyTask;
                }
            }

            var data = notifier.Data;
            var awaiter = new Task[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                awaiter[i] = data[i].Invoke(message);
            }
            return Task.WhenAll(awaiter);
        }

        /// <summary>
        /// Subscribe typed message.
        /// </summary>
        public IDisposable Subscribe<T>(Func<T, Task> asyncAction)
        {
            lock (notifiers)
            {
                if (isDisposed) throw new ObjectDisposedException("AsyncMessageBroker");

                object _notifier;
                if (!notifiers.TryGetValue(typeof(T), out _notifier))
                {
                    var notifier = ImmutableList<Func<T, Task>>.Empty;
                    notifier = notifier.Add(asyncAction);
                    notifiers.Add(typeof(T), notifier);
                }
                else
                {
                    var notifier = (ImmutableList<Func<T, Task>>)_notifier;
                    notifier = notifier.Add(asyncAction);
                    notifiers[typeof(T)] = notifier;
                }
            }

            return new Subscription<T>(this, asyncAction);
        }

        /// <summary>
        /// Stop Pub-Sub system.
        /// </summary>
        public void Dispose()
        {
            lock (notifiers)
            {
                if (!isDisposed)
                {
                    isDisposed = true;
                    notifiers.Clear();
                }
            }
        }

        class Subscription<T> : IDisposable
        {
            readonly AsyncMessageBroker parent;
            readonly Func<T, Task> asyncAction;

            public Subscription(AsyncMessageBroker parent, Func<T, Task> asyncAction)
            {
                this.parent = parent;
                this.asyncAction = asyncAction;
            }

            public void Dispose()
            {
                lock (parent.notifiers)
                {
                    object _notifier;
                    if (parent.notifiers.TryGetValue(typeof(T), out _notifier))
                    {
                        var notifier = (ImmutableList<Func<T, Task>>)_notifier;
                        notifier = notifier.Remove(asyncAction);

                        parent.notifiers[typeof(T)] = notifier;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Extensions of MessageBroker.
    /// </summary>
    public static class MessageBrokerExtensions
    {
        /// <summary>
        /// Convert IMessageSubscriber.Subscribe to Observable.
        /// </summary>
        public static IObservable<T> ToObservable<T>(this IMessageSubscriber messageSubscriber)
        {
            return Observable.Create<T>(observer =>
            {
                var gate = new object();
                var d = messageSubscriber.Subscribe<T>(x =>
                {
                    // needs synchronize
                    lock (gate)
                    {
                        observer.OnNext(x);
                    }
                });
                return d;
            });
        }
    }

    // ImmutableList is from Rx internal
    internal class ImmutableList<T>
    {
        public static readonly ImmutableList<T> Empty = new ImmutableList<T>();

        T[] data;

        public T[] Data
        {
            get { return data; }
        }

        ImmutableList()
        {
            data = new T[0];
        }

        public ImmutableList(T[] data)
        {
            this.data = data;
        }

        public ImmutableList<T> Add(T value)
        {
            var newData = new T[data.Length + 1];
            Array.Copy(data, newData, data.Length);
            newData[data.Length] = value;
            return new ImmutableList<T>(newData);
        }

        public ImmutableList<T> Remove(T value)
        {
            var i = IndexOf(value);
            if (i < 0) return this;

            var length = data.Length;
            if (length == 1) return Empty;

            var newData = new T[length - 1];

            Array.Copy(data, 0, newData, 0, i);
            Array.Copy(data, i + 1, newData, i, length - i - 1);

            return new ImmutableList<T>(newData);
        }

        public int IndexOf(T value)
        {
            for (var i = 0; i < data.Length; ++i)
            {
                if (object.Equals(data[i], value)) return i;
            }
            return -1;
        }
    }
}