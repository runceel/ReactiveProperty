using System;
#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
using SerialDisposable = Microsoft.Phone.Reactive.MutableDisposable;
#else
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Disposables;
using System.Reactive.Concurrency;
#endif

namespace Codeplex.Reactive
{
    // TODO:Add Contracts, Document

    public class ReactiveTimer : IObservable<long>, IDisposable
    {
        long count = 0;
        readonly SerialDisposable disposable = new SerialDisposable();
        readonly IScheduler scheduler;
        readonly Subject<long> subject = new Subject<long>();

        public ReactiveTimer(TimeSpan interval)
            : this(interval, Scheduler.ThreadPool)
        {

        }

        public ReactiveTimer(TimeSpan interval, IScheduler scheduler)
        {
            Interval = interval;
            this.scheduler = scheduler;
        }

        public TimeSpan Interval { get; set; } // TODO:Normalize TimeSpan

        public void Start()
        {
            Start(TimeSpan.Zero);
        }

        public void Start(TimeSpan dueTime)
        {
            disposable.Disposable = scheduler.Schedule(dueTime,
                self =>
                {
                    subject.OnNext(count++);
                    self(Interval);
                });
        }

        public void Stop()
        {
            disposable.Disposable = Disposable.Empty;
        }

        public IDisposable Subscribe(IObserver<long> observer)
        {
            return subject.Subscribe(observer);
        }

        public void Dispose()
        {
            subject.OnCompleted();
            subject.Dispose();
            disposable.Dispose();
        }
    }
}
