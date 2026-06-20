using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using R3;
using Reactive.Bindings.R3Compat;

namespace Reactive.Bindings.R3Compat.Collections;

public sealed class ReadOnlyReactiveCollectionCompat<T> : ReadOnlyObservableCollection<T>, IDisposable
{
    private readonly ObservableCollection<T> _source;
    private readonly IDisposable _subscription;
    private readonly bool _disposeElement;
    private bool _isDisposed;

    private ReadOnlyReactiveCollectionCompat(ObservableCollection<T> source, IDisposable subscription, bool disposeElement)
        : base(source)
    {
        _source = source;
        _subscription = subscription;
        _disposeElement = disposeElement;
        CompatibilityTelemetry.Track("R3Compat.Collections.ReadOnlyReactiveCollectionCompat", string.Empty);
    }

    public static ReadOnlyReactiveCollectionCompat<T> Create<TSource>(
        Observable<CollectionChanged<TSource>> source,
        Func<TSource, T> converter,
        bool disposeElement = true)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(converter);

        var collection = new ObservableCollection<T>();
        IDisposable? subscription = null;
        var result = new ReadOnlyReactiveCollectionCompat<T>(
            collection,
            new DeferredDisposable(() => subscription),
            disposeElement);
        subscription = source.Subscribe(result.Apply(converter));
        return result;
    }

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;
        _subscription.Dispose();
        if (_disposeElement)
        {
            foreach (var item in _source.OfType<IDisposable>().ToArray())
            {
                item.Dispose();
            }
        }
    }

    private Action<CollectionChanged<TSource>> Apply<TSource>(Func<TSource, T> converter) => change =>
    {
        if (_isDisposed)
        {
            return;
        }

        switch (change.Action)
        {
            case NotifyCollectionChangedAction.Add:
                var addIndex = NormalizeAddIndex(change.Index);
                foreach (var item in change.Values.Select(converter))
                {
                    _source.Insert(addIndex++, item);
                }

                break;
            case NotifyCollectionChangedAction.Remove:
                DisposeItem(_source[change.Index]);
                _source.RemoveAt(change.Index);
                break;
            case NotifyCollectionChangedAction.Replace:
                DisposeItem(_source[change.Index]);
                _source[change.Index] = converter(change.Value!);
                break;
            case NotifyCollectionChangedAction.Move:
                _source.Move(change.OldIndex, change.Index);
                break;
            case NotifyCollectionChangedAction.Reset:
                foreach (var item in _source.ToArray())
                {
                    DisposeItem(item);
                }

                _source.Clear();
                if (change.Source != null)
                {
                    foreach (var item in change.Source.Select(converter))
                    {
                        _source.Add(item);
                    }
                }

                break;
        }
    };

    private int NormalizeAddIndex(int index) => index < 0 || index > _source.Count ? _source.Count : index;

    private void DisposeItem(T item)
    {
        if (_disposeElement && item is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    private sealed class DeferredDisposable(Func<IDisposable?> getDisposable) : IDisposable
    {
        public void Dispose() => getDisposable()?.Dispose();
    }
}

public sealed class CollectionChanged<T>
{
    public static CollectionChanged<T> Reset { get; } = new() { Action = NotifyCollectionChangedAction.Reset };

    public static CollectionChanged<T> ResetWithSource(IEnumerable<T>? source) => new()
    {
        Action = NotifyCollectionChangedAction.Reset,
        Source = source?.ToArray(),
    };

    public static CollectionChanged<T> Add(int index, T value) => Add(index, new[] { value });

    public static CollectionChanged<T> Add(int index, IEnumerable<T> values) => new()
    {
        Action = NotifyCollectionChangedAction.Add,
        Index = index,
        Values = values.ToArray(),
    };

    public static CollectionChanged<T> Remove(int index, T value) => new()
    {
        Action = NotifyCollectionChangedAction.Remove,
        Index = index,
        Values = new[] { value },
    };

    public static CollectionChanged<T> Replace(int index, T value) => new()
    {
        Action = NotifyCollectionChangedAction.Replace,
        Index = index,
        Values = new[] { value },
    };

    public static CollectionChanged<T> Move(int oldIndex, int newIndex, T value) => new()
    {
        Action = NotifyCollectionChangedAction.Move,
        OldIndex = oldIndex,
        Index = newIndex,
        Values = new[] { value },
    };

    public IEnumerable<T>? Source { get; init; }

    public IEnumerable<T> Values { get; init; } = [];

    public T? Value => Values.FirstOrDefault();

    public int Index { get; init; }

    public int OldIndex { get; init; }

    public NotifyCollectionChangedAction Action { get; init; }
}
