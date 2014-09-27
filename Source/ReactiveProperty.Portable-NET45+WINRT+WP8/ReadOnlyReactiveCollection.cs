using Codeplex.Reactive.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Codeplex.Reactive
{
    /// <summary>
    /// ReadOnly ReactiveCollection
    /// </summary>
    /// <typeparam name="T">collection item type</typeparam>
    public class ReadOnlyReactiveCollection<T> : ReadOnlyObservableCollection<T>, IDisposable
    {
        private ObservableCollection<T> source;
        private CompositeDisposable token = new CompositeDisposable();

        /// <summary>
        /// Construct RxCollection from CollectionChanged.
        /// </summary>
        /// <param name="ox"></param>
        /// <param name="source"></param>
        public ReadOnlyReactiveCollection(IObservable<CollectionChanged<T>> ox, ObservableCollection<T> source)
            : base(source)
        {
            this.source = source;

            ox.Where(v => v.Action == NotifyCollectionChangedAction.Add)
                .Subscribe(v =>
                {
                    this.source.Insert(v.Index, v.Value);
                })
                .AddTo(this.token);

            ox.Where(v => v.Action == NotifyCollectionChangedAction.Remove)
                .Subscribe(v =>
                {
                    this.source.RemoveAt(v.Index);
                })
                .AddTo(this.token);

            ox.Where(v => v.Action == NotifyCollectionChangedAction.Replace)
                .Subscribe(v =>
                {
                    this.source[v.Index] = v.Value;
                })
                .AddTo(this.token);

            ox.Where(v => v.Action == NotifyCollectionChangedAction.Reset)
                .Subscribe(v =>
                {
                    this.source.Clear();
                })
                .AddTo(this.token);
        }

        /// <summary>
        /// Create basic RxCollection from IO. 
        /// </summary>
        /// <param name="ox">Add</param>
        /// <param name="onReset">Clear</param>
        public ReadOnlyReactiveCollection(IObservable<T> ox, ObservableCollection<T> source, IObservable<Unit> onReset = null) : base(source)
        {
            this.source = source;

            ox.Subscribe(value =>
            {
                this.source.Add(value);
            })
            .AddTo(this.token);
            
            if (onReset != null)
            {
                onReset.Subscribe(_ => this.source.Clear()).AddTo(this.token);
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
        /// <param name="onReset"></param>
        /// <returns></returns>
        public static ReadOnlyReactiveCollection<T> ToReadOnlyReactiveCollection<T>(this IObservable<T> self, IObservable<Unit> onReset = null)
        {
            return new ReadOnlyReactiveCollection<T>(self, new ObservableCollection<T>(), onReset);
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
        /// convert ObservableCollection to ReadOnlyReactiveCollection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static ReadOnlyReactiveCollection<T> ToReadOnlyReactiveCollection<T>(this ObservableCollection<T> self)
        {
            return self.ToCollectionChanged().ToReadOnlyReactiveCollection();
        }

        /// <summary>
        /// convert ObservableCollection to ReadOnlyReactiveCollection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="self"></param>
        /// <param name="converter"></param>
        /// <returns></returns>
        public static ReadOnlyReactiveCollection<U> ToReadOnlyReactiveCollection<T, U>(this ObservableCollection<T> self, Func<T, U> converter)
            where T : class
            where U : class
        {
            var source = new ObservableCollection<U>(self.Select(converter));
            var collectionChanged = self
                .ToCollectionChanged()
                .Select(c => new CollectionChanged<U>
                {
                    Action = c.Action,
                    Index = c.Index,
                    Value = c.Value == null ? null : converter(c.Value)
                });
            return new ReadOnlyReactiveCollection<U>(collectionChanged, source);
        }
    }
}
