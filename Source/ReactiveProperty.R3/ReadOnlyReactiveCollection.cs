using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using R3;
using Reactive.Bindings.R3.Extensions;

namespace Reactive.Bindings.R3;

/// <summary>
/// Read-only reactive collection backed by an R3 observable change stream.
/// </summary>
/// <typeparam name="T">Element type.</typeparam>
public sealed class ReadOnlyReactiveCollection<T> : ReadOnlyObservableCollection<T>, IDisposable
{
    private readonly object _syncRoot = new();
    private readonly ObservableCollection<T> _source;
    private readonly bool _disposeElement;
    private readonly SynchronizationContext? _raiseEventContext;
    private readonly List<IDisposable> _subscriptions = new();
    private bool _isSuppressingEvents;
    private bool _isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyReactiveCollection{T}"/> class.
    /// </summary>
    public ReadOnlyReactiveCollection(
        Observable<CollectionChanged<T>> observable,
        ObservableCollection<T> source,
        bool disposeElement = true,
        SynchronizationContext? raiseEventContext = null)
        : base(source)
    {
        _source = source;
        _disposeElement = disposeElement;
        _raiseEventContext = raiseEventContext;
        _subscriptions.Add(observable.Subscribe(ApplyCollectionChanged));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyReactiveCollection{T}"/> class.
    /// </summary>
    public ReadOnlyReactiveCollection(
        Observable<T> observable,
        ObservableCollection<T> source,
        Observable<Unit>? onReset = null,
        bool disposeElement = true,
        SynchronizationContext? raiseEventContext = null)
        : base(source)
    {
        _source = source;
        _disposeElement = disposeElement;
        _raiseEventContext = raiseEventContext;
        _subscriptions.Add(observable.Subscribe(value => ApplyCollectionChanged(CollectionChanged<T>.Add(-1, value))));
        if (onReset is not null)
        {
            _subscriptions.Add(onReset.Subscribe(_ => ApplyCollectionChanged(CollectionChanged<T>.Reset)));
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;
        foreach (var subscription in _subscriptions.ToArray())
        {
            subscription.Dispose();
        }

        _subscriptions.Clear();
        if (_disposeElement)
        {
            foreach (var item in _source.OfType<IDisposable>().ToArray())
            {
                item.Dispose();
            }
        }
    }

    /// <inheritdoc />
    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
    {
        if (!_isSuppressingEvents)
        {
            base.OnCollectionChanged(args);
        }
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(PropertyChangedEventArgs args)
    {
        if (!_isSuppressingEvents)
        {
            base.OnPropertyChanged(args);
        }
    }

    private void ApplyCollectionChanged(CollectionChanged<T> collectionChanged)
    {
        if (_raiseEventContext is null)
        {
            ApplyCollectionChangedCore(collectionChanged);
        }
        else
        {
            _raiseEventContext.Post(static state =>
            {
                var args = ((ReadOnlyReactiveCollection<T> Collection, CollectionChanged<T> Changed))state!;
                args.Collection.ApplyCollectionChangedCore(args.Changed);
            }, (this, collectionChanged));
        }
    }

    private void ApplyCollectionChangedCore(CollectionChanged<T> collectionChanged)
    {
        NotifyCollectionChangedEventArgs eventArgs;
        lock (_syncRoot)
        {
            _isSuppressingEvents = true;
            try
            {
                eventArgs = collectionChanged.Action switch
                {
                    NotifyCollectionChangedAction.Add => ApplyAdd(collectionChanged),
                    NotifyCollectionChangedAction.Remove => ApplyRemove(collectionChanged),
                    NotifyCollectionChangedAction.Replace => ApplyReplace(collectionChanged),
                    NotifyCollectionChangedAction.Move => ApplyMove(collectionChanged),
                    NotifyCollectionChangedAction.Reset => ApplyReset(collectionChanged),
                    _ => throw new InvalidOperationException($"Unknown NotifyCollectionChangedAction value: {collectionChanged.Action}"),
                };
            }
            finally
            {
                _isSuppressingEvents = false;
            }
        }

        OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
        OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
        OnCollectionChanged(eventArgs);
    }

    private NotifyCollectionChangedEventArgs ApplyAdd(CollectionChanged<T> collectionChanged)
    {
        var index = collectionChanged.Index < 0 ? _source.Count : collectionChanged.Index;
        var values = collectionChanged.Values?.ToArray() ?? Array.Empty<T>();
        foreach (var item in values)
        {
            _source.Insert(index++, item);
        }

        return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, values, collectionChanged.Index < 0 ? _source.Count - values.Length : collectionChanged.Index);
    }

    private NotifyCollectionChangedEventArgs ApplyRemove(CollectionChanged<T> collectionChanged)
    {
        var index = collectionChanged.Index;
        var values = collectionChanged.Values?.Any() == true
            ? collectionChanged.Values.ToArray()
            : new[] { _source[index] };
        foreach (var _ in values)
        {
            InvokeDispose(_source[index]);
            _source.RemoveAt(index);
        }

        return values.Length == 1
            ? new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, values[0], index)
            : new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, values, index);
    }

