using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Reactive.Bindings.Extensions;

namespace Reactive.Bindings
{
    /// <summary>
    /// ReadOnly ReactiveCollection
    /// </summary>
    /// <typeparam name="T">collection item type</typeparam>
    public class ReadOnlyReactiveCollection<T> : ReadOnlyObservableCollection<T>, IDisposable
    {
        private readonly NotifyCollectionChangedEventArgs _resetEventArgs = new(NotifyCollectionChangedAction.Reset);
        private ObservableCollection<T> Source { get; set; }
        private bool _isSupressingEvents = false;

        private CompositeDisposable Token { get; } = new();

        private bool DisposeElement { get; }

        /// <summary>
        /// Construct RxCollection from CollectionChanged.
        /// </summary>
        /// <param name="ox">The ox.</param>
        /// <param name="source">The source.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="disposeElement">if set to <c>true</c> [dispose element].</param>
        public ReadOnlyReactiveCollection(IObservable<CollectionChanged<T>> ox, ObservableCollection<T> source, IScheduler scheduler = null, bool disposeElement = true)
            : base(source)
        {
            Source = source;
            scheduler = scheduler ?? ReactivePropertyScheduler.Default;
            DisposeElement = disposeElement;
            var subject = new Subject<CollectionChanged<T>>();

            subject.Where(v => v.Action == NotifyCollectionChangedAction.Add)
                .Subscribe(v =>
                {
                    Source.Insert(v.Index, v.Value);
                })
                .AddTo(Token);

            subject.Where(v => v.Action == NotifyCollectionChangedAction.Remove)
                .Subscribe(v =>
                {
                    if (Source[v.Index] is IDisposable d) { InvokeDispose(d); }
                    Source.RemoveAt(v.Index);
                })
                .AddTo(Token);

            subject.Where(v => v.Action == NotifyCollectionChangedAction.Replace)
                .Subscribe(v =>
                {
                    if (Source[v.Index] is IDisposable d) { InvokeDispose(d); }
                    Source[v.Index] = v.Value;
                })
                .AddTo(Token);

            subject.Where(v => v.Action == NotifyCollectionChangedAction.Reset)
                .Subscribe(v => ResetCollection(v))
                .AddTo(Token);

            subject.Where(x => x.Action == NotifyCollectionChangedAction.Move)
                .Subscribe(x => Source.Move(x.OldIndex, x.Index))
                .AddTo(Token);

            ox.ObserveOn(scheduler)
                .Subscribe(subject).AddTo(Token);
        }

        /// <summary>
        /// Create basic RxCollection from IO.
        /// </summary>
        /// <param name="ox">Add</param>
        /// <param name="source">The source.</param>
        /// <param name="onReset">Clear</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="disposeElement">if set to <c>true</c> [dispose element].</param>
        public ReadOnlyReactiveCollection(IObservable<T> ox, ObservableCollection<T> source, IObservable<Unit> onReset = null, IScheduler scheduler = null, bool disposeElement = true) : base(source)
        {
            Source = source;
            DisposeElement = disposeElement;
            scheduler = scheduler ?? ReactivePropertyScheduler.Default;

            ox
                .ObserveOn(scheduler)
                .Subscribe(value =>
                {
                    Source.Add(value);
                })
            .AddTo(Token);

            if (onReset != null)
            {
                onReset
                    .ObserveOn(scheduler)
                    .Subscribe(_ => ResetCollection(CollectionChanged<T>.Reset))
                    .AddTo(Token);
            }
        }

        /// <summary>
        /// Dispose managed resource.
        /// </summary>
        public virtual void Dispose()
        {
            if (Token.IsDisposed)
            {
                return;
            }

            Token.Dispose();
            foreach (var d in this.OfType<IDisposable>().ToArray()) { InvokeDispose(d); }
        }


        /// <inheritdoc />
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (_isSupressingEvents) { return; }
            base.OnCollectionChanged(args);
        }

        private void ResetCollection(CollectionChanged<T> collectionChanged)
        {
            _isSupressingEvents = true;
            try
            {
                foreach (var item in Source)
                {
                    if (item is IDisposable d) { InvokeDispose(d); }
                }
                Source.Clear();

                if (collectionChanged.Source != null)
                {
                    foreach (var x in collectionChanged.Source)
                    {
                        Source.Add(x);
                    }
                }
            }
            finally
            {
                _isSupressingEvents = false;
            }

            OnCollectionChanged(_resetEventArgs);
        }



