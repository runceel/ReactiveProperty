using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using Reactive.Bindings.Extensions;

namespace Reactive.Bindings.Helpers
{
    /// <summary>
    /// real time filtered collection interface.
    /// </summary>
    /// <typeparam name="T">Type of collection item.</typeparam>
    public interface IFilteredReadOnlyObservableCollection<out T> : INotifyCollectionChanged, IDisposable, IEnumerable<T>
        where T : class, INotifyPropertyChanged
    {
        /// <summary>
        /// Collection items count.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Get item at index.
        /// </summary>
        /// <param name="index">index</param>
        /// <returns>item</returns>
        T this[int index] { get; }

        /// <summary>
        /// Refresh filter.
        /// </summary>
        /// <param name="filter">filter</param>
        void Refresh(Func<T, bool> filter);
    }

    /// <summary>
    /// real time filtered collection.
    /// </summary>
    /// <typeparam name="TCollection">type of source collection</typeparam>
    /// <typeparam name="TElement">type of collection item</typeparam>
    public sealed class FilteredReadOnlyObservableCollection<TCollection, TElement> : IFilteredReadOnlyObservableCollection<TElement>, IList // IList UWP GridView support.
        where TCollection : INotifyCollectionChanged, IList<TElement>
        where TElement : class, INotifyPropertyChanged
    {
        private TCollection Source { get; }

        private Func<TElement, bool> Filter { get; set; }

        private List<int?> IndexList { get; } = new List<int?>();

        private CompositeDisposable Subscription { get; } = new CompositeDisposable();

        private int ItemsCount { get; set; }

        private List<TElement> InnerCollection { get; } = new List<TElement>();

        /// <summary>
        /// CollectionChanged event.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="source">source collection</param>
        /// <param name="filter">filter function</param>
        public FilteredReadOnlyObservableCollection(TCollection source, Func<TElement, bool> filter)
        {
            Source = source;
            Filter = filter;

            Initialize();

            {
                // propertychanged
                source.ObserveElementPropertyChanged<TCollection, TElement>()
                    .Subscribe(x => {
                        var index = source.IndexOf(x.Sender);
                        var filteredIndex = IndexList[index];
                        var isTarget = Filter(x.Sender);
                        if (isTarget && filteredIndex == null) {
                            // add
                            AppearNewItem(index);
                        } else if (!isTarget && filteredIndex.HasValue) {
                            // remove
                            DisappearItem(index);
                            IndexList[index] = null;
                            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, x.Sender, filteredIndex.Value));
                        }
                    })
                    .AddTo(Subscription);
            }
            {
                // collection changed(support single changed only)
                source.CollectionChangedAsObservable()
                    .Subscribe(x => {
                        switch (x.Action) {
                            case NotifyCollectionChangedAction.Add:
                                // appear
                                IndexList.Insert(x.NewStartingIndex, null);
                                if (Filter(x.NewItems.Cast<TElement>().Single())) {
                                    AppearNewItem(x.NewStartingIndex);
                                }
                                break;

                            case NotifyCollectionChangedAction.Move:
                                throw new NotSupportedException("Move is not supported");
                            case NotifyCollectionChangedAction.Remove:
                                var removedIndex = IndexList[x.OldStartingIndex];
                                if (removedIndex.HasValue) {
                                    DisappearItem(x.OldStartingIndex);
                                    IndexList.RemoveAt(x.OldStartingIndex);
                                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
                                        x.OldItems.Cast<TElement>().Single(), removedIndex.Value));
                                } else {
                                    IndexList.RemoveAt(x.OldStartingIndex);
                                }
                                break;

                            case NotifyCollectionChangedAction.Replace:
                                var index = IndexList[x.NewStartingIndex];
                                var isTarget = Filter(x.NewItems.Cast<TElement>().Single());
                                if (index == null && isTarget) {
                                    // add
                                    AppearNewItem(x.NewStartingIndex);
                                } else if (index.HasValue && isTarget) {
                                    // replace
                                    InnerCollection[index.Value] = x.NewItems.Cast<TElement>().Single();
                                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
                                        x.NewItems.Cast<TElement>().Single(), x.OldItems.Cast<TElement>().Single(), index.Value));
                                } else if (index.HasValue && !isTarget) {
                                    // remove
                                    DisappearItem(x.NewStartingIndex);
                                    IndexList[x.NewStartingIndex] = null;
                                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
                                        x.OldItems.Cast<TElement>().Single(), index.Value));
                                }

                                break;

                            case NotifyCollectionChangedAction.Reset:
                                IndexList.Clear();
                                InnerCollection.Clear();
                                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                                if (source.Any()) {
                                    throw new NotSupportedException("Reset is clear only");
                                }
                                break;

                            default:
                                throw new InvalidOperationException();
                        }
                    })
                    .AddTo(Subscription);
            }
        }

        private void Initialize()
        {
            IndexList.Clear();
            ItemsCount = 0;
            InnerCollection.Clear();

            foreach (var item in Source) {
                var isTarget = Filter(item);
                IndexList.Add(isTarget ? (int?)ItemsCount : null);
                if (isTarget) {
                    ItemsCount++;
                    InnerCollection.Add(item);
                }
            }
        }

        /// <summary>
        /// Count
        /// </summary>
        public int Count => InnerCollection.Count;

        bool IList.IsFixedSize => false;

        bool IList.IsReadOnly => true;

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => null;

        /// <summary>
        /// Gets the Element at the specified index.
        /// </summary>
        /// <value>The Element/&gt;.</value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public TElement this[int index] => InnerCollection[index];

        object IList.this[int index]
        {
            get
            {
                return InnerCollection[index];
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// get enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<TElement> GetEnumerator() => InnerCollection.GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action) {
                case NotifyCollectionChangedAction.Add:
                    ItemsCount++;
                    break;

                case NotifyCollectionChangedAction.Remove:
                    ItemsCount--;
                    break;

                case NotifyCollectionChangedAction.Reset:
                    ItemsCount = 0;
                    break;
            }
            CollectionChanged?.Invoke(this, e);
        }

        /// <summary>
        /// disconnect source collection.
        /// </summary>
        public void Dispose()
        {
            if (Subscription.IsDisposed) { return; }
            Subscription.Dispose();
        }

        private int FindNearIndex(int position) => IndexList.Take(position).Reverse().FirstOrDefault(x => x != null) ?? -1;

        private void AppearNewItem(int index)
        {
            var nearIndex = FindNearIndex(index);
            IndexList[index] = nearIndex + 1;
            for (var i = index + 1; i < IndexList.Count; i++) {
                if (IndexList[i].HasValue) { IndexList[i]++; }
            }
            InnerCollection.Insert(IndexList[index].Value, this.Source[index]);
            OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
                    this.Source[index], IndexList[index].Value));
        }

        private void DisappearItem(int index)
        {
            InnerCollection.RemoveAt(IndexList[index].Value);
            for (var i = index; i < IndexList.Count; i++) {
                if (IndexList[i].HasValue) { IndexList[i]--; }
            }
        }

        int IList.Add(object value)
        {
            throw new NotSupportedException();
        }

        void IList.Clear()
        {
            throw new NotSupportedException();
        }

        bool IList.Contains(object value) => InnerCollection.Contains(value);

        int IList.IndexOf(object value) => InnerCollection.IndexOf((TElement)value);

        void IList.Insert(int index, object value)
        {
            throw new NotSupportedException();
        }

        void IList.Remove(object value)
        {
            throw new NotSupportedException();
        }

        void IList.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        void ICollection.CopyTo(Array array, int index) => InnerCollection.CopyTo((TElement[])array, index);

        /// <summary>
        /// Refresh filter.
        /// </summary>
        /// <param name="filter">filter</param>
        public void Refresh(Func<TElement, bool> filter)
        {
            Filter = filter;
            Initialize();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }

    /// <summary>
    /// FilteredReadOnlyObservableCollection factory methods.
    /// </summary>
    public static class FilteredReadOnlyObservableCollection
    {
        /// <summary>
        /// create IFilteredReadOnlyObservableCollection from ObservableCollection
        /// </summary>
        /// <typeparam name="T">Type of collection item.</typeparam>
        /// <param name="self">Source collection.</param>
        /// <param name="filter">Filter function.</param>
        /// <returns></returns>
        public static IFilteredReadOnlyObservableCollection<T> ToFilteredReadOnlyObservableCollection<T>(this ObservableCollection<T> self, Func<T, bool> filter)
            where T : class, INotifyPropertyChanged =>
            new FilteredReadOnlyObservableCollection<ObservableCollection<T>, T>(self, filter);

        /// <summary>
        /// create IFilteredReadOnlyObservableCollection from ReadOnlyObservableCollection
        /// </summary>
        /// <typeparam name="T">Type of collection item.</typeparam>
        /// <param name="self">Source collection.</param>
        /// <param name="filter">Filter function.</param>
        /// <returns></returns>
        public static IFilteredReadOnlyObservableCollection<T> ToFilteredReadOnlyObservableCollection<T>(this ReadOnlyObservableCollection<T> self, Func<T, bool> filter)
            where T : class, INotifyPropertyChanged =>
            new FilteredReadOnlyObservableCollection<ReadOnlyObservableCollection<T>, T>(self, filter);

        /// <summary>
        /// create ReadOnlyReactiveCollection from IFilteredReadOnlyObservableCollection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self">The self.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="disposeElement">if set to <c>true</c> [dispose element].</param>
        /// <returns></returns>
        public static ReadOnlyReactiveCollection<T> ToReadOnlyReactiveCollection<T>(this IFilteredReadOnlyObservableCollection<T> self, IScheduler scheduler = null, bool disposeElement = true)
            where T : class, INotifyPropertyChanged =>
            self.ToReadOnlyReactiveCollection(x => x, scheduler, disposeElement);

        /// <summary>
        /// create ReadOnlyReactiveCollection from IFilteredReadOnlyObservableCollection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="self">The self.</param>
        /// <param name="converter">The converter.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="disposeElement">if set to <c>true</c> [dispose element].</param>
        /// <returns></returns>
        public static ReadOnlyReactiveCollection<U> ToReadOnlyReactiveCollection<T, U>(this IFilteredReadOnlyObservableCollection<T> self, Func<T, U> converter, IScheduler scheduler = null, bool disposeElement = true)
            where T : class, INotifyPropertyChanged =>
            self.ToReadOnlyReactiveCollection(
                self.ToCollectionChanged<T>(),
                converter,
                scheduler,
                disposeElement);
    }
}
