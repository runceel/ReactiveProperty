using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reactive.Bindings.Extensions;
using System.Reactive.Disposables;
using System.Collections;
using Reactive.Bindings.Notifiers;
using System.Reactive.Concurrency;

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
        private Func<TElement, bool> Filter { get; }
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
            this.Source = source;
            this.Filter = filter;

            {
                // initialize
                foreach (var item in source)
                {
                    var isTarget = filter(item);
                    this.IndexList.Add(isTarget ? (int?)ItemsCount : null);
                    if (isTarget)
                    {
                        this.ItemsCount++;
                        this.InnerCollection.Add(item);
                    }
                }
            }

            {
                // propertychanged
                source.ObserveElementPropertyChanged<TCollection, TElement>()
                    .Subscribe(x =>
                    {
                        var index = source.IndexOf(x.Sender);
                        var filteredIndex = this.IndexList[index];
                        var isTarget = filter(x.Sender);
                        if (isTarget && filteredIndex == null)
                        {
                            // add
                            this.AppearNewItem(index);
                        }
                        else if (!isTarget && filteredIndex.HasValue)
                        {
                            // remove
                            this.DisappearItem(index);
                            this.IndexList[index] = null;
                            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, x.Sender, filteredIndex.Value));
                        }
                    })
                    .AddTo(this.Subscription);
            }
            {
                // collection changed(support single changed only)
                source.CollectionChangedAsObservable()
                    .Subscribe(x =>
                    {
                        switch (x.Action)
                        {
                            case NotifyCollectionChangedAction.Add:
                                // appear
                                this.IndexList.Insert(x.NewStartingIndex, null);
                                if (filter(x.NewItems.Cast<TElement>().Single()))
                                {
                                    this.AppearNewItem(x.NewStartingIndex);
                                }
                                break;
                            case NotifyCollectionChangedAction.Move:
                                throw new NotSupportedException("Move is not supported");
                            case NotifyCollectionChangedAction.Remove:
                                var removedIndex = this.IndexList[x.OldStartingIndex];
                                if (removedIndex.HasValue)
                                {
                                    this.DisappearItem(x.OldStartingIndex);
                                    this.IndexList.RemoveAt(x.OldStartingIndex);
                                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
                                        x.OldItems.Cast<TElement>().Single(), removedIndex.Value));
                                }
                                else
                                {
                                    this.IndexList.RemoveAt(x.OldStartingIndex);
                                }
                                break;
                            case NotifyCollectionChangedAction.Replace:
                                var index = this.IndexList[x.NewStartingIndex];
                                var isTarget = this.Filter(x.NewItems.Cast<TElement>().Single());
                                if (index == null && isTarget)
                                {
                                    // add
                                    this.AppearNewItem(x.NewStartingIndex);
                                }
                                else if (index.HasValue && isTarget)
                                {
                                    // replace
                                    this.InnerCollection[index.Value] = x.NewItems.Cast<TElement>().Single();
                                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
                                        x.NewItems.Cast<TElement>().Single(), x.OldItems.Cast<TElement>().Single(), index.Value));
                                }
                                else if (index.HasValue && !isTarget)
                                {
                                    // remove
                                    this.DisappearItem(x.NewStartingIndex);
                                    this.IndexList[x.NewStartingIndex] = null;
                                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
                                        x.OldItems.Cast<TElement>().Single(), index.Value));
                                }

                                break;
                            case NotifyCollectionChangedAction.Reset:
                                this.IndexList.Clear();
                                this.InnerCollection.Clear();
                                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                                if (source.Any())
                                {
                                    throw new NotSupportedException("Reset is clear only");
                                }
                                break;
                            default:
                                throw new InvalidOperationException();
                        }
                    })
                    .AddTo(this.Subscription);
            }
        }

        /// <summary>
        /// Count
        /// </summary>
        public int Count =>  this.InnerCollection.Count;


        bool IList.IsFixedSize => false;

        bool IList.IsReadOnly => true;


        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => null;

        public TElement this[int index] => this.InnerCollection[index];

        object IList.this[int index]
        {
            get
            {
                return this.InnerCollection[index];
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
        public IEnumerator<TElement> GetEnumerator() => this.InnerCollection.GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => this.GetEnumerator();

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    this.ItemsCount++;
                    break;
                case NotifyCollectionChangedAction.Remove:
                    this.ItemsCount--;
                    break;
                case NotifyCollectionChangedAction.Reset:
                    this.ItemsCount = 0;
                    break;
            }
            this.CollectionChanged?.Invoke(this, e);
        }

        /// <summary>
        /// disconnect source collection.
        /// </summary>
        public void Dispose()
        {
            if (this.Subscription.IsDisposed) { return; }
            this.Subscription.Dispose();
        }

        private int FindNearIndex(int position) => this.IndexList.Take(position).Reverse().FirstOrDefault(x => x != null) ?? -1;

        private void AppearNewItem(int index)
        {
            var nearIndex = this.FindNearIndex(index);
            this.IndexList[index] = nearIndex + 1;
            for (int i = index + 1; i < this.IndexList.Count; i++)
            {
                if (this.IndexList[i].HasValue) { this.IndexList[i]++; }
            }
            this.InnerCollection.Insert(this.IndexList[index].Value, this.Source[index]);
            this.OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
                    this.Source[index], this.IndexList[index].Value));
        }

        private void DisappearItem(int index)
        {
            this.InnerCollection.RemoveAt(this.IndexList[index].Value);
            for (int i = index; i < IndexList.Count; i++)
            {
                if (IndexList[i].HasValue) { this.IndexList[i]--; }
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

        bool IList.Contains(object value) => this.InnerCollection.Contains(value);

        int IList.IndexOf(object value) => this.InnerCollection.IndexOf((TElement)value);

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

        void ICollection.CopyTo(Array array, int index) => this.InnerCollection.CopyTo((TElement[])array, index);
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
        /// <param name="self"></param>
        /// <returns></returns>
        public static ReadOnlyReactiveCollection<T> ToReadOnlyReactiveCollection<T>(this IFilteredReadOnlyObservableCollection<T> self, IScheduler scheduler = null, bool disposeElement = true)
            where T : class, INotifyPropertyChanged =>
            self.ToReadOnlyReactiveCollection(x => x, scheduler, disposeElement);

        /// <summary>
        /// create ReadOnlyReactiveCollection from IFilteredReadOnlyObservableCollection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="self"></param>
        /// <param name="converter"></param>
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
