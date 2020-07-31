using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using Reactive.Bindings.Internals;

namespace Reactive.Bindings
{
    /// <summary>
    /// Read only version ReactiveProperty.
    /// </summary>
    /// <typeparam name="T">Type of property.</typeparam>
    public class ReadOnlyReactiveProperty<T> : IReadOnlyReactiveProperty<T>, IObserverLinkedList<T>, IObserver<T>
    {
        private const int IsDisposedFlagNumber = 1 << 9; // (reserve 0 ~ 8)

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        /// <returns></returns>
        public event PropertyChangedEventHandler PropertyChanged;

        private T _latestValue;
        private IDisposable _sourceSubscription;
        private ReactivePropertyMode _mode; // None = 0, DistinctUntilChanged = 1, RaiseLatestValueOnSubscribe = 2, Disposed = (1 << 9)
        private readonly IEqualityComparer<T> _equalityComparer;
        private ObserverNode<T> _root;
        private ObserverNode<T> _last;
        private readonly IScheduler _raiseEventScheduler;

        private bool IsRaiseLatestValueOnSubscribe => (_mode & ReactivePropertyMode.RaiseLatestValueOnSubscribe) == ReactivePropertyMode.RaiseLatestValueOnSubscribe;
        private bool IsDistinctUntilChanged => (_mode & ReactivePropertyMode.DistinctUntilChanged) == ReactivePropertyMode.DistinctUntilChanged;
        private bool IsDisposeChangedValue => (_mode & ReactivePropertyMode.DisposeChangedValue) == ReactivePropertyMode.DisposeChangedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyReactiveProperty{T}"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="initialValue">The initial value.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="eventScheduler">The event scheduler.</param>
        /// <param name="equalityComparer">The equality comparer.</param>
        public ReadOnlyReactiveProperty(
            IObservable<T> source,
            T initialValue = default,
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe,
            IScheduler eventScheduler = null,
            IEqualityComparer<T> equalityComparer = null)
        {
            _latestValue = initialValue;
            _raiseEventScheduler = eventScheduler ?? ReactivePropertyScheduler.Default;
            _equalityComparer = equalityComparer ?? EqualityComparer<T>.Default;
            _mode = mode;
            _sourceSubscription = source.Subscribe(this);
            if (IsDisposed)
            {
                _sourceSubscription.Dispose();
                _sourceSubscription = null;
            }
        }

        /// <summary>
        /// Get latest value.
        /// </summary>
        public T Value => _latestValue;

        object IReadOnlyReactiveProperty.Value => Value;

        public bool IsDisposed => (int)_mode == IsDisposedFlagNumber;

        /// <summary>
        /// Notifies the provider that an observer is to receive notifications.
        /// </summary>
        /// <param name="observer">The object that is to receive notifications.</param>
        /// <returns>
        /// A reference to an interface that allows observers to stop receiving notifications before
        /// the provider has finished sending them.
        /// </returns>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (IsDisposed)
            {
                observer.OnCompleted();
                return Disposable.Empty;
            }

            if (IsRaiseLatestValueOnSubscribe)
            {
                observer.OnNext(_latestValue);
            }

            // subscribe node, node as subscription.
            var next = new ObserverNode<T>(this, observer);
            if (_root == null)
            {
                _root = _last = next;
            }
            else
            {
                _last.Next = next;
                next.Previous = _last;
                _last = next;
            }

            return next;
        }

        void IObserverLinkedList<T>.UnsubscribeNode(ObserverNode<T> node)
        {
            if (node == _root)
            {
                _root = node.Next;
            }
            if (node == _last)
            {
                _last = node.Previous;
            }

            if (node.Previous != null)
            {
                node.Previous.Next = node.Next;
            }
            if (node.Next != null)
            {
                node.Next.Previous = node.Previous;
            }
        }


        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            var node = _root;
            _root = _last = null;
            _mode = (ReactivePropertyMode)IsDisposedFlagNumber;

            while (node != null)
            {
                node.OnCompleted();
                node = node.Next;
            }

            _sourceSubscription?.Dispose();
            _sourceSubscription = null;
        }

        void IObserver<T>.OnNext(T value)
        {
            if (IsDisposed)
            {
                return;
            }

            if (IsDistinctUntilChanged && _equalityComparer.Equals(_latestValue, value))
            {
                return;
            }

            if (IsDisposeChangedValue && _latestValue is IDisposable disposable)
            {
                disposable.Dispose();
            }

            // SetValue
            _latestValue = value;

            // call source.OnNext
            var node = _root;
            while (node != null)
            {
                node.OnNext(value);
                node = node.Next;
            }

            // Notify changed.
            _raiseEventScheduler.Schedule(() => PropertyChanged?.Invoke(this, SingletonPropertyChangedEventArgs.Value));
        }

        void IObserver<T>.OnError(Exception error)
        {
            if (IsDisposeChangedValue && _latestValue is IDisposable disposable)
            {
                disposable.Dispose();
            }

            // do nothing.
        }

        void IObserver<T>.OnCompleted()
        {
            if (IsDisposeChangedValue && _latestValue is IDisposable disposable)
            {
                disposable.Dispose();
            }

            // oncompleted same as dispose.
            Dispose();
        }
    }

    /// <summary>
    /// ReadOnlyReactiveProperty factory methods.
    /// </summary>
    public static class ReadOnlyReactiveProperty
    {
        /// <summary>
        /// Create ReadOnlyReactiveProperty
        /// </summary>
        /// <typeparam name="T">Type of property.</typeparam>
        /// <param name="self">source stream</param>
        /// <param name="initialValue">initial push value</param>
        /// <param name="mode">ReactivePropertyMode. Default is DistinctUntilChanged | RaiseLatestValueOnSubscribe</param>
        /// <param name="eventScheduler">Scheduler of PropertyChanged event.</param>
        /// <param name="equalityComparer">The equality comparer.</param>
        /// <returns></returns>
        public static ReadOnlyReactiveProperty<T> ToReadOnlyReactiveProperty<T>(this IObservable<T> self,
            T initialValue = default,
            ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe,
            IScheduler eventScheduler = null,
            IEqualityComparer<T> equalityComparer = null) =>
            new ReadOnlyReactiveProperty<T>(
                self,
                initialValue,
                mode,
                eventScheduler,
                equalityComparer);
    }
}
