using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;

namespace Codeplex.Reactive
{
    /// <summary>
    /// ObservableCollection that operate on scheduler use by ***OnScheduler methods.
    /// </summary>
    public class ReactiveCollection<T> : ObservableCollection<T>, IDisposable
    {
        readonly IDisposable subscription;
        readonly IScheduler scheduler;

        /// <summary>Operate scheduler is UIDispatcherScheduler.</summary>
        public ReactiveCollection()
            : this(UIDispatcherScheduler.Default)
        { }

        /// <summary>Operate scheduler is argument's scheduler.</summary>
        public ReactiveCollection(IScheduler scheduler)
        {
            this.scheduler = scheduler;
            this.subscription = Disposable.Empty;
        }

        /// <summary>Source sequence as ObservableCollection. Operate scheduler is UIDispatcherScheduler.</summary>
        public ReactiveCollection(IObservable<T> source)
            : this(source, UIDispatcherScheduler.Default)
        {
        }

        /// <summary>Source sequence as ObservableCollection. Operate scheduler is argument's scheduler.</summary>
        public ReactiveCollection(IObservable<T> source, IScheduler scheduler)
        {
            this.scheduler = scheduler;
            this.subscription = source.ObserveOn(scheduler).Subscribe(this.Add);
        }

        /// <summary>Add called on scheduler</summary>
        public void AddOnScheduler(T item)
        {
            scheduler.Schedule(() => Add(item));
        }

        /// <summary>Clear called on scheduler</summary>
        public void ClearOnScheduler()
        {
            scheduler.Schedule(() => Clear());
        }

        /// <summary>Get(indexer get) called on scheduler, IObservable length is one.</summary>
        public IObservable<T> GetOnScheduler(int index)
        {
            var result = Observable.Start(() => this[index], scheduler);
            return result;
        }

        /// <summary>Insert called on scheduler</summary>
        public void InsertOnScheduler(int index, T item)
        {
            scheduler.Schedule(() => Insert(index, item));
        }

        /// <summary>Move called on scheduler</summary>
        public void MoveOnScheduler(int oldIndex, int newIndex)
        {
            scheduler.Schedule(() => Move(oldIndex, newIndex));
        }

        /// <summary>Remove called on scheduler</summary>
        public void RemoveOnScheduler(T item)
        {
            scheduler.Schedule(() => Remove(item));
        }

        /// <summary>RemoveAt called on scheduler</summary>
        public void RemoveAtOnScheduler(int index)
        {
            scheduler.Schedule(() => RemoveAt(index));
        }

        /// <summary>Set(indexer set) called on scheduler</summary>
        public void SetOnScheduler(int index, T value)
        {
            scheduler.Schedule(() => this[index] = value);
        }

        /// <summary>Unsubcribe source sequence.</summary>
        public void Dispose()
        {
            subscription.Dispose();
        }
    }

    public static partial class ReactiveCollectionObservableExtensions
    {
        /// <summary>Source sequence as ObservableCollection. Operate scheduler is UIDispatcherScheduler.</summary>
        public static ReactiveCollection<T> ToReactiveCollection<T>(this IObservable<T> source)
        {
            return new ReactiveCollection<T>(source);
        }

        /// <summary>Source sequence as ObservableCollection. Operate scheduler is argument's scheduler.</summary>
        public static ReactiveCollection<T> ToReactiveCollection<T>(this IObservable<T> source, IScheduler scheduler)
        {
            return new ReactiveCollection<T>(source, scheduler);
        }
    }
}