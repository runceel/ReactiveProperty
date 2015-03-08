using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Reactive.Bindings.Extensions;

namespace Reactive.Bindings
{
    /// <summary>
    /// ReadOnly ReactiveCollection
    /// </summary>
    /// <typeparam name="T">collection item type</typeparam>
    public class ReadOnlyReactiveCollection<T> : ReadOnlyObservableCollection<T>, IDisposable
    {
        private readonly ObservableCollection<T> source;
		private readonly CompositeDisposable token = new CompositeDisposable();

        /// <summary>
        /// Construct RxCollection from CollectionChanged.
        /// </summary>
        /// <param name="ox"></param>
        /// <param name="source"></param>
        public ReadOnlyReactiveCollection(IObservable<CollectionChanged<T>> ox, ObservableCollection<T> source, IScheduler scheduler = null)
            : base(source)
        {
            this.source = source;
           scheduler = scheduler ?? UIDispatcherScheduler.Default;

            ox.Where(v => v.Action == NotifyCollectionChangedAction.Add)
                .ObserveOn(scheduler)
                .Subscribe(v =>
                {
                    this.source.Insert(v.Index, v.Value);
                })
                .AddTo(this.token);

            ox.Where(v => v.Action == NotifyCollectionChangedAction.Remove)
                .ObserveOn(scheduler)
                .Subscribe(v =>
                {
                    var d = this.source[v.Index] as IDisposable;
                    if (d != null) { d.Dispose(); }
                    this.source.RemoveAt(v.Index);
                })
                .AddTo(this.token);

            ox.Where(v => v.Action == NotifyCollectionChangedAction.Replace)
                .ObserveOn(scheduler)
                .Subscribe(v =>
                {
                    var d = this.source[v.Index] as IDisposable;
                    if (d != null) { d.Dispose(); }
                    this.source[v.Index] = v.Value;
                })
                .AddTo(this.token);

            ox.Where(v => v.Action == NotifyCollectionChangedAction.Reset)
                .ObserveOn(scheduler)
                .Subscribe(v =>
                {
                    foreach (var item in source)
                    {
                        var d = item as IDisposable;
                        if (d != null) { d.Dispose(); }
                    }
                    this.source.Clear();
                })
                .AddTo(this.token);
        }

        /// <summary>
        /// Create basic RxCollection from IO. 
        /// </summary>
        /// <param name="ox">Add</param>
        /// <param name="onReset">Clear</param>
        public ReadOnlyReactiveCollection(IObservable<T> ox, ObservableCollection<T> source, IObservable<Unit> onReset = null, IScheduler scheduler = null) : base(source)
        {
            this.source = source;
            scheduler = scheduler ?? UIDispatcherScheduler.Default;

            ox
                .ObserveOn(scheduler)
                .Subscribe(value =>
            {
                this.source.Add(value);
            })
            .AddTo(this.token);
            
            if (onReset != null)
            {
                onReset
                    .ObserveOn(scheduler)
                    .Subscribe(_ =>
                        {
                            foreach (var item in source)
                            {
                                var d = item as IDisposable;
                                if (d != null) { d.Dispose(); }
                            }
                            this.source.Clear();
                        }).AddTo(this.token);
            }
        }

        /// <summary>
        /// Dispose managed resource.
        /// </summary>
        public void Dispose()
        {
            if (this.token.IsDisposed)
            {
                return;
            }

            foreach (var d in this.OfType<IDisposable>().ToArray()) { d.Dispose(); }
            this.token.Dispose();
        }

    }