    private NotifyCollectionChangedEventArgs ApplyReplace(CollectionChanged<T> collectionChanged)
    {
        var index = collectionChanged.Index;
        var value = collectionChanged.Value!;
        var oldValue = _source[index];
        InvokeDispose(oldValue);
        _source[index] = value;
        return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldValue, index);
    }

    private NotifyCollectionChangedEventArgs ApplyMove(CollectionChanged<T> collectionChanged)
    {
        var value = _source[collectionChanged.OldIndex];
        _source.Move(collectionChanged.OldIndex, collectionChanged.Index);
        return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, value, collectionChanged.Index, collectionChanged.OldIndex);
    }

    private NotifyCollectionChangedEventArgs ApplyReset(CollectionChanged<T> collectionChanged)
    {
        foreach (var item in _source.ToArray())
        {
            InvokeDispose(item);
        }

        _source.Clear();
        if (collectionChanged.Source is not null)
        {
            foreach (var item in collectionChanged.Source)
            {
                _source.Add(item);
            }
        }

        return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
    }

    private void InvokeDispose(object? item)
    {
        if (_disposeElement && item is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}

/// <summary>
/// Represents a collection change.
/// </summary>
/// <typeparam name="T">Element type.</typeparam>
public readonly struct CollectionChanged<T>
{
    private CollectionChanged(
        NotifyCollectionChangedAction action,
        int index,
        IReadOnlyList<T>? values,
        int oldIndex,
        IReadOnlyList<T>? source)
    {
        Action = action;
        Index = index;
        Values = values;
        OldIndex = oldIndex;
        Source = source;
    }

    /// <summary>
    /// Gets a reset change.
    /// </summary>
    public static CollectionChanged<T> Reset { get; } = new(NotifyCollectionChangedAction.Reset, -1, null, -1, null);

    /// <summary>
    /// Creates a reset change with a new source.
    /// </summary>
    public static CollectionChanged<T> ResetWithSource(IEnumerable<T>? source) =>
        new(NotifyCollectionChangedAction.Reset, -1, null, -1, source?.ToArray());

    /// <summary>
    /// Creates an add change.
    /// </summary>
    public static CollectionChanged<T> Add(int index, T value) =>
        Add(index, new[] { value });

    /// <summary>
    /// Creates an add change.
    /// </summary>
    public static CollectionChanged<T> Add(int index, IEnumerable<T> values) =>
        new(NotifyCollectionChangedAction.Add, index, values.ToArray(), -1, null);

    /// <summary>
    /// Creates a remove change.
    /// </summary>
    public static CollectionChanged<T> Remove(int index, T? value) =>
        value is null ? new(NotifyCollectionChangedAction.Remove, index, null, -1, null) : Remove(index, new[] { value });

    /// <summary>
    /// Creates a remove change.
    /// </summary>
    public static CollectionChanged<T> Remove(int index, IEnumerable<T> values) =>
        new(NotifyCollectionChangedAction.Remove, index, values.ToArray(), -1, null);

    /// <summary>
    /// Creates a replace change.
    /// </summary>
    public static CollectionChanged<T> Replace(int index, T value) =>
        new(NotifyCollectionChangedAction.Replace, index, new[] { value }, -1, null);

    /// <summary>
    /// Creates a move change.
    /// </summary>
    public static CollectionChanged<T> Move(int oldIndex, int newIndex, T? value) =>
        new(NotifyCollectionChangedAction.Move, newIndex, value is null ? null : new[] { value }, oldIndex, null);

    /// <summary>
    /// Gets the action.
    /// </summary>
    public NotifyCollectionChangedAction Action { get; }

    /// <summary>
    /// Gets the changed index.
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// Gets the changed value.
    /// </summary>
    public T? Value => Values is null || Values.Count == 0 ? default : Values[0];

    /// <summary>
    /// Gets the changed values.
    /// </summary>
    public IReadOnlyList<T>? Values { get; }

    /// <summary>
    /// Gets the old index for move changes.
    /// </summary>
    public int OldIndex { get; }

    /// <summary>
    /// Gets the reset source.
    /// </summary>
    public IReadOnlyList<T>? Source { get; }
}

/// <summary>
/// Factory methods for <see cref="ReadOnlyReactiveCollection{T}"/>.
/// </summary>
public static class ReadOnlyReactiveCollectionExtensions
{
    /// <summary>
    /// Converts a collection change stream to a read-only reactive collection.
    /// </summary>
    public static ReadOnlyReactiveCollection<T> ToReadOnlyReactiveCollection<T>(
        this Observable<CollectionChanged<T>> self,
        bool disposeElement = true,
        SynchronizationContext? raiseEventContext = null) =>
        new(self, new ObservableCollection<T>(), disposeElement, raiseEventContext);

    /// <summary>
    /// Converts an add stream to a read-only reactive collection.
    /// </summary>
    public static ReadOnlyReactiveCollection<T> ToReadOnlyReactiveCollection<T>(
        this Observable<T> self,
        Observable<Unit>? onReset,
        bool disposeElement = true,
        SynchronizationContext? raiseEventContext = null) =>
        new(self, new ObservableCollection<T>(), onReset, disposeElement, raiseEventContext);

    /// <summary>
    /// Converts collection changes to a change stream.
    /// </summary>
    public static Observable<CollectionChanged<T>> ToCollectionChanged<T>(this INotifyCollectionChanged self)
    {
        if (self is null)
        {
            throw new ArgumentNullException(nameof(self));
        }

        return Observable.Create<CollectionChanged<T>>(observer =>
        {
            void CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
            {
                var collectionChanged = e.Action switch
                {
                    NotifyCollectionChangedAction.Add => CollectionChanged<T>.Add(e.NewStartingIndex, e.NewItems!.Cast<T>()),
                    NotifyCollectionChangedAction.Remove => CollectionChanged<T>.Remove(e.OldStartingIndex, e.OldItems!.Cast<T>()),
                    NotifyCollectionChangedAction.Replace => CollectionChanged<T>.Replace(e.NewStartingIndex, e.NewItems!.Cast<T>().First()),
                    NotifyCollectionChangedAction.Move => CollectionChanged<T>.Move(e.OldStartingIndex, e.NewStartingIndex, e.NewItems!.Cast<T>().First()),
                    NotifyCollectionChangedAction.Reset => CollectionChanged<T>.ResetWithSource(sender as IEnumerable<T>),
                    _ => throw new InvalidOperationException($"Unknown NotifyCollectionChangedAction value: {e.Action}"),
                };
                observer.OnNext(collectionChanged);
            }

            self.CollectionChanged += CollectionChanged;
            return Disposable.Create(() => self.CollectionChanged -= CollectionChanged);
        });
    }

    /// <summary>
    /// Converts collection changes to a change stream.
    /// </summary>
    public static Observable<CollectionChanged<T>> ToCollectionChanged<T>(this ObservableCollection<T> self) =>
        ((INotifyCollectionChanged)self).ToCollectionChanged<T>();

    /// <summary>
    /// Converts collection changes to a change stream.
    /// </summary>
    public static Observable<CollectionChanged<T>> ToCollectionChanged<T>(this ReadOnlyObservableCollection<T> self) =>
        ((INotifyCollectionChanged)self).ToCollectionChanged<T>();

    /// <summary>
    /// Converts an observable collection to a read-only reactive collection.
    /// </summary>
    public static ReadOnlyReactiveCollection<T> ToReadOnlyReactiveCollection<T>(
        this ObservableCollection<T> self,
        bool disposeElement = true,
        SynchronizationContext? raiseEventContext = null) =>
        self.ToReadOnlyReactiveCollection(static x => x, disposeElement, raiseEventContext);

    /// <summary>
    /// Converts an observable collection to a read-only reactive collection.
    /// </summary>
    public static ReadOnlyReactiveCollection<TResult> ToReadOnlyReactiveCollection<TSource, TResult>(
        this ObservableCollection<TSource> self,
        Func<TSource, TResult> converter,
        bool disposeElement = true,
        SynchronizationContext? raiseEventContext = null) =>
        ((IEnumerable<TSource>)self).ToReadOnlyReactiveCollection(self.ToCollectionChanged(), converter, disposeElement, raiseEventContext);

    /// <summary>
    /// Converts a read-only observable collection to a read-only reactive collection.
    /// </summary>
    public static ReadOnlyReactiveCollection<T> ToReadOnlyReactiveCollection<T>(
        this ReadOnlyObservableCollection<T> self,
        bool disposeElement = true,
        SynchronizationContext? raiseEventContext = null) =>
        self.ToReadOnlyReactiveCollection(static x => x, disposeElement, raiseEventContext);

    /// <summary>
    /// Converts a read-only observable collection to a read-only reactive collection.
    /// </summary>
    public static ReadOnlyReactiveCollection<TResult> ToReadOnlyReactiveCollection<TSource, TResult>(
        this ReadOnlyObservableCollection<TSource> self,
        Func<TSource, TResult> converter,
        bool disposeElement = true,
        SynchronizationContext? raiseEventContext = null) =>
        ((IEnumerable<TSource>)self).ToReadOnlyReactiveCollection(self.ToCollectionChanged(), converter, disposeElement, raiseEventContext);

    private static ReadOnlyReactiveCollection<TResult> ToReadOnlyReactiveCollection<TSource, TResult>(
        this IEnumerable<TSource> self,
        Observable<CollectionChanged<TSource>> collectionChanged,
        Func<TSource, TResult> converter,
        bool disposeElement,
        SynchronizationContext? raiseEventContext)
    {
        var source = new ObservableCollection<TResult>(self.Select(converter));
        var convertedChanges = collectionChanged.Select(x => x.Action switch
        {
            NotifyCollectionChangedAction.Add => CollectionChanged<TResult>.Add(x.Index, x.Values!.Select(converter)),
            NotifyCollectionChangedAction.Remove => CollectionChanged<TResult>.Remove(x.Index, default(TResult)),
            NotifyCollectionChangedAction.Replace => CollectionChanged<TResult>.Replace(x.Index, converter(x.Value!)),
            NotifyCollectionChangedAction.Move => CollectionChanged<TResult>.Move(x.OldIndex, x.Index, default),
            NotifyCollectionChangedAction.Reset => CollectionChanged<TResult>.ResetWithSource(x.Source?.Select(converter)),
            _ => throw new InvalidOperationException($"Unknown NotifyCollectionChangedAction value: {x.Action}"),
        });
        return new ReadOnlyReactiveCollection<TResult>(convertedChanges, source, disposeElement, raiseEventContext);
    }
}
