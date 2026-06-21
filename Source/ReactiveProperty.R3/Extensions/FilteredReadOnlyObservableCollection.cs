using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using R3;
using Reactive.Bindings.R3;

namespace Reactive.Bindings.R3.Extensions;

/// <summary>
/// Real-time filtered read-only observable collection.
/// </summary>
/// <typeparam name="TElement">Element type.</typeparam>
public interface IFilteredReadOnlyObservableCollection<out TElement> : IReadOnlyList<TElement>, INotifyCollectionChanged, INotifyPropertyChanged, IDisposable
    where TElement : class, INotifyPropertyChanged
{
    /// <summary>
    /// Refreshes the filter.
    /// </summary>
    /// <param name="filter">New filter.</param>
    void Refresh(Func<TElement, bool> filter);
}

internal sealed class FilteredReadOnlyObservableCollection<TCollection, TElement, TTrigger> : IFilteredReadOnlyObservableCollection<TElement>
    where TCollection : INotifyCollectionChanged, IList<TElement>
    where TElement : class, INotifyPropertyChanged
{
    private readonly object _syncRoot = new();
    private readonly List<TElement> _innerCollection = new();
    private readonly List<int?> _indexList = new();
    private readonly List<IDisposable> _subscriptions = new();
    private readonly Func<TElement, Observable<TTrigger>> _elementChangedFactory;
    private Func<TElement, bool> _filter;
    private bool _isDisposed;

    public FilteredReadOnlyObservableCollection(TCollection source, Func<TElement, bool> filter, Func<TElement, Observable<TTrigger>> elementChangedFactory)
    {
        Source = source ?? throw new ArgumentNullException(nameof(source));
        _filter = filter ?? throw new ArgumentNullException(nameof(filter));
        _elementChangedFactory = elementChangedFactory ?? throw new ArgumentNullException(nameof(elementChangedFactory));

        lock (_syncRoot)
        {
            Initialize();
        }

        Source.CollectionChanged += SourceCollectionChanged;
        _subscriptions.Add(Source.ObserveElementCore<TElement, TElement>(SubscribeElementChanged).Subscribe(SourceElementChanged));
    }

    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    public event PropertyChangedEventHandler? PropertyChanged;

    private TCollection Source { get; }

    public int Count
    {
        get
        {
            lock (_syncRoot)
            {
                return _innerCollection.Count;
            }
        }
    }

    public TElement this[int index]
    {
        get
        {
            lock (_syncRoot)
            {
                return _innerCollection[index];
            }
        }
    }

    public IEnumerator<TElement> GetEnumerator()
    {
        lock (_syncRoot)
        {
            return _innerCollection.ToArray().AsEnumerable().GetEnumerator();
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Refresh(Func<TElement, bool> filter)
    {
        lock (_syncRoot)
        {
            _filter = filter ?? throw new ArgumentNullException(nameof(filter));
            Initialize();
        }

        RaiseReset();
    }

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;
        Source.CollectionChanged -= SourceCollectionChanged;
        foreach (var subscription in _subscriptions.ToArray())
        {
            subscription.Dispose();
        }

        _subscriptions.Clear();
    }

    private IDisposable SubscribeElementChanged(TElement element, Observer<TElement> observer) =>
        _elementChangedFactory(element).Subscribe(_ => observer.OnNext(element));

    private void SourceElementChanged(TElement element)
    {
        NotifyCollectionChangedEventArgs? args = null;
        lock (_syncRoot)
        {
            var sourceIndex = Source.IndexOf(element);
            if (sourceIndex < 0)
            {
                return;
            }

            var filteredIndex = _indexList[sourceIndex];
            var isTarget = _filter(element);
            if (isTarget && filteredIndex is null)
            {
                AppearNewItem(sourceIndex);
                args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, element, _indexList[sourceIndex]!.Value);
            }
            else if (!isTarget && filteredIndex.HasValue)
            {
                DisappearItem(sourceIndex);
                _indexList[sourceIndex] = null;
                args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, element, filteredIndex.Value);
            }
        }

        RaiseChanged(args);
    }

    private void SourceCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        NotifyCollectionChangedEventArgs? args = null;
        lock (_syncRoot)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    _indexList.InsertRange(e.NewStartingIndex, Enumerable.Repeat<int?>(null, e.NewItems!.Count));
                    foreach (var itemAndIndex in e.NewItems.Cast<TElement>().Select((Item, Offset) => (Item, Index: e.NewStartingIndex + Offset)))
                    {
                        if (_filter(itemAndIndex.Item))
                        {
                            AppearNewItem(itemAndIndex.Index);
                            args ??= new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, itemAndIndex.Item, _indexList[itemAndIndex.Index]!.Value);
                        }
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    for (var i = 0; i < e.OldItems!.Count; i++)
                    {
                        var sourceIndex = e.OldStartingIndex;
                        var filteredIndex = _indexList[sourceIndex];
                        var item = e.OldItems.Cast<TElement>().ElementAt(i);
                        if (filteredIndex.HasValue)
                        {
                            DisappearItem(sourceIndex);
                            args ??= new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, filteredIndex.Value);
                        }

                        _indexList.RemoveAt(sourceIndex);
                    }

                    break;
                case NotifyCollectionChangedAction.Replace:
                    var index = e.NewStartingIndex;
                    var oldItem = e.OldItems!.Cast<TElement>().Single();
                    var newItem = e.NewItems!.Cast<TElement>().Single();
                    var currentFilteredIndex = _indexList[index];
                    var isTarget = _filter(newItem);
                    if (currentFilteredIndex is null && isTarget)
                    {
                        AppearNewItem(index);
                        args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItem, _indexList[index]!.Value);
                    }
                    else if (currentFilteredIndex.HasValue && isTarget)
                    {
                        _innerCollection[currentFilteredIndex.Value] = newItem;
                        args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem, currentFilteredIndex.Value);
                    }
                    else if (currentFilteredIndex.HasValue)
                    {
                        DisappearItem(index);
                        _indexList[index] = null;
                        args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItem, currentFilteredIndex.Value);
                    }

                    break;
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Reset:
                    Initialize();
                    args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                    break;
                default:
                    throw new InvalidOperationException($"Unknown NotifyCollectionChangedAction value: {e.Action}");
            }
        }

        RaiseChanged(args);
    }

    private void Initialize()
    {
        _indexList.Clear();
        _innerCollection.Clear();

        foreach (var item in Source)
        {
            var isTarget = _filter(item);
            _indexList.Add(isTarget ? _innerCollection.Count : null);
            if (isTarget)
            {
                _innerCollection.Add(item);
            }
        }
    }

    private void AppearNewItem(int sourceIndex)
    {
        var nearIndex = FindNearIndex(sourceIndex);
        _indexList[sourceIndex] = nearIndex + 1;
        for (var i = sourceIndex + 1; i < _indexList.Count; i++)
        {
            if (_indexList[i].HasValue)
            {
                _indexList[i]++;
            }
        }

        _innerCollection.Insert(_indexList[sourceIndex]!.Value, Source[sourceIndex]);
    }

    private void DisappearItem(int sourceIndex)
    {
        _innerCollection.RemoveAt(_indexList[sourceIndex]!.Value);
        for (var i = sourceIndex; i < _indexList.Count; i++)
        {
            if (_indexList[i].HasValue)
            {
                _indexList[i]--;
            }
        }
    }

    private int FindNearIndex(int position)
    {
        for (var i = position; i >= 0; i--)
        {
            if (_indexList[i].HasValue)
            {
                return _indexList[i]!.Value;
            }
        }

        return -1;
    }

    private void RaiseReset() =>
        RaiseChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

    private void RaiseChanged(NotifyCollectionChangedEventArgs? args)
    {
        if (args is null)
        {
            return;
        }

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
        CollectionChanged?.Invoke(this, args);
    }
}

