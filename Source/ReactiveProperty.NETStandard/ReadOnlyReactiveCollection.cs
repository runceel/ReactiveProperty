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

namespace Reactive.Bindings;

/// <summary>
/// ReadOnly ReactiveCollection
/// </summary>
/// <typeparam name="T">collection item type</typeparam>
public class ReadOnlyReactiveCollection<T> : ReadOnlyObservableCollection<T>, IDisposable
{
    private readonly NotifyCollectionChangedEventArgs _resetEventArgs = new(NotifyCollectionChangedAction.Reset);
    private readonly ObservableCollection<T> _source;

    private readonly object _syncRoot = new();
    private readonly IScheduler _scheduler;
    private bool _isSupressingEvents = false;

    private readonly CompositeDisposable _token = new();

    private readonly bool _disposeElement;

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
        _source = source;
        _scheduler = scheduler ?? ReactivePropertyScheduler.Default;
        _disposeElement = disposeElement;
        var subject = new Subject<CollectionChanged<T>>();

        subject.Where(x => x.Action == NotifyCollectionChangedAction.Add)
            .Subscribe(x => ApplyCollectionChanged(x, static (source, args, _) =>
            {
                var index = args.Index;
                var values = args.Values.ToArray();
                foreach (var item in values)
                {
                    source.Insert(index++, item);
                }

                return new(NotifyCollectionChangedAction.Add, values, args.Index);
            }))
            .AddTo(_token);

        subject.Where(x => x.Action == NotifyCollectionChangedAction.Remove)
                .Subscribe(x => ApplyCollectionChanged(x, static (source, args, invokeDispose) =>
                {
                    var oldValue = source[args.Index];
                    invokeDispose(oldValue);
                    source.RemoveAt(args.Index);
                    return new(NotifyCollectionChangedAction.Remove, oldValue, args.Index);
                }))
                .AddTo(_token);

        subject.Where(x => x.Action == NotifyCollectionChangedAction.Replace)
                .Subscribe(x => ApplyCollectionChanged(x, static (source, args, invokeDispose) =>
                {
                    invokeDispose(source[args.Index]);
                    var oldValue = source[args.Index];
                    source[args.Index] = args.Value;
                    return new(NotifyCollectionChangedAction.Replace, args.Value, oldValue, args.Index);
                }))
                .AddTo(_token);

        subject.Where(x => x.Action == NotifyCollectionChangedAction.Reset)
                .Subscribe(x => ResetCollection(x))
                .AddTo(_token);

        subject.Where(x => x.Action == NotifyCollectionChangedAction.Move)
                .Subscribe(x => ApplyCollectionChanged(x, static (source, args, _) =>
                {
                    var value = source[args.OldIndex];
                    source.Move(args.OldIndex, args.Index);
                    return new(NotifyCollectionChangedAction.Move, value, args.Index, args.OldIndex);
                }))
                .AddTo(_token);

        ox.Subscribe(subject).AddTo(_token);
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
        _source = source;
        _scheduler = scheduler ?? ReactivePropertyScheduler.Default;
        _disposeElement = disposeElement;
        scheduler ??= ReactivePropertyScheduler.Default;

        ox.Select(x => CollectionChanged<T>.Add(_source.Count, x))
            .Subscribe(x => ApplyCollectionChanged(x, static (source, args, _) =>
            {
                source.Add(args.Value);
                return new(NotifyCollectionChangedAction.Add, args.Value, args.Index);
            }))
            .AddTo(_token);

