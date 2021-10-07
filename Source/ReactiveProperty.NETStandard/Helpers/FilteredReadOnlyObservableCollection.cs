using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
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
    /// <typeparam name="TTrigger">type for IObservable to notify source status changed</typeparam>
    public sealed class FilteredReadOnlyObservableCollection<TCollection, TElement, TTrigger> : IFilteredReadOnlyObservableCollection<TElement>, IList // IList UWP GridView support.
        where TCollection : INotifyCollectionChanged, IList<TElement>
        where TElement : class, INotifyPropertyChanged
    {
        private readonly object _syncRoot = new object();
        private TCollection Source { get; }

        private Func<TElement, bool> Filter { get; set; }

        private List<int?> IndexList { get; } = new List<int?>();

        private CompositeDisposable Subscription { get; } = new CompositeDisposable();

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
        /// <param name="elementChangedFactory">IObservable to notify source status changed</param>
        public FilteredReadOnlyObservableCollection(TCollection source, Func<TElement, bool> filter, Func<TElement, IObservable<TTrigger>> elementChangedFactory)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Filter = filter ?? throw new ArgumentNullException(nameof(filter));
            if (elementChangedFactory == null)
            {
                throw new ArgumentNullException(nameof(elementChangedFactory));
            }

            lock (_syncRoot)
            {
                Initialize();
            }

            // propertychanged
            CollectionUtilities.ObserveElementCore<TCollection, TElement, TElement>(source, (x, observer) =>
                elementChangedFactory(x).Subscribe(_ => observer.OnNext(x)))
                .Subscribe(SourceElementChanged)
                .AddTo(Subscription);

            // collection changed(support single changed only)
            source.CollectionChanged += Source_CollectionChanged;
        }

        private void SourceElementChanged(TElement x)
        {
            NotifyCollectionChangedEventArgs args = default;
            lock (_syncRoot)
            {
                var index = Source.IndexOf(x);
                if (index == -1)
                {
                    throw new InvalidOperationException($"An object instance {x} did not found at the source collection.");
                }

                var filteredIndex = IndexList[index];
                var isTarget = Filter(x);
                if (isTarget && filteredIndex == null)
                {
                    // add
                    AppearNewItem(index);
                    args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
                        Source[index], IndexList[index].Value);

                }
                else if (!isTarget && filteredIndex.HasValue)
                {
                    // remove
                    DisappearItem(index);
                    IndexList[index] = null;
                    args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, x, filteredIndex.Value);
                }
            }

            if (args != null)
            {
                CollectionChanged?.Invoke(this, args);
            }
        }

        private void Source_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedEventArgs args = default;
            lock (_syncRoot)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        // appear
                        IndexList.Insert(e.NewStartingIndex, null);
                        if (Filter(e.NewItems.Cast<TElement>().Single()))
                        {
                            AppearNewItem(e.NewStartingIndex);
                            args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
                                Source[e.NewStartingIndex], IndexList[e.NewStartingIndex].Value);
                        }
                        break;

                    case NotifyCollectionChangedAction.Move:
                        throw new NotSupportedException("Move is not supported");
                    case NotifyCollectionChangedAction.Remove:
                        var removedIndex = IndexList[e.OldStartingIndex];
                        if (removedIndex.HasValue)
                        {
                            DisappearItem(e.OldStartingIndex);
                            IndexList.RemoveAt(e.OldStartingIndex);
                            args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
                                e.OldItems.Cast<TElement>().Single(), removedIndex.Value);
                        }
                        else
                        {
                            IndexList.RemoveAt(e.OldStartingIndex);
                        }
                        break;

                    case NotifyCollectionChangedAction.Replace:
                        var index = IndexList[e.NewStartingIndex];
                        var isTarget = Filter(e.NewItems.Cast<TElement>().Single());
                        if (index == null && isTarget)
                        {
                            // add
                            AppearNewItem(e.NewStartingIndex);
                            args = new NotifyCollectionChangedEventArgs(
                                NotifyCollectionChangedAction.Add,
                                Source[e.NewStartingIndex], IndexList[e.NewStartingIndex].Value);

                        }
                        else if (index.HasValue && isTarget)
                        {
                            // replace
                            InnerCollection[index.Value] = e.NewItems.Cast<TElement>().Single();
                            args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
                                e.NewItems.Cast<TElement>().Single(), e.OldItems.Cast<TElement>().Single(), index.Value);
                        }
                        else if (index.HasValue && !isTarget)
                        {
                            // remove
                            DisappearItem(e.NewStartingIndex);
                            IndexList[e.NewStartingIndex] = null;
                            args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
                                e.OldItems.Cast<TElement>().Single(), index.Value);
                        }

                        break;

                    case NotifyCollectionChangedAction.Reset:
                        Initialize();
                        args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                        break;

                    default:
                        throw new InvalidOperationException();
                }
            }

            if (args != null)
            {
                CollectionChanged?.Invoke(this, args);
            }
        }

        private void Initialize()
        {
            IndexList.Clear();
            InnerCollection.Clear();

            foreach (var item in Source)
            {
                var isTarget = Filter(item);
                IndexList.Add(isTarget ? (int?)InnerCollection.Count : null);
                if (isTarget)
                {
                    InnerCollection.Add(item);
                }
            }
        }

        /// <summary>
        /// Count
        /// </summary>
        public int Count
        {
            get
            {
                lock (_syncRoot)
                {
                    return InnerCollection.Count;
                }
            }
        }

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
        public TElement this[int index]
        {
            get
            {
                lock (_syncRoot)
                {
                    return InnerCollection[index];
                }
            }
        }

        object IList.this[int index]
        {
            get
            {
                lock (_syncRoot)
                {
                    return InnerCollection[index];
                }
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
        public IEnumerator<TElement> GetEnumerator()
        {
            lock (_syncRoot)
            {
                return InnerCollection.GetEnumerator();
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// disconnect source collection.
        /// </summary>
        public void Dispose()
        {
            if (Subscription.IsDisposed) { return; }
            Subscription.Dispose();
            CollectionChanged -= Source_CollectionChanged;
        }

        private int FindNearIndex(int position) => IndexList.Take(position).Reverse().FirstOrDefault(x => x != null) ?? -1;

        private void AppearNewItem(int index)
        {
            var nearIndex = FindNearIndex(index);
            IndexList[index] = nearIndex + 1;
            for (var i = index + 1; i < IndexList.Count; i++)
            {
                if (IndexList[i].HasValue) { IndexList[i]++; }
            }

            InnerCollection.Insert(IndexList[index].Value, Source[index]);
        }

        private void DisappearItem(int index)
        {
            InnerCollection.RemoveAt(IndexList[index].Value);
            for (var i = index; i < IndexList.Count; i++)
            {
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

        bool IList.Contains(object value)
        {
            lock (_syncRoot)
            {
                return InnerCollection.Contains(value);
            }
        }

        int IList.IndexOf(object value)
        {
            lock (_syncRoot)
            {
                return InnerCollection.IndexOf((TElement)value);
            }
        }

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

        void ICollection.CopyTo(Array array, int index)
        {
            lock (_syncRoot)
            {
                InnerCollection.CopyTo((TElement[])array, index);
            }
        }

        /// <summary>
        /// Refresh filter.
        /// </summary>
        /// <param name="filter">filter</param>
        public void Refresh(Func<TElement, bool> filter)
        {
            lock (_syncRoot)
            {
                Filter = filter;
                Initialize();
            }

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
            new FilteredReadOnlyObservableCollection<ObservableCollection<T>, T, PropertyChangedEventArgs>(
                self, 
                filter, 
                x => x.PropertyChangedAsObservable());

        /// <summary>
        /// create IFilteredReadOnlyObservableCollection from ReadOnlyObservableCollection
        /// </summary>
        /// <typeparam name="T">Type of collection item.</typeparam>
        /// <param name="self">Source collection.</param>
        /// <param name="filter">Filter function.</param>
        /// <returns></returns>
        public static IFilteredReadOnlyObservableCollection<T> ToFilteredReadOnlyObservableCollection<T>(this ReadOnlyObservableCollection<T> self, Func<T, bool> filter)
            where T : class, INotifyPropertyChanged =>
            new FilteredReadOnlyObservableCollection<ReadOnlyObservableCollection<T>, T, PropertyChangedEventArgs>(
                self,
                filter,
                x => x.PropertyChangedAsObservable());

        /// <summary>
        /// create IFilteredReadOnlyObservableCollection from ReadOnlyObservableCollection
        /// </summary>
        /// <typeparam name="T">Type of collection item.</typeparam>
        /// <typeparam name="U">Type of return type for elementStatusChangedFactory</typeparam>
        /// <param name="self">Source collection.</param>
        /// <param name="filter">Filter function.</param>
        /// <param name="elementStatusChangedFactory">IObservable to notify source status changed</param>
        /// <returns></returns>
        public static IFilteredReadOnlyObservableCollection<T> ToFilteredReadOnlyObservableCollection<T, U>(this ReadOnlyObservableCollection<T> self, Func<T, bool> filter, Func<T, IObservable<U>> elementStatusChangedFactory)
            where T : class, INotifyPropertyChanged =>
            new FilteredReadOnlyObservableCollection<ReadOnlyObservableCollection<T>, T, U>(self, filter, elementStatusChangedFactory);

        /// <summary>
        /// create IFilteredReadOnlyObservableCollection from ObservableCollection
        /// </summary>
        /// <typeparam name="T">Type of collection item.</typeparam>
        /// <typeparam name="U">Type of return type for elementStatusChangedFactory</typeparam>
        /// <param name="self">Source collection.</param>
        /// <param name="filter">Filter function.</param>
        /// <param name="elementStatusChangedFactory">IObservable to notify source status changed</param>
        /// <returns></returns>
        public static IFilteredReadOnlyObservableCollection<T> ToFilteredReadOnlyObservableCollection<T, U>(this ObservableCollection<T> self, Func<T, bool> filter, Func<T, IObservable<U>> elementStatusChangedFactory)
            where T : class, INotifyPropertyChanged =>
            new FilteredReadOnlyObservableCollection<ObservableCollection<T>, T, U>(self, filter, elementStatusChangedFactory);

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
