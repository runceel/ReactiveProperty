using System;
using System.Diagnostics.Contracts;
#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#else
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
#endif

namespace Codeplex.Reactive.Notifiers
{
    /// <summary>
    /// Notify value on setuped scheduler.
    /// </summary>
    public class ScheduledNotifier<T> : IObservable<T>, IProgress<T>
    {
        readonly IScheduler scheduler;
        readonly Subject<T> trigger = new Subject<T>();

        /// <summary>
        /// Use scheduler is Scheduler.Immediate.
        /// </summary>
        public ScheduledNotifier()
        {
            this.scheduler = Scheduler.Immediate;
        }
        /// <summary>
        /// Use scheduler is argument's scheduler.
        /// </summary>
        public ScheduledNotifier(IScheduler scheduler)
        {
            Contract.Requires<ArgumentNullException>(scheduler != null);

            this.scheduler = scheduler;
        }

        /// <summary>
        /// Push value to subscribers on setuped scheduler.
        /// </summary>
        public void Report(T value)
        {
            scheduler.Schedule(() => trigger.OnNext(value));
        }

        /// <summary>
        /// Push value to subscribers on setuped scheduler.
        /// </summary>
        public IDisposable Report(T value, TimeSpan dueTime)
        {
            Contract.Ensures(Contract.Result<IDisposable>() != null);

            var cancel = scheduler.Schedule(dueTime, () => trigger.OnNext(value));

            Contract.Assume(cancel != null);
            return cancel;
        }

        /// <summary>
        /// Push value to subscribers on setuped scheduler.
        /// </summary>
        public IDisposable Report(T value, DateTimeOffset dueTime)
        {
            Contract.Ensures(Contract.Result<IDisposable>() != null);

            var cancel = scheduler.Schedule(dueTime, () => trigger.OnNext(value));

            Contract.Assume(cancel != null);
            return cancel;
        }

        /// <summary>
        /// Subscribe observer.
        /// </summary>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            return trigger.Subscribe(observer);
        }
    }
}