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
        readonly IObserver<T> observer;
        IObserverLinkedList<T> list;

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
            if (sourceList != null)
            {
                sourceList.UnsubscribeNode(this);
                sourceList = null;
            }
        }
    }

    public class ReactivePropertySlim<T> : IReactiveProperty<T>, IReadOnlyReactiveProperty<T>, IObserverLinkedList<T>
    {
        const int IsDisposedFlagNumber = 1 << 9; // (reserve 0 ~ 8)

        // minimize field count
        T latestValue;
        ReactivePropertyMode mode; // None = 0, DistinctUntilChanged = 1, RaiseLatestValueOnSubscribe = 2, Disposed = (1 << 9)
        readonly IEqualityComparer<T> equalityComparer;
        ObserverNode<T> root;
        ObserverNode<T> last;

        public event PropertyChangedEventHandler PropertyChanged;

        public T Value
        {
            get
            {
                return latestValue;
            }
            set
            {
                if (IsDistinctUntilChanged && equalityComparer.Equals(latestValue, value))
                {
                    return;
                }

                // Note:can set null and can set after disposed.
                this.latestValue = value;
                if (!IsDisposed)
                {
                    OnNextAndRaiseValueChanged(ref value);
                }
            }
        }

        public bool IsDisposed => (int)mode == IsDisposedFlagNumber;

        object IReactiveProperty.Value
        {
            get
            {
                return (object)Value;
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
                return (object)Value;
            }
        }

        public bool IsDistinctUntilChanged => (mode & ReactivePropertyMode.DistinctUntilChanged) == ReactivePropertyMode.DistinctUntilChanged;
        public bool IsRaiseLatestValueOnSubscribe => (mode & ReactivePropertyMode.RaiseLatestValueOnSubscribe) == ReactivePropertyMode.RaiseLatestValueOnSubscribe;

        public ReactivePropertySlim(T initialValue = default(T), ReactivePropertyMode mode = ReactivePropertyMode.Default, IEqualityComparer<T> equalityComparer = null)
        {
            this.latestValue = initialValue;
            this.mode = mode;
            this.equalityComparer = equalityComparer ?? EqualityComparer<T>.Default;
        }

        void OnNextAndRaiseValueChanged(ref T value)
        {
            // call source.OnNext
            var node = root;
            while (node != null)
            {
                node.OnNext(value);
                node = node.Next;
            }

            this.PropertyChanged?.Invoke(this, SingletonPropertyChangedEventArgs.Value);
            awaiter?.InvokeContinuation(ref value);
        }

        public void ForceNotify()
        {
            OnNextAndRaiseValueChanged(ref latestValue);
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (IsDisposed)
            {
                observer.OnCompleted();
                return Disposable.Empty;
            }

            if (IsRaiseLatestValueOnSubscribe)
            {
                observer.OnNext(this.latestValue);
            }

            // subscribe node, node as subscription.
            var next = new ObserverNode<T>(this, observer);
            if (root == null)
            {
                root = last = next;
            }
            else
            {
                last.Next = next;
                next.Previous = last;
                last = next;
            }
            return next;
        }

        void IObserverLinkedList<T>.UnsubscribeNode(ObserverNode<T> node)
        {
            if (node == root)
            {
                root = node.Next;
            }
            if (node == last)
            {
                last = node.Previous;
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

        public void Dispose()
        {
            if (IsDisposed) return;

            var node = root;
            root = last = null;
            mode = (ReactivePropertyMode)IsDisposedFlagNumber;

            while (node != null)
            {
                node.OnCompleted();
                node = node.Next;
            }
        }

        public override string ToString()
        {
            return (latestValue == null)
                ? "null"
                : latestValue.ToString();
        }

        // async extension

        ReactivePropertyAwaiter<T> awaiter;

        public ReactivePropertyAwaiter<T> GetAwaiter()
        {
            if (awaiter != null) return awaiter;
            Interlocked.CompareExchange(ref awaiter, new ReactivePropertyAwaiter<T>(), null);
            return awaiter;
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

    public class ReadOnlyReactivePropertySlim<T> : IReadOnlyReactiveProperty<T>, IObserverLinkedList<T>, IObserver<T>
    {
        const int IsDisposedFlagNumber = 1 << 9; // (reserve 0 ~ 8)

        // minimize field count
        T latestValue;
        IDisposable sourceSubscription;
        ReactivePropertyMode mode; // None = 0, DistinctUntilChanged = 1, RaiseLatestValueOnSubscribe = 2, Disposed = (1 << 9)
        readonly IEqualityComparer<T> equalityComparer;

        ObserverNode<T> root;
        ObserverNode<T> last;

        public event PropertyChangedEventHandler PropertyChanged;

        public T Value
        {
            get
            {
                return latestValue;
            }
        }

        public bool IsDisposed => (int)mode == IsDisposedFlagNumber;

        object IReadOnlyReactiveProperty.Value
        {
            get
            {
                return (object)Value;
            }
        }

        bool IsDistinctUntilChanged => (mode & ReactivePropertyMode.DistinctUntilChanged) == ReactivePropertyMode.DistinctUntilChanged;
        bool IsRaiseLatestValueOnSubscribe => (mode & ReactivePropertyMode.RaiseLatestValueOnSubscribe) == ReactivePropertyMode.RaiseLatestValueOnSubscribe;

        public ReadOnlyReactivePropertySlim(IObservable<T> source, T initialValue = default(T), ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe, IEqualityComparer<T> equalityComparer = null)
        {
            this.latestValue = initialValue;
            this.mode = mode;
            this.equalityComparer = equalityComparer ?? EqualityComparer<T>.Default;
            this.sourceSubscription = source.Subscribe(this);
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (IsDisposed)
            {
                observer.OnCompleted();
                return Disposable.Empty;
            }

            if (IsRaiseLatestValueOnSubscribe)
            {
                observer.OnNext(latestValue);
            }

            // subscribe node, node as subscription.
            var next = new ObserverNode<T>(this, observer);
            if (root == null)
            {
                root = last = next;
            }
            else
            {
                last.Next = next;
                next.Previous = last;
                last = next;
            }

            return next;
        }

        void IObserverLinkedList<T>.UnsubscribeNode(ObserverNode<T> node)
        {
            if (node == root)
            {
                root = node.Next;
            }
            if (node == last)
            {
                last = node.Previous;
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

        public void Dispose()
        {
            if (IsDisposed) return;

            var node = root;
            root = last = null;
            mode = (ReactivePropertyMode)IsDisposedFlagNumber;

            while (node != null)
            {
                node.OnCompleted();
                node = node.Next;
            }
            sourceSubscription.Dispose();
            sourceSubscription = null;
        }

        void IObserver<T>.OnNext(T value)
        {
            if (IsDisposed) return;

            if (IsDistinctUntilChanged && equalityComparer.Equals(latestValue, value))
            {
                return;
            }

            // SetValue
            this.latestValue = value;

            // call source.OnNext
            var node = root;
            while (node != null)
            {
                node.OnNext(value);
                node = node.Next;
            }

            // Notify changed.
            this.PropertyChanged?.Invoke(this, SingletonPropertyChangedEventArgs.Value);

            awaiter?.InvokeContinuation(ref value);
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

        public override string ToString()
        {
            return (latestValue == null)
                ? "null"
                : latestValue.ToString();
        }

        // async extension

        ReactivePropertyAwaiter<T> awaiter;

        public ReactivePropertyAwaiter<T> GetAwaiter()
        {
            if (awaiter != null) return awaiter;
            Interlocked.CompareExchange(ref awaiter, new ReactivePropertyAwaiter<T>(), null);
            return awaiter;
        }
    }

    public static class ReadOnlyReactivePropertySlim
    {
        public static ReadOnlyReactivePropertySlim<T> ToReadOnlyReactivePropertySlim<T>(this IObservable<T> source, T initialValue = default(T), ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe, IEqualityComparer<T> equalityComparer = null)
        {
            return new ReadOnlyReactivePropertySlim<T>(source, initialValue, mode, equalityComparer);
        }
    }
}