    /// <summary>
    /// CollectionChanged action
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CollectionChanged<T>
    {
        /// <summary>
        /// Reset action
        /// </summary>
        public static readonly CollectionChanged<T> Reset = new CollectionChanged<T> 
        { 
            Action = NotifyCollectionChangedAction.Reset 
        };

        /// <summary>
        /// Create Remove action
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static CollectionChanged<T> Remove(int index) 
        {
            return new CollectionChanged<T> 
            { 
                Index = index, 
                Action = NotifyCollectionChangedAction.Remove 
            };
        }

        /// <summary>
        /// Create add action
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static CollectionChanged<T> Add(int index, T value)
        {
            return new CollectionChanged<T> 
            { 
                Index = index, 
                Value = value, 
                Action = NotifyCollectionChangedAction.Add 
            };
        }

        /// <summary>
        /// Create replace action
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static CollectionChanged<T> Replace(int index, T value)
        {
            return new CollectionChanged<T> 
            { 
                Index = index, 
                Value = value, 
                Action = NotifyCollectionChangedAction.Replace };
        }

        public T Value { get; set; }
        public int Index { get; set; }

        /// <summary>
        /// Support action is Add and Remove and Reset and Replace.
        /// </summary>
        public NotifyCollectionChangedAction Action { get; set; }

    }

    public static class ReadOnlyReactiveCollection
    {
        /// <summary>
        /// Create ReadOnlyReactiveCollection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static ReadOnlyReactiveCollection<T> ToReadOnlyReactiveCollection<T>(this IObservable<CollectionChanged<T>> self)
        {
            return new ReadOnlyReactiveCollection<T>(self, new ObservableCollection<T>());
        }

        /// <summary>
        /// Create ReadOnlyReactiveCollection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static ReadOnlyReactiveCollection<T> ToReadOnlyReactiveCollection<T>(this IObservable<CollectionChanged<T>> self, IScheduler scheduler)
        {
            return new ReadOnlyReactiveCollection<T>(self, new ObservableCollection<T>(), scheduler);
        }

        /// <summary>
        /// Create ReadOnlyReactiveCollection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="onReset"></param>
        /// <returns></returns>
        public static ReadOnlyReactiveCollection<T> ToReadOnlyReactiveCollection<T>(this IObservable<T> self, IObservable<Unit> onReset = null, IScheduler scheduler = null)
        {
            return new ReadOnlyReactiveCollection<T>(self, new ObservableCollection<T>(), onReset, scheduler);
        }

        /// <summary>
        /// convert ObservableCollection to IO&lt;CollectionChanged&gt;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static IObservable<CollectionChanged<T>> ToCollectionChanged<T>(this ObservableCollection<T> self)
        {
            return Observable.Create<CollectionChanged<T>>(ox =>
            {
                var d = new CompositeDisposable();
                self.CollectionChangedAsObservable()
                    .Where(e => e.Action == NotifyCollectionChangedAction.Add)
                    .Select(e => CollectionChanged<T>.Add(e.NewStartingIndex, e.NewItems.Cast<T>().First()))
                    .Subscribe(c => ox.OnNext(c))
                    .AddTo(d);

                self.CollectionChangedAsObservable()
                    .Where(e => e.Action == NotifyCollectionChangedAction.Remove)
                    .Select(e => CollectionChanged<T>.Remove(e.OldStartingIndex))
                    .Subscribe(c => ox.OnNext(c))
                    .AddTo(d);

                self.CollectionChangedAsObservable()
                    .Where(e => e.Action == NotifyCollectionChangedAction.Replace)
                    .Select(e => CollectionChanged<T>.Replace(e.NewStartingIndex, e.NewItems.Cast<T>().First()))
                    .Subscribe(c => ox.OnNext(c))
                    .AddTo(d);

                self.CollectionChangedAsObservable()
                    .Where(e => e.Action == NotifyCollectionChangedAction.Reset)
                    .Select(_ => CollectionChanged<T>.Reset)
                    .Subscribe(c => ox.OnNext(c))
                    .AddTo(d);

                return d;
            });
        }

