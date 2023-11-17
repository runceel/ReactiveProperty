using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using global::System.Collections.ObjectModel;

namespace ReactiveProperty.Tests.TestUtilities;

// Original: https://github.com/mattleibow/RangedObservableCollection/

/// <summary>
/// Implementation of a dynamic data collection based on generic Collection&lt;T&gt;,
/// implementing INotifyCollectionChanged to notify listeners
/// when items get added, removed or the whole list is refreshed.
/// </summary>
internal class RangedObservableCollection<T> : ObservableCollection<T>
{
    private const string CountName = "Count";
    private const string IndexerName = "Item[]";

    /// <summary>
    /// Initializes a new instance of RangedObservableCollection that is empty and has default initial capacity.
    /// </summary>
    public RangedObservableCollection()
        : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the RangedObservableCollection class that contains
    /// elements copied from the specified collection and has sufficient capacity
    /// to accommodate the number of elements copied.
    /// </summary>
    /// <param name="collection">The collection whose elements are copied to the new list.</param>
    /// <remarks>
    /// The elements are copied onto the RangedObservableCollection in the
    /// same order they are read by the enumerator of the collection.
    /// </remarks>
    /// <exception cref="ArgumentNullException"> collection is a null reference </exception>
    public RangedObservableCollection(IEnumerable<T> collection)
        : base(collection)
    {
    }

    /// <summary>
    /// Adds the elements of the specified collection to the end of the RangedObservableCollection.
    /// </summary>
    /// <param name="collection">The collection whose elements are added to the list.</param>
    /// <remarks>
    /// The elements are copied onto the RangedObservableCollection in the
    /// same order they are read by the enumerator of the collection.
    /// </remarks>
    /// <exception cref="ArgumentNullException"> collection is a null reference </exception>
    public void AddRange(IEnumerable<T> collection)
    {
        if (collection == null)
            throw new ArgumentNullException(nameof(collection));

        CheckReentrancy();

        var startIndex = Count;
        var changedItems = new List<T>(collection);

        foreach (var i in changedItems)
            Items.Add(i);

        OnCountPropertyChanged();
        OnIndexerPropertyChanged();
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, changedItems, startIndex));
    }

    /// <summary>
    /// Clears the current RangedObservableCollection and replaces the elements with the elements of specified collection.
    /// </summary>
    /// <param name="collection">The collection whose elements are added to the list.</param>
    /// <remarks>
    /// The elements are copied onto the RangedObservableCollection in the
    /// same order they are read by the enumerator of the collection.
    /// </remarks>
    /// <exception cref="ArgumentNullException"> collection is a null reference </exception>
    public void ReplaceRange(IEnumerable<T> collection)
    {
        if (collection == null)
            throw new ArgumentNullException(nameof(collection));

        CheckReentrancy();

        Items.Clear();
        foreach (var i in collection)
            Items.Add(i);

        OnCountPropertyChanged();
        OnIndexerPropertyChanged();
        OnCollectionReset();
    }

    /// <summary>
    /// Removes the first occurence of each item in the specified collection.
    /// </summary>
    /// <param name="collection">The collection whose elements are removed from list.</param>
    /// <remarks>
    /// The elements are copied onto the RangedObservableCollection in the
    /// same order they are read by the enumerator of the collection.
    /// </remarks>
    /// <exception cref="ArgumentNullException"> collection is a null reference </exception>
    public void RemoveRange(IEnumerable<T> collection)
    {
        if (collection == null)
            throw new ArgumentNullException(nameof(collection));

        CheckReentrancy();

        var changedItems = new List<T>(collection);
        for (int i = 0; i < changedItems.Count; i++)
        {
            if (!Items.Remove(changedItems[i]))
            {
                changedItems.RemoveAt(i);
                i--;
            }
        }

        if (changedItems.Count > 0)
        {
            OnCountPropertyChanged();
            OnIndexerPropertyChanged();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, changedItems));
        }
    }

    /// <summary>
    /// Clears the current RangedObservableCollection and replaces the elements with the specified element.
    /// </summary>
    /// <param name="item">The element which is added to the list.</param>
    public void Replace(T item)
    {
        CheckReentrancy();

        Items.Clear();
        Items.Add(item);

        OnCountPropertyChanged();
        OnIndexerPropertyChanged();
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    private void OnCountPropertyChanged()
    {
        OnPropertyChanged(EventArgsCache.CountPropertyChanged);
    }

    private void OnIndexerPropertyChanged()
    {
        OnPropertyChanged(EventArgsCache.IndexerPropertyChanged);
    }

    private void OnCollectionReset()
    {
        OnCollectionChanged(EventArgsCache.ResetCollectionChanged);
    }

    private static class EventArgsCache
    {
        internal static readonly PropertyChangedEventArgs CountPropertyChanged = new PropertyChangedEventArgs(CountName);
        internal static readonly PropertyChangedEventArgs IndexerPropertyChanged = new PropertyChangedEventArgs(IndexerName);
        internal static readonly NotifyCollectionChangedEventArgs ResetCollectionChanged = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
    }
}