        private void InvokeDispose(object item)
        {
            if (!DisposeElement)
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
        public static CollectionChanged<T> Reset { get; } = new()
        {
            Action = NotifyCollectionChangedAction.Reset
        };

        /// <summary>
        /// Reset action with source collection
        /// </summary>
        /// <param name="source">An event source collection.</param>
        public static CollectionChanged<T> ResetWithSource(IEnumerable<T> source) =>
            new CollectionChanged<T>
            {
                Action = NotifyCollectionChangedAction.Reset,
                Source = source,
            };

        /// <summary>
        /// Create Remove action
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static CollectionChanged<T> Remove(int index, T value)
        {
            return new CollectionChanged<T>
            {
                Index = index,
                Action = NotifyCollectionChangedAction.Remove,
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
        /// イベントソースのコレクション
        /// </summary>
        public IEnumerable<T> Source { get; init; }

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

    /// <summary>
    /// </summary>
    /// <seealso cref="System.Collections.ObjectModel.ReadOnlyObservableCollection{T}"/>
    /// <seealso cref="System.IDisposable"/>
    public static class ReadOnlyReactiveCollection
    {
        /// <summary>
        /// Create ReadOnlyReactiveCollection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self">The self.</param>
        /// <param name="disposeElement">if set to <c>true</c> [dispose element].</param>
        /// <returns></returns>
        public static ReadOnlyReactiveCollection<T> ToReadOnlyReactiveCollection<T>(this IObservable<CollectionChanged<T>> self, bool disposeElement = true) =>
            new(self, new ObservableCollection<T>(), disposeElement: disposeElement);

        /// <summary>
        /// Create ReadOnlyReactiveCollection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self">The self.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <returns></returns>
        public static ReadOnlyReactiveCollection<T> ToReadOnlyReactiveCollection<T>(this IObservable<CollectionChanged<T>> self, IScheduler scheduler) =>
            new(self, new ObservableCollection<T>(), scheduler, true);

        /// <summary>
        /// Create ReadOnlyReactiveCollection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self">The self.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="disposeElement">if set to <c>true</c> [dispose element].</param>
        /// <returns></returns>
        public static ReadOnlyReactiveCollection<T> ToReadOnlyReactiveCollection<T>(this IObservable<CollectionChanged<T>> self, IScheduler scheduler, bool disposeElement) =>
            new(self, new ObservableCollection<T>(), scheduler, disposeElement: disposeElement);

        /// <summary>
        /// Create ReadOnlyReactiveCollection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self">The self.</param>
        /// <param name="onReset">The on reset.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="disposeElement">if set to <c>true</c> [dispose element].</param>
        /// <returns></returns>
        public static ReadOnlyReactiveCollection<T> ToReadOnlyReactiveCollection<T>(this IObservable<T> self, IObservable<Unit> onReset = null, IScheduler scheduler = null, bool disposeElement = true) =>
            new(self, new ObservableCollection<T>(), onReset, scheduler, disposeElement: disposeElement);

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
                void collectionChanged(object sender, NotifyCollectionChangedEventArgs e)
                {
                    var collectionChanged = e.Action switch
                    {
                        NotifyCollectionChangedAction.Add => CollectionChanged<T>.Add(e.NewStartingIndex, e.NewItems.Cast<T>().First()),
                        NotifyCollectionChangedAction.Remove => CollectionChanged<T>.Remove(e.OldStartingIndex, e.OldItems.Cast<T>().First()),
                        NotifyCollectionChangedAction.Replace => CollectionChanged<T>.Replace(e.NewStartingIndex, e.NewItems.Cast<T>().First()),
                        NotifyCollectionChangedAction.Reset => CollectionChanged<T>.ResetWithSource(sender as IEnumerable<T>),
                        NotifyCollectionChangedAction.Move => CollectionChanged<T>.Move(e.OldStartingIndex, e.NewStartingIndex, e.NewItems.Cast<T>().First()),
                        _ => throw new InvalidOperationException($"Unknown NotifyCollectionChangedAction value: {e.Action}"),
                    };
                    ox.OnNext(collectionChanged);
                }

                self.CollectionChanged += collectionChanged;
                return Disposable.Create(
                    (Source: self, Handler: (NotifyCollectionChangedEventHandler)collectionChanged),
                    static state => state.Source.CollectionChanged -= state.Handler);
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
        /// <param name="self">The self.</param>
        /// <param name="collectionChanged">The collection changed.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="disposeElement">if set to <c>true</c> [dispose element].</param>
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
        /// <param name="self">The self.</param>
        /// <param name="collectionChanged">The collection changed.</param>
        /// <param name="converter">The converter.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="disposeElement">if set to <c>true</c> [dispose element].</param>
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
                    Source = x.Source?.Select(x => converter(x)),
                    Action = x.Action,
                    Index = x.Index,
                    OldIndex = x.OldIndex,
                    Value = ReferenceEquals(x.Value, null) ? default(U) :
                        x.Action == NotifyCollectionChangedAction.Add || x.Action == NotifyCollectionChangedAction.Replace ? converter(x.Value) : default(U),
                });
            return new ReadOnlyReactiveCollection<U>(convertedCollectionChanged, source, scheduler, disposeElement);
        }

        /// <summary>
        /// convert ObservableCollection to ReadOnlyReactiveCollection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self">The self.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="disposeElement">if set to <c>true</c> [dispose element].</param>
        /// <returns></returns>
        public static ReadOnlyReactiveCollection<T> ToReadOnlyReactiveCollection<T>(this ObservableCollection<T> self, IScheduler scheduler = null, bool disposeElement = true)
            where T : class =>
            self.ToReadOnlyReactiveCollection(x => x, scheduler, disposeElement);

        /// <summary>
        /// convert ObservableCollection to ReadOnlyReactiveCollection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="self">The self.</param>
        /// <param name="converter">The converter.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="disposeElement">if set to <c>true</c> [dispose element].</param>
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
        /// <param name="self">The self.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="disposeElement">if set to <c>true</c> [dispose element].</param>
        /// <returns></returns>
        public static ReadOnlyReactiveCollection<T> ToReadOnlyReactiveCollection<T>(this ReadOnlyObservableCollection<T> self, IScheduler scheduler = null, bool disposeElement = true)
            where T : class =>
            self.ToReadOnlyReactiveCollection(x => x, scheduler, disposeElement);

        /// <summary>
        /// convert ReadOnlyObservableCollection to ReadOnlyReactiveCollection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="self">The self.</param>
        /// <param name="converter">The converter.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="disposeElement">if set to <c>true</c> [dispose element].</param>
        /// <returns></returns>
        public static ReadOnlyReactiveCollection<U> ToReadOnlyReactiveCollection<T, U>(this ReadOnlyObservableCollection<T> self, Func<T, U> converter, IScheduler scheduler = null, bool disposeElement = true) =>
            ((IEnumerable<T>)self).ToReadOnlyReactiveCollection(
                self.ToCollectionChanged(),
                converter,
                scheduler,
                disposeElement);
    }
}