        /// <summary>
        /// convert ReadOnlyObservableCollection to IO&lt;T&gt;
        /// </summary>
        /// <typeparam name="T">source type</typeparam>
        /// <param name="self">source</param>
        /// <returns>dest</returns>
        public static IObservable<CollectionChanged<T>> ToCollectionChanged<T>(this ReadOnlyObservableCollection<T> self)
        {
            return Observable.Create<CollectionChanged<T>>(ox =>
            {
                var d = new CompositeDisposable();
                self.CollectionChangedAsObservable()
                    .Where(e => e.Action == NotifyCollectionChangedAction.Add)
                    .Select(e => CollectionChanged<T>.Add(e.NewStartingIndex, e.NewItems.Cast<T>().First()))
                    .Subscribe(c => ox.OnNext(c))
                    .AddTo(d);

                self.CollectionChangedAsObservable()
                    .Where(e => e.Action == NotifyCollectionChangedAction.Remove)
                    .Select(e => CollectionChanged<T>.Remove(e.OldStartingIndex))
                    .Subscribe(c => ox.OnNext(c))
                    .AddTo(d);

                self.CollectionChangedAsObservable()
                    .Where(e => e.Action == NotifyCollectionChangedAction.Replace)
                    .Select(e => CollectionChanged<T>.Replace(e.NewStartingIndex, e.NewItems.Cast<T>().First()))
                    .Subscribe(c => ox.OnNext(c))
                    .AddTo(d);

                self.CollectionChangedAsObservable()
                    .Where(e => e.Action == NotifyCollectionChangedAction.Reset)
                    .Select(_ => CollectionChanged<T>.Reset)
                    .Subscribe(c => ox.OnNext(c))
                    .AddTo(d);

                return d;
            });
        }

        /// <summary>
        /// convert ObservableCollection to ReadOnlyReactiveCollection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static ReadOnlyReactiveCollection<T> ToReadOnlyReactiveCollection<T>(this ObservableCollection<T> self, IScheduler scheduler = null)
            where T : class
        {
            return self.ToReadOnlyReactiveCollection(x => x, scheduler);
        }

        /// <summary>
        /// convert ObservableCollection to ReadOnlyReactiveCollection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="self"></param>
        /// <param name="converter"></param>
        /// <returns></returns>
        public static ReadOnlyReactiveCollection<U> ToReadOnlyReactiveCollection<T, U>(this ObservableCollection<T> self, Func<T, U> converter, IScheduler scheduler = null)
        {
            var source = new ObservableCollection<U>(self.Select(converter));
            var collectionChanged = self
                .ToCollectionChanged()
                .Select(c => new CollectionChanged<U>
                {
                    Action = c.Action,
                    Index = c.Index,
                    Value = object.ReferenceEquals(c.Value, null) ? default(U) : converter(c.Value)
                });
            return new ReadOnlyReactiveCollection<U>(collectionChanged, source, scheduler);
        }

        /// <summary>
        /// convert ReadOnlyObservableCollection to ReadOnlyReactiveCollection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static ReadOnlyReactiveCollection<T> ToReadOnlyReactiveCollection<T>(this ReadOnlyObservableCollection<T> self, IScheduler scheduler = null)
            where T : class
        {
            return self.ToReadOnlyReactiveCollection(x => x, scheduler);
        }

        /// <summary>
        /// convert ReadOnlyObservableCollection to ReadOnlyReactiveCollection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="self"></param>
        /// <param name="converter"></param>
        /// <returns></returns>
        public static ReadOnlyReactiveCollection<U> ToReadOnlyReactiveCollection<T, U>(this ReadOnlyObservableCollection<T> self, Func<T, U> converter, IScheduler scheduler = null)
        {
            var source = new ObservableCollection<U>(self.Select(converter));
            var collectionChanged = self
                .ToCollectionChanged()
                .Select(c => new CollectionChanged<U>
                {
                    Action = c.Action,
                    Index = c.Index,
                    Value = object.ReferenceEquals(c.Value, null) ? default(U) : converter(c.Value)
                });
            return new ReadOnlyReactiveCollection<U>(collectionChanged, source, scheduler);
        }
    }
}
