using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ObservableCollections;
using R3;
using Reactive.Bindings.R3Compat;

namespace Reactive.Bindings.R3Compat.Collections;

/// <summary>
/// Minimal ReadOnlyReactiveCollection compatibility wrapper backed by R3 collection change streams.
/// </summary>
/// <typeparam name="T">The collection item type.</typeparam>
/// <remarks>
/// Downgrade when disposeElement is false and Reset semantics are unused: use ObservableCollections.R3 directly.
/// Remove when automatic Dispose and Reset-diff dependencies are zero.
/// </remarks>
public sealed class ReadOnlyReactiveCollectionCompat<T> : ReadOnlyObservableCollection<T>, IDisposable
{
    private readonly ObservableCollection<T> _source;
    private readonly IDisposable _subscription;
    private readonly bool _disposeElement;
    private bool _isDisposed;

    private ReadOnlyReactiveCollectionCompat(
        Observable<CollectionChangedEvent<T>> source,
        bool disposeElement)
        : base(new ObservableCollection<T>())
    {
        _source = (ObservableCollection<T>)Items;
        _disposeElement = disposeElement;
        _subscription = source.Subscribe(Apply);
        CompatibilityTelemetry.Track("RP-COL-001", typeof(ReadOnlyReactiveCollectionCompat<T>).FullName ?? nameof(ReadOnlyReactiveCollectionCompat<T>));
    }

    /// <summary>Creates a compatibility collection from an R3 ObservableCollections change stream.</summary>
    /// <typeparam name="TSource">The source item type.</typeparam>
    /// <param name="source">The source collection change stream.</param>
    /// <param name="converter">The projection from source item to destination item.</param>
    /// <param name="disposeElement">Whether removed/reset/disposed elements should be disposed.</param>
    public static ReadOnlyReactiveCollectionCompat<T> Create<TSource>(
        Observable<CollectionChangedEvent<TSource>> source,
        Func<TSource, T> converter,
        bool disposeElement = true)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (converter == null)
        {
            throw new ArgumentNullException(nameof(converter));
        }

        return new ReadOnlyReactiveCollectionCompat<T>(source.Select(x => Convert(x, converter)), disposeElement);
    }

    /// <inheritdoc />
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
            foreach (var item in _source)
            {
                DisposeElement(item);
            }
        }
    }

    private static CollectionChangedEvent<T> Convert<TSource>(CollectionChangedEvent<TSource> item, Func<TSource, T> converter) =>
        new(
            item.Action,
            ConvertItem(item.NewItem, converter),
            ConvertItem(item.OldItem, converter),
            item.NewStartingIndex,
            item.OldStartingIndex,
            default);

    private static T ConvertItem<TSource>(TSource value, Func<TSource, T> converter) => value == null ? default! : converter(value);

    private void Apply(CollectionChangedEvent<T> change)
    {
        if (_isDisposed)
        {
            return;
        }

        switch (change.Action)
        {
            case NotifyCollectionChangedAction.Add:
                _source.Insert(NormalizeAddIndex(change.NewStartingIndex), change.NewItem);
                break;
            case NotifyCollectionChangedAction.Remove:
                RemoveAt(NormalizeExistingIndex(change.OldStartingIndex));
                break;
            case NotifyCollectionChangedAction.Replace:
                ReplaceAt(NormalizeExistingIndex(change.NewStartingIndex), change.NewItem);
                break;
            case NotifyCollectionChangedAction.Move:
                _source.Move(NormalizeExistingIndex(change.OldStartingIndex), NormalizeExistingIndex(change.NewStartingIndex));
                break;
            case NotifyCollectionChangedAction.Reset:
                Reset();
                break;
        }
    }

    private int NormalizeAddIndex(int index) => index < 0 || index > _source.Count ? _source.Count : index;

    private int NormalizeExistingIndex(int index) => index < 0 ? 0 : index;

    private void RemoveAt(int index)
    {
        if (index >= _source.Count)
        {
            return;
        }

        var oldItem = _source[index];
        _source.RemoveAt(index);
        DisposeElement(oldItem);
    }

    private void ReplaceAt(int index, T newItem)
    {
        if (index >= _source.Count)
        {
            return;
        }

        var oldItem = _source[index];
        _source[index] = newItem;
        DisposeElement(oldItem);
    }

    private void Reset()
    {
        if (_disposeElement)
        {
            foreach (var item in _source)
            {
                DisposeElement(item);
            }
        }

        _source.Clear();
    }

    private void DisposeElement(T item)
    {
        if (_disposeElement && item is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}
