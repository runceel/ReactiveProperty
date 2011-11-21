using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Windows.Threading;
#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#else
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
#endif

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
            Contract.Requires<ArgumentNullException>(scheduler != null);

            this.scheduler = scheduler;
            this.subscription = Disposable.Empty;
        }

        /// <summary>Source sequence as ObservableCollection. Operate scheduler is UIDispatcherScheduler.</summary>
        public ReactiveCollection(IObservable<T> source)
            : this(source, UIDispatcherScheduler.Default)
        {
            Contract.Requires<ArgumentNullException>(source != null);
        }

        /// <summary>Source sequence as ObservableCollection. Operate scheduler is argument's scheduler.</summary>
        public ReactiveCollection(IObservable<T> source, IScheduler scheduler)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(scheduler != null);

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
            Contract.Requires<ArgumentOutOfRangeException>(index >= 0);
            Contract.Ensures(Contract.Result<IObservable<T>>() != null);

            var result = Observable.Start(() => this[index], scheduler);

            Contract.Assume(result != null);
            return result;
        }

        /// <summary>Insert called on scheduler</summary>
        public void InsertOnScheduler(int index, T item)
        {
            Contract.Requires<ArgumentOutOfRangeException>(index >= 0);

            scheduler.Schedule(() => Insert(index, item));
        }

#if !SILVERLIGHT

        /// <summary>Move called on scheduler</summary>
        public void MoveOnScheduler(int oldIndex, int newIndex)
        {
            Contract.Requires<ArgumentOutOfRangeException>(oldIndex >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(newIndex >= 0);

            scheduler.Schedule(() => Move(oldIndex, newIndex));
        }

#endif

        /// <summary>Remove called on scheduler</summary>
        public void RemoveOnScheduler(T item)
        {
            scheduler.Schedule(() => Remove(item));
        }

        /// <summary>RemoveAt called on scheduler</summary>
        public void RemoveAtOnScheduler(int index)
        {
            Contract.Requires<ArgumentOutOfRangeException>(index >= 0);

            scheduler.Schedule(() => RemoveAt(index));
        }

        /// <summary>Set(indexer set) called on scheduler</summary>
        public void SetOnScheduler(int index, T value)
        {
            Contract.Requires<ArgumentOutOfRangeException>(index >= 0);

            scheduler.Schedule(() => this[index] = value);
        }

        /// <summary>Unsubcribe source sequence.</summary>
        public void Dispose()
        {
            subscription.Dispose();
        }
    }

    public static class ReactiveCollectionObservableExtensions
    {
        /// <summary>Source sequence as ObservableCollection. Operate scheduler is UIDispatcherScheduler.</summary>
        public static ReactiveCollection<T> ToReactiveCollection<T>(this IObservable<T> source)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Ensures(Contract.Result<ReactiveCollection<T>>() != null);

            return new ReactiveCollection<T>(source);
        }

        /// <summary>Source sequence as ObservableCollection. Operate scheduler is argument's scheduler.</summary>
        public static ReactiveCollection<T> ToReactiveCollection<T>(this IObservable<T> source, IScheduler scheduler)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(scheduler != null);
            Contract.Ensures(Contract.Result<ReactiveCollection<T>>() != null);

            return new ReactiveCollection<T>(source, scheduler);
        }
    }
}