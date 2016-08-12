using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Reactive.Bindings.Extensions;
using System.Collections.Generic;
using System.Reactive.Subjects;

namespace Reactive.Bindings
{
    /// <summary>
    /// ReadOnly ReactiveCollection
    /// </summary>
    /// <typeparam name="T">collection item type</typeparam>
    public class ReadOnlyReactiveCollection<T> : ReadOnlyObservableCollection<T>, IDisposable
    {
        private ObservableCollection<T> Source { get; set; }
		private CompositeDisposable Token { get; } = new CompositeDisposable();
        private bool DisposeElement { get; }

        /// <summary>
        /// Construct RxCollection from CollectionChanged.
        /// </summary>
        /// <param name="ox"></param>
        /// <param name="source"></param>
        public ReadOnlyReactiveCollection(IObservable<CollectionChanged<T>> ox, ObservableCollection<T> source, IScheduler scheduler = null, bool disposeElement = true)
            : base(source)
        {
            this.Source = source;
            scheduler = scheduler ?? ReactivePropertyScheduler.Default;
            this.DisposeElement = disposeElement;
            var subject = new Subject<CollectionChanged<T>>();

            subject.Where(v => v.Action == NotifyCollectionChangedAction.Add)
                .Subscribe(v =>
                {
                    this.Source.Insert(v.Index, v.Value);
                })
                .AddTo(this.Token);

            subject.Where(v => v.Action == NotifyCollectionChangedAction.Remove)
                .Subscribe(v =>
                {
                    var d = this.Source[v.Index] as IDisposable;
                    if (d != null) { InvokeDispose(d); }
                    this.Source.RemoveAt(v.Index);
                })
                .AddTo(this.Token);

            subject.Where(v => v.Action == NotifyCollectionChangedAction.Replace)
                .Subscribe(v =>
                {
                    var d = this.Source[v.Index] as IDisposable;
                    if (d != null) { InvokeDispose(d); }
                    this.Source[v.Index] = v.Value;
                })
                .AddTo(this.Token);

            subject.Where(v => v.Action == NotifyCollectionChangedAction.Reset)
                .Subscribe(v =>
                {
                    foreach (var item in source)
                    {
                        var d = item as IDisposable;
                        if (d != null) { InvokeDispose(d); }
                    }
                    this.Source.Clear();
                })
                .AddTo(this.Token);

            subject.Where(x => x.Action == NotifyCollectionChangedAction.Move)
                .Subscribe(x =>
                {
                    var targetValue = this.Source[x.OldIndex];
                    this.Source.RemoveAt(x.OldIndex);
                    this.Source.Insert(x.Index, targetValue);
                })
                .AddTo(this.Token);

            ox.ObserveOn(scheduler)
                .Subscribe(subject.OnNext, subject.OnError, subject.OnCompleted).AddTo(this.Token);
        }

        /// <summary>
        /// Create basic RxCollection from IO. 
        /// </summary>
        /// <param name="ox">Add</param>
        /// <param name="onReset">Clear</param>
        public ReadOnlyReactiveCollection(IObservable<T> ox, ObservableCollection<T> source, IObservable<Unit> onReset = null, IScheduler scheduler = null, bool disposeElement = true) : base(source)
        {
            this.Source = source;
            this.DisposeElement = disposeElement;
            scheduler = scheduler ?? ReactivePropertyScheduler.Default;

            ox
                .ObserveOn(scheduler)
                .Subscribe(value =>
            {
                this.Source.Add(value);
            })
            .AddTo(this.Token);
            
            if (onReset != null)
            {
                onReset
                    .ObserveOn(scheduler)
                    .Subscribe(_ =>
                        {
                            foreach (var item in source)
                            {
                                InvokeDispose(item);
                            }
                            this.Source.Clear();
                        }).AddTo(this.Token);
            }
        }

        /// <summary>
        /// Dispose managed resource.
        /// </summary>
        public virtual void Dispose()
        {
            if (this.Token.IsDisposed)
            {
                return;
            }

            this.Token.Dispose();
            foreach (var d in this.OfType<IDisposable>().ToArray()) { InvokeDispose(d); }
        }

