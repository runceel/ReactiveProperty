using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.ExceptionServices;
using Reactive.Bindings.Internals;

namespace Reactive.Bindings;

// This file includes ReactivePropertySlim and ReadOnlyReactivePropertySlim.

/// <summary>
/// </summary>
/// <typeparam name="T"></typeparam>
public class ReactivePropertySlim<T> : IReactiveProperty<T>, IObserverLinkedList<T>
{
    private const int IsDisposedFlagNumber = 1 << 9; // (reserve 0 ~ 8)

    // minimize field count
    private T _latestValue;

    private ReactivePropertyMode _mode; // None = 0, DistinctUntilChanged = 1, RaiseLatestValueOnSubscribe = 2, Disposed = (1 << 9)
    private readonly IEqualityComparer<T> _equalityComparer;
    private ObserverNode<T>? _root;
    private ObserverNode<T>? _last;

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    /// <returns></returns>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>The value.</value>
    public T Value
    {
        get
        {
            return _latestValue;
        }

        set
        {
            if (IsDistinctUntilChanged && _equalityComparer.Equals(_latestValue, value))
            {
                return;
            }

            // Note:can set null and can set after disposed.
            _latestValue = value;
            if (!IsDisposed)
            {
                OnNextAndRaiseValueChanged(ref value);
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is disposed.
    /// </summary>
    /// <value><c>true</c> if this instance is disposed; otherwise, <c>false</c>.</value>
    public bool IsDisposed => (int)_mode == IsDisposedFlagNumber;

    object? IReactiveProperty.Value
    {
        get
        {
            return Value!;
        }

        set
        {
            Value = (T)value!;
        }
    }

    object? IReadOnlyReactiveProperty.Value
    {
        get
        {
            return Value!;
        }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is distinct until changed.
    /// </summary>
    /// <value><c>true</c> if this instance is distinct until changed; otherwise, <c>false</c>.</value>
    public bool IsDistinctUntilChanged => (_mode & ReactivePropertyMode.DistinctUntilChanged) == ReactivePropertyMode.DistinctUntilChanged;

    /// <summary>
    /// Gets a value indicating whether this instance is raise latest value on subscribe.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is raise latest value on subscribe; otherwise, <c>false</c>.
    /// </value>
    public bool IsRaiseLatestValueOnSubscribe => (_mode & ReactivePropertyMode.RaiseLatestValueOnSubscribe) == ReactivePropertyMode.RaiseLatestValueOnSubscribe;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactivePropertySlim{T}"/> class.
    /// </summary>
    /// <param name="initialValue">The initial value.</param>
    /// <param name="mode">The mode.</param>
    /// <param name="equalityComparer">The equality comparer.</param>
    public ReactivePropertySlim(
#pragma warning disable CS8601 // Nullable
        T initialValue = default,
#pragma warning restore CS8601 // Nullable
        ReactivePropertyMode mode = ReactivePropertyMode.Default, 
        IEqualityComparer<T>? equalityComparer = null)
    {
        _latestValue = initialValue;
        _mode = mode;
        _equalityComparer = equalityComparer ?? EqualityComparer<T>.Default;
    }

    private void OnNextAndRaiseValueChanged(ref T value)
    {
        // call source.OnNext
        var node = _root;
        while (node != null)
        {
            node.OnNext(value);
            node = node.Next;
        }

        PropertyChanged?.Invoke(this, SingletonPropertyChangedEventArgs.Value);
    }

    /// <summary>
    /// Forces the notify.
    /// </summary>
    public void ForceNotify() => OnNextAndRaiseValueChanged(ref _latestValue);

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
            return InternalDisposable.Empty;
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
            _last!.Next = next;
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
    }

    /// <summary>
    /// Returns a <see cref="string"/> that represents this instance.
    /// </summary>
    /// <returns>A <see cref="string"/> that represents this instance.</returns>
    public override string? ToString()
    {
        return (_latestValue == null)
            ? "null"
            : _latestValue.ToString();
    }

    // NotSupported, return always true/empty.

    bool INotifyDataErrorInfo.HasErrors => false;

    IObservable<IEnumerable> IHasErrors.ObserveErrorChanged => throw new NotSupportedException();

    IObservable<bool> IHasErrors.ObserveHasErrors => throw new NotSupportedException();

    event EventHandler<DataErrorsChangedEventArgs>? INotifyDataErrorInfo.ErrorsChanged
    {
        add
        {
        }
        remove
        {
        }
    }

    IEnumerable INotifyDataErrorInfo.GetErrors(string? propertyName)
    {
        return System.Linq.Enumerable.Empty<object>();
    }

    protected virtual void OnPropertyChanged(string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

/// <summary>
/// </summary>
/// <typeparam name="T"></typeparam>
/// <seealso cref="Reactive.Bindings.IReadOnlyReactiveProperty{T}"/>
/// <seealso cref="Reactive.Bindings.Internals.IObserverLinkedList{T}"/>
/// <seealso cref="System.IObserver{T}"/>
public class ReadOnlyReactivePropertySlim<T> : IReadOnlyReactiveProperty<T>, IObserverLinkedList<T>, IObserver<T>
{
    private const int IsDisposedFlagNumber = 1 << 9; // (reserve 0 ~ 8)

    // minimize field count
    private T _latestValue;

    private IDisposable? _sourceSubscription;
    private ReactivePropertyMode _mode; // None = 0, DistinctUntilChanged = 1, RaiseLatestValueOnSubscribe = 2, Disposed = (1 << 9)
    private readonly IEqualityComparer<T> _equalityComparer;
    private ObserverNode<T>? _root;
    private ObserverNode<T>? _last;

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    /// <returns></returns>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <value>The value.</value>
    public T Value
    {
        get
        {
            return _latestValue;
        }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is disposed.
    /// </summary>
    /// <value><c>true</c> if this instance is disposed; otherwise, <c>false</c>.</value>
    public bool IsDisposed => (int)_mode == IsDisposedFlagNumber;

    object? IReadOnlyReactiveProperty.Value => Value;

    private bool IsDistinctUntilChanged => (_mode & ReactivePropertyMode.DistinctUntilChanged) == ReactivePropertyMode.DistinctUntilChanged;

    private bool IsRaiseLatestValueOnSubscribe => (_mode & ReactivePropertyMode.RaiseLatestValueOnSubscribe) == ReactivePropertyMode.RaiseLatestValueOnSubscribe;

    private bool IsIgnoreException => (_mode & ReactivePropertyMode.IgnoreException) == ReactivePropertyMode.IgnoreException;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyReactivePropertySlim{T}"/> class.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="initialValue">The initial value.</param>
    /// <param name="mode">The mode.</param>
    /// <param name="equalityComparer">The equality comparer.</param>
    public ReadOnlyReactivePropertySlim(
        IObservable<T> source,
#pragma warning disable CS8601 // nullable
        T initialValue = default,
#pragma warning restore CS8601 // nullable
        ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe, 
        IEqualityComparer<T>? equalityComparer = null)
    {
        _latestValue = initialValue;
        _mode = mode;
        _equalityComparer = equalityComparer ?? EqualityComparer<T>.Default;
        _sourceSubscription = source.Subscribe(this);
        if (IsDisposed)
        {
            _sourceSubscription.Dispose();
            _sourceSubscription = null;
        }
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
        if (IsDisposed)
        {
            observer.OnCompleted();
            return InternalDisposable.Empty;
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
            _last!.Next = next;
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
        PropertyChanged?.Invoke(this, SingletonPropertyChangedEventArgs.Value);
    }

    void IObserver<T>.OnError(Exception error)
    {
        if (IsIgnoreException) return;
        ExceptionDispatchInfo.Capture(error).Throw();
    }

    void IObserver<T>.OnCompleted()
    {
        // oncompleted same as dispose.
        Dispose();
    }

    /// <summary>
    /// Returns a <see cref="string"/> that represents this instance.
    /// </summary>
    /// <returns>A <see cref="string"/> that represents this instance.</returns>
    public override string? ToString()
    {
        return (_latestValue == null)
            ? "null"
            : _latestValue.ToString();
    }
}

/// <summary>
/// </summary>
/// <seealso cref="Reactive.Bindings.IReadOnlyReactiveProperty{T}"/>
/// <seealso cref="Reactive.Bindings.Internals.IObserverLinkedList{T}"/>
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
    public static ReadOnlyReactivePropertySlim<T> ToReadOnlyReactivePropertySlim<T>(
        this IObservable<T> source,
#pragma warning disable CS8601 // Nullable
        T initialValue = default,
#pragma warning restore CS8601 // Nullable
        ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe, 
        IEqualityComparer<T>? equalityComparer = null)
    {
        return new ReadOnlyReactivePropertySlim<T>(source, initialValue, mode, equalityComparer);
    }
}
