using System;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;

namespace Codeplex.Reactive.Notifier
{
    public class ScheduledNotifier<T> : IObservable<T>
    {
        readonly IScheduler scheduler;
        readonly Subject<T> trigger = new Subject<T>();

        public ScheduledNotifier(IScheduler scheduler)
        {
            if (scheduler == null) throw new ArgumentNullException("scheduler");

            this.scheduler = scheduler;
        }

        public void Raise(T value)
        {
            scheduler.Schedule(() => trigger.OnNext(value));
        }

        public void Raise(T value, TimeSpan dueTime)
        {
            scheduler.Schedule(dueTime, () => trigger.OnNext(value));
        }

        public void Raise(T value, DateTimeOffset dueTime)
        {
            scheduler.Schedule(dueTime, () => trigger.OnNext(value));
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return trigger.Subscribe(observer);
        }
    }
}