/// <summary>
/// Factory methods for filtered read-only observable collections.
/// </summary>
public static class FilteredReadOnlyObservableCollectionExtensions
{
    /// <summary>
    /// Creates a filtered read-only observable collection.
    /// </summary>
    public static IFilteredReadOnlyObservableCollection<TElement> ToFilteredReadOnlyObservableCollection<TCollection, TElement>(
        this TCollection source,
        Func<TElement, bool> filter)
        where TCollection : INotifyCollectionChanged, IList<TElement>
        where TElement : class, INotifyPropertyChanged =>
        new FilteredReadOnlyObservableCollection<TCollection, TElement, PropertyChangedEventArgs>(
            source,
            filter,
            static x => Observable.FromEvent<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                h => (_, e) => h(e),
                h => x.PropertyChanged += h,
                h => x.PropertyChanged -= h,
                default));

    /// <summary>
    /// Creates a filtered read-only observable collection.
    /// </summary>
    public static IFilteredReadOnlyObservableCollection<TElement> ToFilteredReadOnlyObservableCollection<TElement>(
        this ObservableCollection<TElement> source,
        Func<TElement, bool> filter)
        where TElement : class, INotifyPropertyChanged =>
        source.ToFilteredReadOnlyObservableCollection<ObservableCollection<TElement>, TElement>(filter);

    /// <summary>
    /// Creates a filtered read-only observable collection.
    /// </summary>
    public static IFilteredReadOnlyObservableCollection<TElement> ToFilteredReadOnlyObservableCollection<TElement>(
        this ReadOnlyObservableCollection<TElement> source,
        Func<TElement, bool> filter)
        where TElement : class, INotifyPropertyChanged =>
        source.ToFilteredReadOnlyObservableCollection<ReadOnlyObservableCollection<TElement>, TElement>(filter);

    /// <summary>
    /// Creates a filtered read-only observable collection.
    /// </summary>
    public static IFilteredReadOnlyObservableCollection<TElement> ToFilteredReadOnlyObservableCollection<TCollection, TElement, TTrigger>(
        this TCollection source,
        Func<TElement, bool> filter,
        Func<TElement, Observable<TTrigger>> elementStatusChangedFactory)
        where TCollection : INotifyCollectionChanged, IList<TElement>
        where TElement : class, INotifyPropertyChanged =>
        new FilteredReadOnlyObservableCollection<TCollection, TElement, TTrigger>(source, filter, elementStatusChangedFactory);
}