        if (onReset != null)
        {
            onReset
                .Subscribe(_ => ResetCollection(CollectionChanged<T>.Reset))
                .AddTo(_token);
        }
    }

    /// <summary>
    /// Dispose managed resource.
    /// </summary>
    public virtual void Dispose()
    {
        if (_token.IsDisposed) return;

        _token.Dispose();
        foreach (var d in this.OfType<IDisposable>().ToArray()) { InvokeDispose(d); }
    }


    /// <inheritdoc />
    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
    {
        if (args == null) return;
        if (_isSupressingEvents) return; 
        _scheduler.Schedule(() => base.OnCollectionChanged(args));
    }

    private void ApplyCollectionChanged(
        CollectionChanged<T> collectionChanged,
        Func<ObservableCollection<T>, CollectionChanged<T>, Action<object>, NotifyCollectionChangedEventArgs> action)
    {
        NotifyCollectionChangedEventArgs args;
        lock (_syncRoot)
        {
            _isSupressingEvents = true;
            try
            {
                args = action(_source, collectionChanged, InvokeDispose);
            }
            finally
            {
                _isSupressingEvents = false;
            }
        }

        OnCollectionChanged(args);
    }

    private void ResetCollection(CollectionChanged<T> collectionChanged)
    {
        ApplyCollectionChanged(collectionChanged, (source, args, invokeDispose) =>
        {
            foreach (var item in source)
            {
                if (item is IDisposable d) { invokeDispose(d); }
            }
            source.Clear();

            if (collectionChanged.Source != null)
            {
                foreach (var x in collectionChanged.Source)
                {
                    source.Add(x);
                }
            }

            return _resetEventArgs;
        });
    }



    private void InvokeDispose(object item)
    {
        if (!_disposeElement)
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
        new()
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
    public static CollectionChanged<T> Remove(int index, T value) =>
        Remove(index, new[] { value });

    /// <summary>
    /// Create Remove action
    /// </summary>
    /// <param name="index">The index.</param>
    /// <param name="values">The value.</param>
    /// <returns></returns>
    public static CollectionChanged<T> Remove(int index, IEnumerable<T> values) =>
        new()
        {
            Index = index,
            Action = NotifyCollectionChangedAction.Remove,
            Values = values,
        };

    /// <summary>
    /// Create add action
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static CollectionChanged<T> Add(int index, T value) =>
        Add(index, new[] { value });

    /// <summary>
    /// Create add action
    /// </summary>
    /// <param name="index"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static CollectionChanged<T> Add(int index, IEnumerable<T> values) =>
        new()
        {
            Index = index,
            Values = values ?? Enumerable.Empty<T>(),
            Action = NotifyCollectionChangedAction.Add
        };

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
            Values = value is null ? Enumerable.Empty<T>() : new[] { value },
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
            Values = value is null ? Enumerable.Empty<T>() : new[] { value },
            Action = NotifyCollectionChangedAction.Move
        };
    }

    /// <summary>
    /// イベントソースのコレクション
    /// </summary>
    public IEnumerable<T> Source { get; init; }

    /// <summary>
    /// Changed values.
    /// </summary>
    public IEnumerable<T> Values { get; init; }

    /// <summary>
    /// Changed value.
    /// </summary>
    public T Value => Values == null ? default : Values.FirstOrDefault();

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
                    NotifyCollectionChangedAction.Add => CollectionChanged<T>.Add(e.NewStartingIndex, e.NewItems.Cast<T>()),
                    NotifyCollectionChangedAction.Remove => CollectionChanged<T>.Remove(e.OldStartingIndex, e.OldItems.Cast<T>()),
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
            .Select(x => x.Action switch
            {
                NotifyCollectionChangedAction.Add => CollectionChanged<U>.Add(x.Index, x.Values.Select(x => converter(x))),
                NotifyCollectionChangedAction.Remove => CollectionChanged<U>.Remove(x.Index, default(U)),
                NotifyCollectionChangedAction.Replace => CollectionChanged<U>.Replace(x.Index, converter(x.Value)),
                NotifyCollectionChangedAction.Reset => CollectionChanged<U>.ResetWithSource(source),
                NotifyCollectionChangedAction.Move => CollectionChanged<U>.Move(x.OldIndex, x.Index, default(U)),
                _ => throw new InvalidOperationException($"Unknown NotifyCollectionChangedAction value: {x.Action}"),
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
