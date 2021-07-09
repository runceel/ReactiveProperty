using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Reactive.Bindings
{
    /// <summary>
    /// ObservableCollection that operate on scheduler use by ***OnScheduler methods.
    /// </summary>
    public class ReactiveCollection<T> : ObservableCollection<T>, IDisposable
    {
        private readonly IDisposable _subscription;
        private readonly IScheduler _scheduler;

        /// <summary>
        /// Operate scheduler is UIDispatcherScheduler.
        /// </summary>
        public ReactiveCollection()
            : this(ReactivePropertyScheduler.Default)
        { }

        /// <summary>
        /// Operate scheduler is argument's scheduler.
        /// </summary>
        public ReactiveCollection(IScheduler scheduler)
        {
            _scheduler = scheduler;
            _subscription = Disposable.Empty;
        }

        /// <summary>
        /// Source sequence as ObservableCollection. Operate scheduler is UIDispatcherScheduler.
        /// </summary>
        public ReactiveCollection(IObservable<T> source)
            : this(source, ReactivePropertyScheduler.Default)
        {
        }

        /// <summary>
        /// Source sequence as ObservableCollection. Operate scheduler is argument's scheduler.
        /// </summary>
        public ReactiveCollection(IObservable<T> source, IScheduler scheduler)
        {
            _scheduler = scheduler;
            _subscription = source.ObserveOn(scheduler).Subscribe(Add);
        }

        /// <summary>
        /// Add called on scheduler
        /// </summary>
        public void AddOnScheduler(T item) => _scheduler.Schedule(() => Add(item));

        /// <summary>
        /// Add called on scheduler
        /// </summary>
        /// <param name="items"></param>
        public void AddRangeOnScheduler(params T[] items)
        {
            _scheduler.Schedule(() =>
            {
                foreach (var item in items) { Add(item); }
            });
        }

        /// <summary>
        /// Add called on scheduler
        /// </summary>
        /// <param name="items"></param>
        public void AddRangeOnScheduler(IEnumerable<T> items) => AddRangeOnScheduler(items.ToArray());

        /// <summary>
        /// Clear called on scheduler
        /// </summary>
        public void ClearOnScheduler() => _scheduler.Schedule(() => Clear());

        /// <summary>
        /// Get(indexer get) called on scheduler, IObservable length is one.
        /// </summary>
        public IObservable<T> GetOnScheduler(int index) => Observable.Start(() => this[index], _scheduler);

        /// <summary>
        /// Insert called on scheduler
        /// </summary>
        public void InsertOnScheduler(int index, T item) => _scheduler.Schedule(() => Insert(index, item));

        /// <summary>
        /// Move called on scheduler
        /// </summary>
        public void MoveOnScheduler(int oldIndex, int newIndex) => _scheduler.Schedule(() => Move(oldIndex, newIndex));

        /// <summary>
        /// Remove called on scheduler
        /// </summary>
        public void RemoveOnScheduler(T item) => _scheduler.Schedule(() => Remove(item));

        /// <summary>
        /// RemoveAt called on scheduler
        /// </summary>
        public void RemoveAtOnScheduler(int index) => _scheduler.Schedule(() => RemoveAt(index));

        /// <summary>
        /// Set(indexer set) called on scheduler
        /// </summary>
        public void SetOnScheduler(int index, T value) => _scheduler.Schedule(() => this[index] = value);

        /// <summary>
        /// Unsubscribe source sequence.
        /// </summary>
        public void Dispose() => _subscription.Dispose();
    }

    /// <summary>
    /// Reactive Collection Observable Extensions
    /// </summary>
    public static partial class ReactiveCollectionObservableExtensions
    {
        /// <summary>
        /// Source sequence as ObservableCollection. Operate scheduler is ReactivePropertyScheduler.
        /// </summary>
        public static ReactiveCollection<T> ToReactiveCollection<T>(this IObservable<T> source) =>
            new ReactiveCollection<T>(source);

        /// <summary>
        /// Source sequence as ObservableCollection. Operate scheduler is argument's scheduler.
        /// </summary>
        public static ReactiveCollection<T> ToReactiveCollection<T>(this IObservable<T> source, IScheduler scheduler) =>
            new ReactiveCollection<T>(source, scheduler);
    }
}
