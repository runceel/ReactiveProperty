using Codeplex.Reactive.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Codeplex.Reactive
{
    /// <summary>
    /// ReadOnly ReactiveCollection
    /// </summary>
    /// <typeparam name="T">collection item type</typeparam>
    public class ReadOnlyReactiveCollection<T> : IReadOnlyCollection<T>, INotifyCollectionChanged, IDisposable
    {
        private ObservableCollection<T> source = new ObservableCollection<T>();
        private CompositeDisposable token = new CompositeDisposable();

        /// <summary>
        /// CollectionChanged event
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Construct RxCollection from CollectionChanged.
        /// </summary>
        /// <param name="ox"></param>
        public ReadOnlyReactiveCollection(IObservable<CollectionChanged<T>> ox)
        {
            this.InitializeNotifyCollectionChanged();

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
        public ReadOnlyReactiveCollection(IObservable<T> ox, IObservable<Unit> onReset = null)
        {
            this.InitializeNotifyCollectionChanged();

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
        /// get collection count.
        /// </summary>
        public int Count
        {
            get { return this.source.Count; }
        }

        /// <summary>
        /// Get IEnumerable
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this.source.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private void InitializeNotifyCollectionChanged()
        {
            this.source.CollectionChanged += this.OnCollectionChanged;
            Disposable.Create(() =>
                this.source.CollectionChanged -= this.OnCollectionChanged).AddTo(this.token);
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var h = this.CollectionChanged;
            if (h != null)
            {
                h(this, e);
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
            return new ReadOnlyReactiveCollection<T>(self);
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
            return new ReadOnlyReactiveCollection<T>(self, onReset);
        }
    }
}