        private void InvokeDispose(object item)
        {
            if (!this.DisposeElement)
            {
                return;
            }

            (item as IDisposable)?.Dispose();
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
        public static CollectionChanged<T> Remove(int index, T value) 
        {
            return new CollectionChanged<T> 
            { 
                Index = index, 
                Action = NotifyCollectionChangedAction.Remove ,
                Value = value,
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
                Action = NotifyCollectionChangedAction.Replace 
            };
        }

        /// <summary>
        /// Create move action.
        /// </summary>
        /// <param name="oldIndex"></param>
        /// <param name="newIndex"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static CollectionChanged<T> Move(int oldIndex, int newIndex, T value)
        {
            return new CollectionChanged<T>
            {
                OldIndex = oldIndex,
                Index = newIndex,
                Value = value,
                Action = NotifyCollectionChangedAction.Move
            };
        }

        /// <summary>
        /// Changed value.
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// Changed index.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Changed old index.(Move only)
        /// </summary>
        public int OldIndex { get; set; }

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
        public static ReadOnlyReactiveCollection<T> ToReadOnlyReactiveCollection<T>(this IObservable<CollectionChanged<T>> self, bool disposeElement = true) =>
            new ReadOnlyReactiveCollection<T>(self, new ObservableCollection<T>(), disposeElement: disposeElement);

        /// <summary>
        /// Create ReadOnlyReactiveCollection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static ReadOnlyReactiveCollection<T> ToReadOnlyReactiveCollection<T>(this IObservable<CollectionChanged<T>> self, IScheduler scheduler) =>
            new ReadOnlyReactiveCollection<T>(self, new ObservableCollection<T>(), scheduler, true);

        /// <summary>
        /// Create ReadOnlyReactiveCollection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static ReadOnlyReactiveCollection<T> ToReadOnlyReactiveCollection<T>(this IObservable<CollectionChanged<T>> self, IScheduler scheduler, bool disposeElement) =>
            new ReadOnlyReactiveCollection<T>(self, new ObservableCollection<T>(), scheduler, disposeElement: disposeElement);

        /// <summary>
        /// Create ReadOnlyReactiveCollection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="onReset"></param>
        /// <returns></returns>
        public static ReadOnlyReactiveCollection<T> ToReadOnlyReactiveCollection<T>(this IObservable<T> self, IObservable<Unit> onReset = null, IScheduler scheduler = null, bool disposeElement = true) =>
            new ReadOnlyReactiveCollection<T>(self, new ObservableCollection<T>(), onReset, scheduler, disposeElement: disposeElement);

        /// <summary>
        /// convert INotifyCollectionChanged to IO&lt;CollectionChanged&gt;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static IObservable<CollectionChanged<T>> ToCollectionChanged<T>(this INotifyCollectionChanged self)
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
                    .Select(e => CollectionChanged<T>.Remove(e.OldStartingIndex, e.OldItems.Cast<T>().First()))
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

                self.CollectionChangedAsObservable()
                    .Where(e => e.Action == NotifyCollectionChangedAction.Move)
                    .Select(e => CollectionChanged<T>.Move(e.OldStartingIndex, e.NewStartingIndex, e.NewItems.Cast<T>().First()))
                    .Subscribe(c => ox.OnNext(c))
                    .AddTo(d);

                return d;
            });
        }

        /// <summary>
        /// convert ObservableCollection to IO&lt;CollectionChanged&gt;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static IObservable<CollectionChanged<T>> ToCollectionChanged<T>(this ObservableCollection<T> self) =>
            ((INotifyCollectionChanged)self).ToCollectionChanged<T>();

        /// <summary>
        /// convert ReadOnlyObservableCollection to IO&lt;T&gt;
        /// </summary>
        /// <typeparam name="T">source type</typeparam>
        /// <param name="self">source</param>
        /// <returns>dest</returns>
        public static IObservable<CollectionChanged<T>> ToCollectionChanged<T>(this ReadOnlyObservableCollection<T> self) =>
            ((INotifyCollectionChanged)self).ToCollectionChanged<T>();

        /// <summary>
        /// Convert IEnumerable to ReadOnlyReactiveCollection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="collectionChanged"></param>
        /// <param name="scheduler"></param>
        /// <returns></returns>
        public static ReadOnlyReactiveCollection<T> ToReadOnlyReactiveCollection<T>(
            this IEnumerable<T> self, 
            IObservable<CollectionChanged<T>> collectionChanged, 
            IScheduler scheduler = null,
            bool disposeElement = true) =>
            self.ToReadOnlyReactiveCollection<T, T>(collectionChanged, x => x, scheduler, disposeElement);

        /// <summary>
        /// Convert IEnumerable to ReadOnlyReactiveCollection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="self"></param>
        /// <param name="collectionChanged"></param>
        /// <param name="converter"></param>
        /// <param name="scheduler"></param>
        /// <returns></returns>
        public static ReadOnlyReactiveCollection<U> ToReadOnlyReactiveCollection<T, U>(
            this IEnumerable<T> self, 
            IObservable<CollectionChanged<T>> collectionChanged, 
            Func<T, U> converter, 
            IScheduler scheduler = null,
            bool disposeElement = true)
        {
            var source = new ObservableCollection<U>(self.Select(converter));
            var convertedCollectionChanged = collectionChanged
                .Select(x => new CollectionChanged<U>
                {
                    Action = x.Action,
                    Index = x.Index,
                    OldIndex = x.OldIndex,
                    Value = object.ReferenceEquals(x.Value, null) ? default(U) :
                        x.Action == NotifyCollectionChangedAction.Add || x.Action == NotifyCollectionChangedAction.Replace ? converter(x.Value) : default(U),
                });
            return new ReadOnlyReactiveCollection<U>(convertedCollectionChanged, source, scheduler, disposeElement);
        }

        /// <summary>
        /// convert ObservableCollection to ReadOnlyReactiveCollection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static ReadOnlyReactiveCollection<T> ToReadOnlyReactiveCollection<T>(this ObservableCollection<T> self, IScheduler scheduler = null, bool disposeElement = true)
            where T : class =>
            self.ToReadOnlyReactiveCollection(x => x, scheduler, disposeElement);

        /// <summary>
        /// convert ObservableCollection to ReadOnlyReactiveCollection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="self"></param>
        /// <param name="converter"></param>
        /// <returns></returns>
        public static ReadOnlyReactiveCollection<U> ToReadOnlyReactiveCollection<T, U>(this ObservableCollection<T> self, Func<T, U> converter, IScheduler scheduler = null, bool disposeElement = true) =>
            ((IEnumerable<T>)self).ToReadOnlyReactiveCollection(
                self.ToCollectionChanged(),
                converter,
                scheduler,
                disposeElement);

        /// <summary>
        /// convert ReadOnlyObservableCollection to ReadOnlyReactiveCollection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static ReadOnlyReactiveCollection<T> ToReadOnlyReactiveCollection<T>(this ReadOnlyObservableCollection<T> self, IScheduler scheduler = null, bool disposeElement = true) 
            where T : class =>
            self.ToReadOnlyReactiveCollection(x => x, scheduler, disposeElement);

        /// <summary>
        /// convert ReadOnlyObservableCollection to ReadOnlyReactiveCollection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="self"></param>
        /// <param name="converter"></param>
        /// <returns></returns>
        public static ReadOnlyReactiveCollection<U> ToReadOnlyReactiveCollection<T, U>(this ReadOnlyObservableCollection<T> self, Func<T, U> converter, IScheduler scheduler = null, bool disposeElement = true) =>
            ((IEnumerable<T>)self).ToReadOnlyReactiveCollection(
                self.ToCollectionChanged(),
                converter,
                scheduler,
                disposeElement);
    }
}
