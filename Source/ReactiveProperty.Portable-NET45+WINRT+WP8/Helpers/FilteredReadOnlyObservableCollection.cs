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

namespace Reactive.Bindings.Helpers
{
    /// <summary>
    /// real time filtered collection interface.
    /// </summary>
    /// <typeparam name="T">Type of collection item.</typeparam>
    public interface IFilteredReadOnlyObservableCollection<T> : INotifyCollectionChanged, IDisposable, IEnumerable<T>, IEnumerable
#if !NET4
, IReadOnlyList<T>
#endif
        where T : class, INotifyPropertyChanged
    {
        /// <summary>
        /// get collection item
        /// </summary>
        /// <param name="index">index</param>
        /// <returns></returns>
        T this[int index] { get; }
    }

    /// <summary>
    /// real time filtered collection.
    /// </summary>
    /// <typeparam name="TCollection">type of source collection</typeparam>
    /// <typeparam name="TElement">type of collection item</typeparam>
    public sealed class FilteredReadOnlyObservableCollection<TCollection, TElement> : IFilteredReadOnlyObservableCollection<TElement>
        where TCollection : INotifyCollectionChanged, IList<TElement>
        where TElement : class, INotifyPropertyChanged
    {
        private readonly TCollection source;
        private readonly Func<TElement, bool> filter;
        private readonly List<int?> indexList = new List<int?>();
        private readonly CompositeDisposable subscription = new CompositeDisposable();
        private int count;

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
            this.source = source;
            this.filter = filter;

            {
                // initialize
                foreach (var item in source.Select((x, i) => new { x, i }))
                {
                    var isTarget = filter(item.x);
                    this.indexList.Add(isTarget ? (int?)item.i : null);
                    if (isTarget) { this.count++; }
                }
            }

            {
                // propertychanged
                source.ObserveElementPropertyChanged<TCollection, TElement>()
                    .Subscribe(x =>
                    {
                        var index = source.IndexOf(x.Sender);
                        var filteredIndex = this.indexList[index];
                        var isTarget = filter(x.Sender);
                        if (isTarget && filteredIndex == null)
                        {
                            // add
                            this.AppearNewItem(index);
                        }
                        else if (!isTarget && filteredIndex.HasValue)
                        {
                            // remove
                            this.indexList[index] = null;
                            this.DisappearItem(index);
                            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, x.Sender, filteredIndex.Value));
                        }
                    })
                    .AddTo(this.subscription);
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
                                this.indexList.Insert(x.NewStartingIndex, null);
                                if (filter(x.NewItems.Cast<TElement>().Single()))
                                {
                                    this.AppearNewItem(x.NewStartingIndex);
                                }
                                break;
                            case NotifyCollectionChangedAction.Move:
                                throw new NotSupportedException("Move is not supported");
                            case NotifyCollectionChangedAction.Remove:
                                var removedIndex = this.indexList[x.OldStartingIndex];
                                this.indexList.RemoveAt(x.OldStartingIndex);
                                this.DisappearItem(x.OldStartingIndex);
                                if (removedIndex.HasValue)
                                {
                                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
                                        x.OldItems.Cast<TElement>().Single(), removedIndex.Value));
                                }
                                break;
                            case NotifyCollectionChangedAction.Replace:
                                var index = this.indexList[x.NewStartingIndex];
                                var isTarget = this.filter(x.NewItems.Cast<TElement>().Single());
                                if (index == null && isTarget)
                                {
                                    // add
                                    this.AppearNewItem(x.NewStartingIndex);
                                }
                                else if (index.HasValue && isTarget)
                                {
                                    // replace
                                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
                                        x.NewItems.Cast<TElement>().Single(), x.OldItems.Cast<TElement>().Single(), index.Value));
                                }
                                else if (index.HasValue && !isTarget)
                                {
                                    // remove
                                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
                                        x.OldItems.Cast<TElement>().Single(), index.Value));
                                }

                                break;
                            case NotifyCollectionChangedAction.Reset:
                                this.indexList.Clear();
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
                    .AddTo(this.subscription);
            }
        }

        /// <summary>
        /// get collection item.
        /// </summary>
        /// <param name="index">index</param>
        /// <returns>collection item</returns>
        public TElement this[int index]
        {
            get
            {
                var i = this.indexList.Where(x => x.HasValue).ElementAt(index);
                return this.source[i.Value];
            }
        }

        /// <summary>
        /// Count
        /// </summary>
        public int Count
        {
            get { return this.count; }
        }

        /// <summary>
        /// get enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<TElement> GetEnumerator()
        {
            return this.indexList.Where(x => x.HasValue)
                .Select(x => this.source[x.Value])
                .GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    this.count++;
                    break;
                case NotifyCollectionChangedAction.Remove:
                    this.count--;
                    break;
                case NotifyCollectionChangedAction.Reset:
                    this.count = 0;
                    break;
            }
            var h = this.CollectionChanged;
            if (h != null) { h(this, e); }
        }

        /// <summary>
        /// disconnect source collection.
        /// </summary>
        public void Dispose()
        {
            if (this.subscription.IsDisposed) { return; }
            this.subscription.Dispose();
        }

        private int FindNearIndex(int position)
        {
            var index = this.indexList.Take(position).Reverse().FirstOrDefault(x => x != null);
            return index ?? -1;
        }

        private void AppearNewItem(int index)
        {
            var nearIndex = this.FindNearIndex(index);
            this.indexList[index] = nearIndex + 1;
            for (int i = index + 1; i < this.indexList.Count; i++)
            {
                if (this.indexList[i].HasValue) { this.indexList[i]++; }
            }

            this.OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
                    this.source[index], this.indexList[index].Value));
        }

        private void DisappearItem(int index)
        {
            for (int i = index; i < indexList.Count; i++)
            {
                if (indexList[i].HasValue) { this.indexList[i]--; }
            }
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
            where T : class, INotifyPropertyChanged
        {
            return new FilteredReadOnlyObservableCollection<ObservableCollection<T>, T>(self, filter);
        }

        /// <summary>
        /// create IFilteredReadOnlyObservableCollection from ReadOnlyObservableCollection
        /// </summary>
        /// <typeparam name="T">Type of collection item.</typeparam>
        /// <param name="self">Source collection.</param>
        /// <param name="filter">Filter function.</param>
        /// <returns></returns>
        public static IFilteredReadOnlyObservableCollection<T> ToFilteredReadOnlyObservableCollection<T>(this ReadOnlyObservableCollection<T> self, Func<T, bool> filter)
            where T : class, INotifyPropertyChanged
        {
            return new FilteredReadOnlyObservableCollection<ReadOnlyObservableCollection<T>, T>(self, filter);
        }
    }
}
