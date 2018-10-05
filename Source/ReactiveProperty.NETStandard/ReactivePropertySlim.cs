using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Threading;

namespace Reactive.Bindings
{
    // This file includes ReactivePropertySlim and ReadOnlyReactivePropertySlim.

    internal interface IObserverLinkedList<T>
    {
        void UnsubscribeNode(ObserverNode<T> node);
    }

    internal sealed class ObserverNode<T> : IObserver<T>, IDisposable
    {
        private readonly IObserver<T> observer;
        private IObserverLinkedList<T> list;

        public ObserverNode<T> Previous { get; internal set; }

        public ObserverNode<T> Next { get; internal set; }

        public ObserverNode(IObserverLinkedList<T> list, IObserver<T> observer)
        {
            this.list = list;
            this.observer = observer;
        }

        public void OnNext(T value)
        {
            observer.OnNext(value);
        }

        public void OnError(Exception error)
        {
            observer.OnError(error);
        }

        public void OnCompleted()
        {
            observer.OnCompleted();
        }

        public void Dispose()
        {
            var sourceList = Interlocked.Exchange(ref list, null);
            if (sourceList != null) {
                sourceList.UnsubscribeNode(this);
                sourceList = null;
            }
        }
    }

    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ReactivePropertySlim<T> : IReactiveProperty<T>, IReadOnlyReactiveProperty<T>, IObserverLinkedList<T>
    {
        private const int IsDisposedFlagNumber = 1 << 9; // (reserve 0 ~ 8)

        // minimize field count
        private T latestValue;

        private ReactivePropertyMode mode; // None = 0, DistinctUntilChanged = 1, RaiseLatestValueOnSubscribe = 2, Disposed = (1 << 9)
        private readonly IEqualityComparer<T> equalityComparer;
        private ObserverNode<T> root;
        private ObserverNode<T> last;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        /// <returns></returns>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public T Value
        {
            get
            {
                return latestValue;
            }

            set
            {
                if (IsDistinctUntilChanged && equalityComparer.Equals(latestValue, value)) {
                    return;
                }

                // Note:can set null and can set after disposed.
                latestValue = value;
                if (!IsDisposed) {
                    OnNextAndRaiseValueChanged(ref value);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        /// <value><c>true</c> if this instance is disposed; otherwise, <c>false</c>.</value>
        public bool IsDisposed => (int)mode == IsDisposedFlagNumber;

        object IReactiveProperty.Value
        {
            get
            {
                return Value;
            }

            set
            {
                Value = (T)value;
            }
        }

        object IReadOnlyReactiveProperty.Value
        {
            get
            {
                return Value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is distinct until changed.
        /// </summary>
        /// <value><c>true</c> if this instance is distinct until changed; otherwise, <c>false</c>.</value>
        public bool IsDistinctUntilChanged => (mode & ReactivePropertyMode.DistinctUntilChanged) == ReactivePropertyMode.DistinctUntilChanged;

        /// <summary>
        /// Gets a value indicating whether this instance is raise latest value on subscribe.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is raise latest value on subscribe; otherwise, <c>false</c>.
        /// </value>
        public bool IsRaiseLatestValueOnSubscribe => (mode & ReactivePropertyMode.RaiseLatestValueOnSubscribe) == ReactivePropertyMode.RaiseLatestValueOnSubscribe;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactivePropertySlim{T}"/> class.
        /// </summary>
        /// <param name="initialValue">The initial value.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="equalityComparer">The equality comparer.</param>
        public ReactivePropertySlim(T initialValue = default(T), ReactivePropertyMode mode = ReactivePropertyMode.Default, IEqualityComparer<T> equalityComparer = null)
        {
            latestValue = initialValue;
            this.mode = mode;
            this.equalityComparer = equalityComparer ?? EqualityComparer<T>.Default;
        }

        private void OnNextAndRaiseValueChanged(ref T value)
        {
            // call source.OnNext
            var node = root;
            while (node != null) {
                node.OnNext(value);
                node = node.Next;
            }

            PropertyChanged?.Invoke(this, SingletonPropertyChangedEventArgs.Value);
        }

        /// <summary>
        /// Forces the notify.
        /// </summary>
        public void ForceNotify()
        {
            OnNextAndRaiseValueChanged(ref latestValue);
        }

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
            if (IsDisposed) {
                observer.OnCompleted();
                return Disposable.Empty;
            }

            if (IsRaiseLatestValueOnSubscribe) {
                observer.OnNext(latestValue);
            }

            // subscribe node, node as subscription.
            var next = new ObserverNode<T>(this, observer);
            if (root == null) {
                root = last = next;
            } else {
                last.Next = next;
                next.Previous = last;
                last = next;
            }
            return next;
        }

        void IObserverLinkedList<T>.UnsubscribeNode(ObserverNode<T> node)
        {
            if (node == root) {
                root = node.Next;
            }
            if (node == last) {
                last = node.Previous;
            }

            if (node.Previous != null) {
                node.Previous.Next = node.Next;
            }
            if (node.Next != null) {
                node.Next.Previous = node.Previous;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed) {
                return;
            }

            var node = root;
            root = last = null;
            mode = (ReactivePropertyMode)IsDisposedFlagNumber;

            while (node != null) {
                node.OnCompleted();
                node = node.Next;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString()
        {
            return (latestValue == null)
                ? "null"
                : latestValue.ToString();
        }

        // NotSupported, return always true/empty.

        bool INotifyDataErrorInfo.HasErrors => false;

        IObservable<IEnumerable> IHasErrors.ObserveErrorChanged => throw new NotSupportedException();

        IObservable<bool> IHasErrors.ObserveHasErrors => throw new NotSupportedException();

        event EventHandler<DataErrorsChangedEventArgs> INotifyDataErrorInfo.ErrorsChanged
        {
            add
            {
            }
            remove
            {
            }
        }

        IEnumerable INotifyDataErrorInfo.GetErrors(string propertyName)
        {
            return System.Linq.Enumerable.Empty<object>();
        }
    }

    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Reactive.Bindings.IReadOnlyReactiveProperty{T}"/>
    /// <seealso cref="Reactive.Bindings.IObserverLinkedList{T}"/>
    /// <seealso cref="System.IObserver{T}"/>
    public class ReadOnlyReactivePropertySlim<T> : IReadOnlyReactiveProperty<T>, IObserverLinkedList<T>, IObserver<T>
    {
        private const int IsDisposedFlagNumber = 1 << 9; // (reserve 0 ~ 8)

        // minimize field count
        private T latestValue;

        private IDisposable sourceSubscription;
        private ReactivePropertyMode mode; // None = 0, DistinctUntilChanged = 1, RaiseLatestValueOnSubscribe = 2, Disposed = (1 << 9)
        private readonly IEqualityComparer<T> equalityComparer;
        private ObserverNode<T> root;
        private ObserverNode<T> last;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        /// <returns></returns>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public T Value
        {
            get
            {
                return latestValue;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        /// <value><c>true</c> if this instance is disposed; otherwise, <c>false</c>.</value>
        public bool IsDisposed => (int)mode == IsDisposedFlagNumber;

        object IReadOnlyReactiveProperty.Value
        {
            get
            {
                return Value;
            }
        }

        private bool IsDistinctUntilChanged => (mode & ReactivePropertyMode.DistinctUntilChanged) == ReactivePropertyMode.DistinctUntilChanged;

        private bool IsRaiseLatestValueOnSubscribe => (mode & ReactivePropertyMode.RaiseLatestValueOnSubscribe) == ReactivePropertyMode.RaiseLatestValueOnSubscribe;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyReactivePropertySlim{T}"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="initialValue">The initial value.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="equalityComparer">The equality comparer.</param>
        public ReadOnlyReactivePropertySlim(IObservable<T> source, T initialValue = default(T), ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe, IEqualityComparer<T> equalityComparer = null)
        {
            latestValue = initialValue;
            this.mode = mode;
            this.equalityComparer = equalityComparer ?? EqualityComparer<T>.Default;
            sourceSubscription = source.Subscribe(this);
        }

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
            if (IsDisposed) {
                observer.OnCompleted();
                return Disposable.Empty;
            }

            if (IsRaiseLatestValueOnSubscribe) {
                observer.OnNext(latestValue);
            }

            // subscribe node, node as subscription.
            var next = new ObserverNode<T>(this, observer);
            if (root == null) {
                root = last = next;
            } else {
                last.Next = next;
                next.Previous = last;
                last = next;
            }

            return next;
        }

        void IObserverLinkedList<T>.UnsubscribeNode(ObserverNode<T> node)
        {
            if (node == root) {
                root = node.Next;
            }
            if (node == last) {
                last = node.Previous;
            }

            if (node.Previous != null) {
                node.Previous.Next = node.Next;
            }
            if (node.Next != null) {
                node.Next.Previous = node.Previous;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed) {
                return;
            }

            var node = root;
            root = last = null;
            mode = (ReactivePropertyMode)IsDisposedFlagNumber;

            while (node != null) {
                node.OnCompleted();
                node = node.Next;
            }
            sourceSubscription.Dispose();
            sourceSubscription = null;
        }

        void IObserver<T>.OnNext(T value)
        {
            if (IsDisposed) {
                return;
            }

            if (IsDistinctUntilChanged && equalityComparer.Equals(latestValue, value)) {
                return;
            }

            // SetValue
            latestValue = value;

            // call source.OnNext
            var node = root;
            while (node != null) {
                node.OnNext(value);
                node = node.Next;
            }

            // Notify changed.
            PropertyChanged?.Invoke(this, SingletonPropertyChangedEventArgs.Value);
        }

        void IObserver<T>.OnError(Exception error)
        {
            // do nothing.
        }

        void IObserver<T>.OnCompleted()
        {
            // oncompleted same as dispose.
            Dispose();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString()
        {
            return (latestValue == null)
                ? "null"
                : latestValue.ToString();
        }
    }

    /// <summary>
    /// </summary>
    /// <seealso cref="Reactive.Bindings.IReadOnlyReactiveProperty{T}"/>
    /// <seealso cref="Reactive.Bindings.IObserverLinkedList{T}"/>
    /// <seealso cref="System.IObserver{T}"/>
    public static class ReadOnlyReactivePropertySlim
    {
        /// <summary>
        /// To the read only reactive property slim.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="initialValue">The initial value.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="equalityComparer">The equality comparer.</param>
        /// <returns></returns>
        public static ReadOnlyReactivePropertySlim<T> ToReadOnlyReactivePropertySlim<T>(this IObservable<T> source, T initialValue = default(T), ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe, IEqualityComparer<T> equalityComparer = null)
        {
            return new ReadOnlyReactivePropertySlim<T>(source, initialValue, mode, equalityComparer);
        }
    }
}
